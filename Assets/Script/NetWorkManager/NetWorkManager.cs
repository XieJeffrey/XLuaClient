using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.Events;
using ProtoBuf;

public class ClientProtocal
{//不要与Protocal.ContractType里面的重复
    public const short TimeOut = 10001;
    public const short Connected = 10002;
    public const short ConnectedFail = 1003;
}

public enum ServerType
{
    None,
    Hall,
    Logic,
}

public class NetworkManager : MonoBehaviour
{
    internal class MessageTimeOut
    {
        public ServerType server;
        public short protocal;
        public short messageID;
        public float beginTime;

        public MessageTimeOut(ServerType server, short protocal, short messageID, float beginTime)
        {
            this.server = server;
            this.protocal = protocal;
            this.messageID = messageID;
            this.beginTime = beginTime;
        }
    }

    internal class ServerEvent
    {
        public ServerType _type;
        public int Key;
        public ByteBuffer Value;
        public ServerEvent(ServerType type, int key, ByteBuffer buf)
        {
            _type = type;
            Key = key;
            Value = buf;
        }
    }

    //private SocketClient hallSocket;
    private SocketClient logicSocket;
    private short _messageID = 1;
    private List<MessageTimeOut> timeOutList = new List<MessageTimeOut>();
    private Queue<ServerEvent> sEvents = new Queue<ServerEvent>();        

    private bool isStopUpdate = false;

    private short MessageID
    {
        get
        {
            if (_messageID > 30000)
                return _messageID = 1;
            else
                return _messageID++;
        }
    }

    //SocketClient HallSocketClient
    //{
    //    get
    //    {
    //        if (hallSocket == null)
    //            hallSocket = new SocketClient();
    //        return hallSocket;
    //    }
    //}

    SocketClient LogicSocketClient
    {
        get
        {
            if (logicSocket == null)
                logicSocket = new SocketClient();
            return logicSocket;
        }
    }
    
    //public bool HallConnected { get { return HallSocketClient.IsConnect; } }
    public bool LogicConnected { get { return LogicSocketClient.IsConnect; } }    

    void Awake()
    {
        Init();
    }

    void Start()
    {
        
    }

    public void Init()
    {
        //HallSocketClient.OnRegister(ClientType.Hall);
        LogicSocketClient.OnRegister(ServerType.Logic);
    }
    
    //public void InitHallSocket()
    //{
    //    HallSocketClient.Close();
    //    HallSocketClient.OnRegister(ClientType.Hall);
    //}

    public void InitLogicSocket()
    {
        LogicSocketClient.Close();
        LogicSocketClient.OnRegister(ServerType.Logic);
    }

    /// <summary>
    /// 交给Command，这里不想关心发给谁。
    /// </summary>
    void Update()
    {
        if (sEvents.Count > 0)
        {
            lock (sEvents)
            {
                while (sEvents.Count > 0)
                {
                    ServerEvent _event = sEvents.Dequeue();
                    if (_event == null)
                    {
                        Util.LogError("ServerType is null");
                        continue;
                    }
                    Distribute(_event._type, _event.Key, _event.Value);
                }
            }
        }
        lock (timeOutList)
        {
            if (timeOutList.Count > 0)
            {
                float curTime = Time.unscaledTime;

                for (int i = timeOutList.Count - 1; i >= 0; i--)
                {
                    if (isStopUpdate)
                    {
                        isStopUpdate = false;
                        break;
                    }
                    MessageTimeOut mto = timeOutList[i];
                    if (curTime - mto.beginTime >= AppConst.SendMessageTimeOut)
                    {//超时了
                        if (IsHandleTimeOut(mto.protocal))
                        {//超时处理
                            Util.LogError("server->" + mto.server + "--protocal-->" + mto.protocal + "--messageID-->" + mto.messageID + "--timeout");

                            ByteBuffer buffer = new ByteBuffer();
                            buffer.WriteShort((short)mto.server);
                            buffer.WriteShort(mto.protocal);

                            ByteBuffer _buffer = new ByteBuffer(buffer.ToBytes());
                            buffer.Close();
                            AddEvent(ServerType.Logic, ClientProtocal.TimeOut, _buffer);
                            //EventManager.instance.NotifyEvent(NetEventType.TimeOut, mto.protocal);
                        }
                        timeOutList.Remove(mto);
                    }

                }
            }
        }
    }
    
    ///服务器的响应添加在这里
    public void AddEvent(ServerType type, int _event, ByteBuffer data)
    {
        ServerEvent tmp = new ServerEvent(type, _event, data);
        if (tmp != null)
            lock (sEvents)
            {
                sEvents.Enqueue(tmp);
            }
        else
            Util.LogError("========内存不够，new出来的serverType是null==========");
    }

    public void RemoveTimeOutList()
    {
        if (timeOutList != null)
        {
            timeOutList.Clear();
            isStopUpdate = true;
        }
    }

    /// <summary>
    /// 移除超时检测
    /// </summary>
    /// <param name="server"></param>
    /// <param name="protocal"></param>
    public void RemoveTimeOut(ServerType server, short messageID)
    {
        if (timeOutList.Count > 0)
        {
            lock (timeOutList)
            {
                for (int i = timeOutList.Count - 1; i >= 0; i--)
                {
                    MessageTimeOut mto = timeOutList[i];
                    if (mto.server == server && mto.messageID == messageID)
                    {
                        timeOutList.RemoveAt(i);
                    }
                }
            }
        }
    }
    /// <summary>
    /// 判断当前协议是否进行超时处理
    /// </summary>
    /// <param name="curProtocal">当前协议号</param>
    /// <returns>是否进行超时处理</returns>
    bool IsHandleTimeOut(short curProtocal)
    {
        Debug.Log(curProtocal + "  Is time out? ============================= ");
        return !IgnoreTimeoutList.List.Contains(curProtocal);
    }

    /// <summary>
    /// 发送大厅链接请求
    /// </summary>
    //public void SendConnectHall(string address, int port)
    //{
    //    HallSocketClient.SendConnect(address, port);
    //}

    /// <summary>
    /// 发送BT7链接请求
    /// </summary>
    public void SendConnectLogic(string address, int port)
    {
        Util.LogForNet("Connect to IP:" + address + " Port:" + port);
        LogicSocketClient.SendConnect(address, port);
    }

    //public void SendMessageHall(short protocal, short protocalType, MemoryStream buf, string key)
    //{
    //    if (protocal == Protocal.ContractTypes.Heartbeat)
    //    {
    //        //Debug.Log("Hall--heartbeat");
    //    }
    //    Debug.Log("Hall--protocal============" + protocal);
    //    short messageID = MessageID;
    //    timeOutList.Add(new MessageTimeOut(ServerType.Hall, protocal, messageID, Time.unscaledTime));
    //    if (buf.Length == 0)
    //    {
    //        HallSocketClient.SendMessage(protocal, messageID, null, key);
    //    }
    //    else
    //    {
    //        byte[] data = new byte[buf.Length];
    //        buf.Position = 0;
    //        buf.Read(data, 0, data.Length);
    //        HallSocketClient.SendMessage(protocal, messageID, data, key);
    //    }
    //}

    private void SendMessageLogic(short protocal, short protocalType, MemoryStream buf, string key = "")
    {
        if (!LogicSocketClient.IsConnect)
        {
            Util.LogForNet("Socket 未连接无法发送消息");
            return;
        }
        if (protocal == Protocal.ContractTypes.Heartbeat)
        {
            //Debug.Log("BT7--heartbeat");
        }
        Util.LogForNet("===Send Message of Protocal NO." + protocal);
        short messageID = MessageID;
        timeOutList.Add(new MessageTimeOut(ServerType.Logic, protocal, messageID, Time.unscaledTime));
        if (buf.Length == 0)
        {
            LogicSocketClient.SendMessage(protocal, messageID, null, key);
        }
        else
        {
            byte[] data = new byte[buf.Length];
            buf.Position = 0;
            buf.Read(data, 0, data.Length);
            LogicSocketClient.SendMessage(protocal, messageID, data, key);
        }
    }

    #region 跟逻辑模块对接
    public delegate void serverAction(ServerType type, int key, ByteBuffer buffer);
    private Dictionary<int, List<serverAction>> m_observerLst = new Dictionary<int, List<serverAction>>();

    private Dictionary<Type, Manager> m_initManagerList = new Dictionary<Type, Manager>();
    private bool isInit = false;
    public void InitLogicManager()
    {
        foreach (ManagerType m_type in Enum.GetValues(typeof(ManagerType)))
        {
            Type managerType = Type.GetType(m_type.ToString());
            if (m_initManagerList.ContainsKey(managerType))
                continue;
            Manager tmp = Activator.CreateInstance(managerType) as Manager;
            Util.Log("{0} OnInit", managerType.Name);
            tmp.OnInit();
            m_initManagerList.Add(managerType, tmp);
        }
        isInit = true;
    }

    private void Distribute(ServerType _type, int key, ByteBuffer buffer)
    {
        //10000+ 的是客户端自己定义的
        Util.LogForNet("Distribute Key:" + key + "============");
        List<serverAction> fun;

        m_observerLst.TryGetValue(key, out fun);
        if (null != fun)
        {
            for (int i = 0; i < fun.Count; i++)
                fun[i](_type, key, buffer);
        }
    }

    public void AddListener(int key, serverAction fun)
    {
        if (m_observerLst.ContainsKey(key))
        {
            if (m_observerLst[key].Contains(fun) == false)
            {
                m_observerLst[key].Add(fun);
            }
        }
        else
        {
            m_observerLst.Add(key, new List<serverAction>());
            m_observerLst[key].Add(fun);
        }
    }


    public void RemoveListener(int key, serverAction fun)
    {
        if (m_observerLst.ContainsKey(key))
        {
            m_observerLst[key].Remove(fun);
        }
    }

    public T Deserialize<T>(ByteBuffer buffer)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            byte[] d = buffer.ReadBytes();
            ms.Write(d, 0, d.Length);
            ms.Position = 0;
            T loginMsg = Serializer.Deserialize<T>(ms);
            return loginMsg;
        }
    }

    public void SendMsg(short protocal)
    {
        SendMessageLogic(protocal, 0, new MemoryStream());
    }

    public void SendMsg<T>(short protocal, object data)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            Serializer.Serialize<T>(ms, (T)data);
            SendMessageLogic(protocal, 0, ms);
        }
    }
    #endregion

#if ISDEBUG
    void OnGUI()
    {
        if (!LogicSocketClient.IsConnect)
        {
            GUI.Label(new Rect(Screen.width - 60, 0, 30, 80), "Logic");
        }
        //if (LoginMgr.instance != null)
        //{
        //    if (LoginMgr.instance.isBT7Logined)
        //    {
        //        GUI.Label(new Rect(Screen.width - 300, 0, 70, 80), "BT7_On");
        //    }

        //    if (LoginMgr.instance.isHallLogined)
        //    {
        //        GUI.Label(new Rect(Screen.width - 200, 0, 50, 80), "Hall_On");
        //    }
        //}
    }
#endif


    /// <summary>
    /// 关闭连接
    /// </summary>
    public void Close()
    {
        // AppFacade.Instance.RemoveCommand(NotiConst.DISPATCH_MESSAGE);
        //Debug.LogWarning("Close the HallSocketClient");
        //HallSocketClient.Close();
        Debug.LogWarning("Close the LogicSocketClient");
        LogicSocketClient.Close();
        timeOutList.Clear();
        isStopUpdate = true;

        sEvents.Clear();
        //hallSocket = null;
        //bt7Socket = null;
    }

    void OnDestroy()
    {
        //HallSocketClient.OnRemove();
        LogicSocketClient.OnRemove();
        Debug.Log("~NetworkManager was destroy");
    }
}
using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;

public enum DisType
{
    Exception,
    Disconnect,
}

public class SocketClient
{      
    private NetworkStream outStream = null;
    private MemoryStream memStream;
    private BinaryReader reader;
    private ServerType socketType;
    private bool isConnect = false;
    private const int HEAD_SIZE = 12;
    private const int MAX_READ = 8000;
    private byte[] byteBuffer = new byte[MAX_READ];

    /// <summary>
    /// 注册代理
    /// </summary>
    public void OnRegister(ServerType param)
    {        
        socketType = param;
        isConnect = false;
        memStream = new MemoryStream();
        reader = new BinaryReader(memStream);
    }

    /// <summary>
    /// 移除代理
    /// </summary>
    public void OnRemove()
    {
        this.Close();
    }

    public bool IsConnect
    {
        get
        {
            return isConnect;
        }
    }

    #region connect
    /// <summary>
    /// 发送连接请求
    /// </summary>
    public void SendConnect(string address, int port)
    {
        ConnectServer(address, port);
    }

    /// <summary>
    /// 连接服务器
    /// </summary>
    void ConnectServer(string host, int port)
    {
        TcpClient client = new TcpClient();
        client.SendTimeout = 1000;
        client.ReceiveTimeout = 1000;
        client.NoDelay = true;
        try
        {
            client.BeginConnect(host, port, new AsyncCallback(OnConnect), client);
        }
        catch (Exception e)
        {               
            Close();
        }
    }

    /// <summary>
    /// 连接上服务器
    /// </summary>
    void OnConnect(IAsyncResult asr)
    {
        TcpClient client = null;
        try
        {
            client = (TcpClient)asr.AsyncState;            
            client.EndConnect(asr);
            if (!client.Connected)
            {//连接失败
                Util.LogError("连接服务器失败!请检查网络");
                Main.NetworkManager.AddEvent(socketType, ClientProtocal.ConnectedFail, null);
                return;
            }
            isConnect = true;
            outStream = client.GetStream();
            outStream.BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), client);
            Main.NetworkManager.AddEvent(socketType, ClientProtocal.Connected, null);            
        }
        catch (Exception e)
        {
            Close();
            isConnect = false;
            //Util.LogError("OnConnect exception msg:" + e.Message);
            //if (LoginWindow.instance == null || !LoginWindow.instance.IsShow)
            //    ReConnectMgr.instance.ConnectFail();
        }
    }
    #endregion

    #region write

    public void SendMessage(short protocal, short messageID, byte[] buf, string key)
    {
        //if (protocal != Protocal.Heartbeat)
        //{
        //    Util.Log("发送大厅消息,编号=========== " + protocal);
        //}
        // 加密          
        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteShort(protocal);
        buffer.WriteShort(messageID);
        if (buf != null)
            //buffer.WriteBytes(SecurityManager.Encrypt(protocal, buf, key));   //加密
            buffer.WriteBytes(buf);
        else
            buffer.WriteInt(0);

        byte[] bytes = buffer.ToBytes();
        WriteMessage(bytes, messageID, protocal);
        buffer.Close();
    }

    /// <summary>
    /// 写数据
    /// </summary>
    void WriteMessage(byte[] newBytes, short messageID, short protocal)
    {
        if (!isConnect)
            return;

        MemoryStream ms = null;
        using (ms = new MemoryStream())
        {
            ms.Position = 0;
            BinaryWriter writer = new BinaryWriter(ms);
            int msglen = (int)newBytes.Length + 4;            
            writer.Write(msglen);
            writer.Write(newBytes);
            writer.Flush();
            if (isConnect)
            {
                byte[] payload = ms.ToArray();                
                outStream.BeginWrite(payload, 0, payload.Length, new AsyncCallback(OnWrite), null);
            }
            else
            {
                Debug.LogWarning("client.connected----->>false");
                ByteBuffer buffer = new ByteBuffer();
                buffer.WriteShort(protocal);

                ByteBuffer _buffer = new ByteBuffer(buffer.ToBytes());
                buffer.Close();

                Main.NetworkManager.RemoveTimeOut(socketType, messageID);
                Main.NetworkManager.AddEvent(socketType, ClientProtocal.ConnectedFail, _buffer);
            }
        }
    }

    /// <summary>
    /// 向链接写入数据流
    /// </summary>
    void OnWrite(IAsyncResult r)
    {
        try
        {
            outStream.EndWrite(r);
        }
        catch (Exception ex)
        {
            //  Util.Log("Catch Exception!:{0}", ex.StackTrace);
            OnDisconnected(DisType.Exception, ex.Message + "OnWirte-----------" + ex.StackTrace);
        }
    }
    #endregion

    #region read

    /// <summary>
    /// 读取消息
    /// </summary>
    void OnRead(IAsyncResult asr)
    {
        int bytesRead = 0;
        TcpClient client = (TcpClient)asr.AsyncState;
        try
        {
            //读取字节流到缓冲区             
            lock (outStream)
            {
                bytesRead = outStream.EndRead(asr);
                //Util.Log(bytesRead.ToString());

                if (bytesRead < 1)
                {
                    //包尺寸有问题，断线处理
                    //if (ReConnectMgr.instance != null)
                    //{
                    //    ReConnectMgr.instance.ConnectFail();   //2017/12/4 断线重连做好就打开这里
                    //}
                    return;
                }
                OnReceive(byteBuffer, bytesRead);
                // 分析完，再次监听服务器发过来的新消息
                Array.Clear(byteBuffer, 0, byteBuffer.Length);   //清空数组
                outStream.BeginRead(byteBuffer, 0, MAX_READ, new AsyncCallback(OnRead), client);
            }
        }
        catch (Exception ex)
        {
            //Util.Log("Catch Exception!:{0}", ex.StackTrace);
            OnDisconnected(DisType.Exception, ex.Message + "OnRead-----------" + ex.StackTrace);
        }
    }

    /// <summary>
    /// 接收到消息
    /// </summary>
    void OnReceive(byte[] bytes, int length)
    {
        memStream.Seek(0, SeekOrigin.End);
        memStream.Write(bytes, 0, length);
        //Reset to beginning
        memStream.Seek(0, SeekOrigin.Begin);

        while (RemainingBytes() >= HEAD_SIZE)
        {
            int messageLen = reader.ReadInt32();
            short protocal = reader.ReadInt16();
            short messageID = reader.ReadInt16();
            int bodyLength = reader.ReadInt32();
            //if (protocal != Protocal.Heartbeat)
            //{
            //    Util.OtherLog("收到大厅消息,编号=================" + protocal);
            //}
            messageLen = messageLen - HEAD_SIZE;
            if (RemainingBytes() >= bodyLength)
            {
                MemoryStream ms = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(ms);
                writer.Write(reader.ReadBytes(messageLen));
                ms.Seek(0, SeekOrigin.Begin);
                OnReceivedMessage(ms, protocal, messageID, bodyLength);
            }
            else
            {
                //Back up the position two bytes
                memStream.Position = memStream.Position - HEAD_SIZE;
                break;
            }
        }
        //Create a new stream with any leftover bytes
        byte[] leftover = reader.ReadBytes((int)RemainingBytes());
        memStream.SetLength(0);     //Clear
        memStream.Write(leftover, 0, leftover.Length);
    }

    /// <summary>
    /// 剩余的字节
    /// </summary>
    private long RemainingBytes()
    {
        return memStream.Length - memStream.Position;
    }

    /// <summary>
    /// 接收到消息
    /// startIndex:0 length:4
    /// MessageType startIndex:4 length:2
    /// ProtocalType startIndex:6 length:2
    /// BodyLength startIndex:8 length:4
    /// </summary>
    /// <param name="ms"></param>
    void OnReceivedMessage(MemoryStream ms, short protocal, short messageID, int bufLength)
    {
        BinaryReader r = new BinaryReader(ms);
        //if (protocal != Protocal.Heartbeat)
        //{
        //    Util.OtherLog("解析大厅消息,编号==================" + protocal);
        //}
        //移除超时
        Main.NetworkManager.RemoveTimeOut(socketType, messageID);

        byte[] oldBytes = r.ReadBytes((int)(ms.Length - ms.Position));
        byte[] newBytes=new byte[1024];
        //if (clientType == ClientType.Hall)
        //    newBytes = SecurityManager.Decrypt(protocal, oldBytes, AppConst.desHallKey);
        //else
        //    newBytes = SecurityManager.Decrypt(protocal, oldBytes, AppConst.desBt7Key);

        ByteBuffer buffer = new ByteBuffer();
        buffer.WriteBytes(oldBytes);
        ByteBuffer _buffer = new ByteBuffer(buffer.ToBytes());
        buffer.Close();
        Main.NetworkManager.AddEvent(socketType, protocal, _buffer);
    }

    #endregion

    #region other
    /// <summary>
    /// 丢失链接
    /// </summary>
    void OnDisconnected(DisType dis, string msg)
    {
        Util.LogForNet("服务器关掉了!");
        Debug.Log("!Connection was closed by the server:>" + msg + " Distype:>" + dis);
        Close();   //关掉客户端链接      
    }

    /// <summary>
    /// 关闭链接
    /// </summary>
    public void Close()
    {
        if (!isConnect)
            return;
        Debug.Log("Close Client");
        isConnect = false;
        reader.Close();
        Array.Clear(byteBuffer, 0, byteBuffer.Length);   //清空数组
        outStream.Close();
        outStream.Dispose();
        memStream.Close();
        memStream.Dispose();
    }
    #endregion
}
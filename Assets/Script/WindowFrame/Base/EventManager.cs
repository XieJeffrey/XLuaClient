using System.Collections;
using System.Collections.Generic;



public class EventManager : Singleton<EventManager>
{
    public class UIEvent
    {
        public UIEventType eventType;
        public Window window;
        public VoidHandle handle;
    }

    public class MgrEvent
    {
        public NetEventType eventType;
        public VoidHandle handle;
    }

    public delegate void VoidHandle(params object[] param);
    public delegate void voidHandle();
    private List<UIEvent> uiEventList = new List<UIEvent>();
    private List<MgrEvent> mgrEventList = new List<MgrEvent>();

    #region UI消息分发
    public void RegistEvent(UIEventType m_event, Window m_win, VoidHandle handle)
    {
        UIEvent tmp = new UIEvent();
        tmp.eventType = m_event;
        tmp.window = m_win;
        tmp.handle = handle;
        if (uiEventList.Contains(tmp) == false)
        {
            uiEventList.Add(tmp);
        }
    }

    public void RemoveEvent(UIEventType m_event)
    {
        for (int i = 0; i < uiEventList.Count; i++)
        {
            if (uiEventList[i].eventType == m_event)
            {
                uiEventList.RemoveAt(i);
                i--;
            }
        }
    }

    public void RemoveEvent(Window win)
    {
        for (int i = 0; i < uiEventList.Count; i++)
        {
            if (uiEventList[i].window == win)
            {
                uiEventList.RemoveAt(i);
                i--;
            }
        }
    }

    public void NotifyUIEvent(UIEventType m_event, params object[] param)
    {
        for (int i = 0; i < uiEventList.Count; i++)
        {
            if (uiEventList[i].eventType == m_event && uiEventList[i].window.IsShow)
            {
                uiEventList[i].handle(param);
            }
        }
    }

    #endregion

    #region logic管理器事件分发
    public void RegistEvent(NetEventType m_event, VoidHandle handle)
    {
        MgrEvent tmp = new MgrEvent();
        tmp.eventType = m_event;
        tmp.handle = handle;
        if (mgrEventList.Contains(tmp) == false)
            mgrEventList.Add(tmp);
    }

    public void RemoveEvent(NetEventType m_event, VoidHandle handle)
    {
        for (int i = 0; i < mgrEventList.Count; i++)
        {
            if (mgrEventList[i].eventType == m_event)
            {
                mgrEventList.RemoveAt(i);
                i--;
            }
        }
    }

    public void NotifyEvent(NetEventType m_event, params object[] param)
    {
        for (int i = 0; i < mgrEventList.Count; i++)
        {
            if (mgrEventList[i].eventType == m_event)
            {
                mgrEventList[i].handle(param);
            }
        }
    }
    #endregion
}


using System.Collections;
using System.Collections.Generic;



    public class Manager
    {
        private Dictionary<NetEventType, EventManager.VoidHandle> m_eventList = new Dictionary<NetEventType, EventManager.VoidHandle>();
        public virtual void OnInit()
        { }

        public virtual void OnDestory()
        {
            var tmp = m_eventList.GetEnumerator();
            while (tmp.MoveNext())
            {
                EventManager.instance.RemoveEvent(tmp.Current.Key, tmp.Current.Value);
            }
            m_eventList.Clear();
        }

        public void Register(NetEventType m_eventType, EventManager.VoidHandle handle)
        {            
            if (m_eventList.ContainsKey(m_eventType) == false)
                m_eventList.Add(m_eventType, handle);
            else
            { }
            //SimpleFramework.Util.LogError("{0}已经被注册了", m_eventType.ToString());
        }
    }


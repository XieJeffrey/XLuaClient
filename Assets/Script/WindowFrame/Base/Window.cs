using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Window
{
    public Window()
    {
        m_name = GetType().Name;
    }

    #region UI事件

    protected void RegistEvent(UIEventType UIEventType, EventManager.VoidHandle voidHandle)
    {
        EventManager.instance.RegistEvent(UIEventType, this, voidHandle);
    }

    protected void RemoveEvent(UIEventType UIEventType)
    {
        EventManager.instance.RemoveEvent(UIEventType);
    }

    #endregion

    #region 虚函数

    public virtual void OnShow(params object[] param)
    {
    }

    public virtual void OnClose()
    {
        //string abName = Util.Upper2LowerAnd_(m_name.Replace("Window", ""));
        // string abName = m_name.Replace("Window", "").ToLower();
#if !UNITY_EDITOR
        Main.ResManager.UnLoadAssetBundle(bundle);
#endif     
    }

    public virtual void OnInit()
    {
        if (!m_isInit)
        {
            if (m_gameObject.GetComponent<Canvas>() == null)
            {
                m_gameObject.AddComponent<Canvas>();
            }
            if (m_mono == null)
            {
                m_mono = m_gameObject.AddComponent<emptyMono>();
            }           

            RegistEvents();
        }
    }

    public virtual void RegistEvents()
    {
        // SimpleFramework.Util.Log("registEvents");
    }

    private void RemoveEvents()
    {
        // SimpleFramework.Util.LogError("windowName:{0} removeEvent", m_name);
        EventManager.instance.RemoveEvent(this);
        m_eventLsit.Clear();
    }

    public virtual void OnUpdate()
    {

    }

    public virtual void OnLateUpdate()
    { }

    public virtual void OnHide()
    { }

    #endregion

    #region 成员

    public Canvas m_panel;
    public WindowType m_winEnumType;
    private bool m_isInit = false;
    public UnityEngine.GameObject m_gameObject;
    public List<UIEventType> m_eventLsit = new List<UIEventType>();
    private bool m_isShow = false;
    public emptyMono m_mono;

    public bool IsShow
    {
        get { return m_isShow; }
    }
    private bool m_isLoad = false;

    public bool IsLoad
    {
        get { return m_isLoad; }
    }

    public winType m_wndType = winType.TypeNormal;

    private int m_sortingOrder;

    public int SortingOrder
    {
        get { return m_sortingOrder; }
        set
        {
            m_sortingOrder = value;
        }
    }

    public virtual string resUrl
    {
        get { return string.Empty; }
    }

    public virtual string bundle
    {
        get { return string.Empty; }
    }

    public virtual ResourceType resType
    {
        get { return ResourceType.AssetBundle; }
    }

    public string Name
    {
        get
        {
            return m_name;
        }
    }

    private string m_name;

    public Dictionary<int, Dictionary<string, object>> m_cache_go = new Dictionary<int, Dictionary<string, object>>();

    public List<WindowType> brotherWindowLst = new List<WindowType>();
    #endregion
    public void OnLoad()
    {
        RemoveEvents();
        m_panel = m_gameObject.GetComponent<Canvas>();
        if (m_panel == null)
        {
            m_panel = m_gameObject.AddComponent<Canvas>();
        }
        if (m_gameObject.GetComponent<GraphicRaycaster>() == null)
            m_gameObject.AddComponent<GraphicRaycaster>();
        m_panel.overrideSorting = true;
     
        //todo
        //m_gameObject.transform.SetParent(SimpleFramework.LuaHelper.GetPanelManager().parent);
        m_gameObject.transform.localScale = Vector3.one;
        m_gameObject.transform.localPosition = new Vector3(0, 0, 0);
        if (m_isInit == false)
        {
            OnInit();
            m_isInit = true;
        }
        m_isLoad = true;
        //if (m_sortingOrder == 0)
        //{
        //    
        //}

        if(m_sortingOrder==0)
            m_sortingOrder = WindowManager.instance.CurrentOrder;
        m_panel = m_gameObject.GetComponent<Canvas>();
        //if (m_panel.sortingOrder == 0)
        //{         
        //    m_panel.overrideSorting = true;
        //    m_panel.sortingOrder = m_sortingOrder;
        //}
        m_panel.overrideSorting = true;
        m_panel.sortingOrder = m_sortingOrder;

        Canvas[] m_panel_array = m_gameObject.GetComponentsInChildren<Canvas>();
        foreach (Transform t in m_gameObject.transform)
        {
            Canvas tmp = t.GetComponent<Canvas>();
            if (tmp == null)
                continue;

            if(tmp==m_panel)
                continue;

            tmp.overrideSorting = true;
            tmp.sortingOrder = tmp.sortingOrder + m_panel.sortingOrder;
        }       
    }

    public void Close()
    {
        //Util.Log("Close window:{0}", m_name);   
        RemoveEvents();
        m_mono.StopAllCoroutines();
        m_cache_go.Clear();
        m_isLoad = false;
        m_isShow = false;
        m_isInit = false;
        OnClose();
        WindowManager.instance.RemoveWindow(this);
        for (int i = 0; i < brotherWindowLst.Count; i++)
        {
            WindowManager.instance.CloseWindow(brotherWindowLst[i]);
        }
        brotherWindowLst.Clear();
        GameObject.Destroy(m_gameObject);
    }


    public void Hide()
    {
        try
        {
            // Util.Log("Hide window:{0}", m_name);
            m_mono.StopAllCoroutines();
            m_gameObject.SetActive(false);
            // UIHelper.instance.SetActive(m_gameObject, false);
            m_isShow = false;
            OnHide();
        }
        catch (System.Exception e)
        {
            m_isLoad = false;
            m_isInit = false;
            m_isShow = false;
        }
    }

    public void Show(params object[] param)
    {
        //   Util.Log("Show window:{0}", m_name);
        m_gameObject.SetActive(true);
        //UIHelper.instance.SetActive(m_gameObject, true);
        m_isShow = true;
        OnShow(param);
    }

    public UIEventListener Register(GameObject go)
    {
        return UIEventListener.Get(go);
    }

    public T Find<T>(string goName) where T : class
    {
        if (string.IsNullOrEmpty(goName))
            return null;
        var tmpValue = m_cache_go.GetEnumerator();
        while (tmpValue.MoveNext())
        {
            Dictionary<string, object> tmpValue1 = tmpValue.Current.Value;
            if (tmpValue1.ContainsKey(goName))
                return (tmpValue1[goName] as GameObject).GetComponent<T>();
        }

        GameObject tmp = Find(goName);
        if (tmp != null)
            return tmp.GetComponent<T>();

        return null;
    }

    public GameObject Find(string goName)
    {
        if (string.IsNullOrEmpty(goName))
            return null;
        var tmpValue = m_cache_go.GetEnumerator();
        while (tmpValue.MoveNext())
        {
            Dictionary<string, object> tmpValue1 = tmpValue.Current.Value;
            if (tmpValue1.ContainsKey(goName))
                return (tmpValue1[goName] as GameObject);
        }

        GameObject go = UIHelper.instance.Find(m_gameObject, goName);
        if (go != null)
        {
            int parentID = go.transform.parent.gameObject.GetInstanceID();
            if (m_cache_go.ContainsKey(parentID) == false)
                m_cache_go.Add(parentID, new Dictionary<string, object>());
            if (m_cache_go[parentID].ContainsKey(goName) == false)
                m_cache_go[parentID].Add(goName, go);
        }
        return go;
    }

    public GameObject Find(GameObject go, string goName)
    {
        if (go == null)
            return null;
        if (string.IsNullOrEmpty(goName))
            return null;
        int parentID = go.GetInstanceID();
        if (m_cache_go.ContainsKey(parentID))
        {
            if (m_cache_go[parentID].ContainsKey(goName))
                return m_cache_go[parentID][goName] as GameObject;
        }

        GameObject tmpObj = UIHelper.instance.Find(go, goName);
        if (tmpObj != null)
        {
            if (m_cache_go.ContainsKey(parentID) == false)
                m_cache_go.Add(parentID, new Dictionary<string, object>());
            m_cache_go[parentID].Add(goName, tmpObj);
        }
        return tmpObj;
    }

    public T Find<T>(GameObject go, string goName) where T : MonoBehaviour
    {
        GameObject tmpObj = Find(go, goName);
        if (tmpObj != null)
        {
            return tmpObj.GetComponent<T>();
        }
        return null;
    }

    public GameObject Find(string parentName, string goName)
    {
        if (string.IsNullOrEmpty(parentName) || string.IsNullOrEmpty(goName))
            return null;
        string key = parentName + goName;


        GameObject go = null;
        GameObject parent = null;
        parent = UIHelper.instance.Find(m_gameObject, parentName);
        if (parent != null)
        {
            go = Find(parent, goName);
            return go;
        }
        else
        {
            //SimpleFramework.Util.LogError("can't find parent:{0} ", parentName);
            return null;
        }
    }

    public T Find<T>(string parentName, string goName) where T : class
    {
        if (string.IsNullOrEmpty(parentName) || string.IsNullOrEmpty(goName))
            return null;
        GameObject go = Find(parentName, goName);
        T component = go.GetComponent<T>();
        if (component != null)
            return component;
        return null;
    }

    public void SetActive(GameObject go, bool isActive)
    {
        UIHelper.instance.SetActive(go, isActive);
    }
}

public enum winType
{
    TypeNormal = 1,
    TypeCache = 2,
}


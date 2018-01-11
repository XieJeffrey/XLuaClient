using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class PanelManager : Singleton<PanelManager>
{
    private GameObject m_uiRoot;
    private Transform parent;

#if UNITY_EDITOR
    public Dictionary<string, string> m_windowPrefabPath = new Dictionary<string, string>();
    public List<string> m_fileList = new List<string>();

    public PanelManager()
    {
        string filePath = Application.dataPath + "/GameRes/Prefab/UI/";
        DirectoryInfo folder = new DirectoryInfo(filePath);
        foreach (FileInfo file in folder.GetFiles())
        {
            if (file.FullName.Contains("meta"))
                continue;
            string key = file.Name.Split('.')[0].ToLower();
            if (m_windowPrefabPath.ContainsKey(key) == false)
            {
                int index = file.FullName.IndexOf("Assets");
                m_windowPrefabPath.Add(key, file.FullName.Substring(index, file.FullName.Length - index));
            }
        }

        foreach (DirectoryInfo nextfolder in folder.GetDirectories())
        {
            foreach (FileInfo file in nextfolder.GetFiles())
            {
                if (file.FullName.Contains("meta"))
                    continue;
                string key = file.Name.Split('.')[0].ToLower();
                if (m_windowPrefabPath.ContainsKey(key) == false)
                {
                    int index = file.FullName.IndexOf("Assets");
                    m_windowPrefabPath.Add(key, file.FullName.Substring(index, file.FullName.Length - index));
                }
            }
        }
    }
#endif
    Transform Parent
    {
        get
        {

            if (parent == null)
            {
                m_uiRoot = GameObject.FindGameObjectWithTag("UIRoot");
                if (m_uiRoot != null)
                    parent = m_uiRoot.transform;
            }
            return parent;
        }
    }

    public float RefWidth
    {
        get
        {
            if (m_uiRoot != null)
            {
                return m_uiRoot.GetComponent<CanvasScaler>().referenceResolution.x;
            }
            return 0;
        }
    }

    public float RefHeight
    {
        get
        {
            if (m_uiRoot != null)
            {
                return m_uiRoot.GetComponent<CanvasScaler>().referenceResolution.y;
            }
            return 0;
        }
    }

    public void CreatePanel(Window win, string name, WindowFactory.VoidHandle func = null, bool isShow = true, params object[] param)
    {
        try
        {
            //string abName = Util.Upper2LowerAnd_(name) + AppConst.ExtName;
            string abName = name.ToLower() + AppConst.ExtName;
            string assetName = name;
            string objName = assetName.Replace("_pad", "") + "Panel";
            Util.Log("CreatePanel::>>name-->" + name + "--abName-->" + abName + "====DataPath=========" + Util.DataPath);

#if UNITY_EDITOR
            UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath(m_windowPrefabPath[name.ToLower()], typeof(GameObject));
            GameObject prefab = obj as GameObject;
            if (prefab == null)
            {
                WindowFactory.instance.OnStopLoad(win.m_winEnumType);
                return;
            }
            if (Parent.Find(name) != null)
            {
                Util.Log("��һ�������Ԥ����====" + name);
                GameObject.Destroy(Parent.Find(name).gameObject);
            }
            if (Parent.Find(objName) != null)
            {
                Util.Log("�Ѿ�������====" + objName);
                GameObject.Destroy(Parent.Find(objName).gameObject);
            }
            GameObject go = GameObject.Instantiate(prefab) as GameObject;
            Vector3 anchoredPosition3D = go.GetComponent<RectTransform>().anchoredPosition3D;
            go.name = assetName + "Panel";
            go.layer = LayerMask.NameToLayer("UI");
            go.transform.SetParent(Parent);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            RectTransform rtf = go.GetComponent<RectTransform>();
            rtf.sizeDelta = Vector2.zero;
            rtf.anchoredPosition3D = anchoredPosition3D;

            win.m_gameObject = go;
            WindowManager.instance.InsertWindow(win);
            WindowFactory.instance.OnCreateWindow(win);
            Util.Log("finishLoad:{0},show:{1}", win.ToString(), isShow);
            win.OnLoad();
            if (isShow)
                win.Show(param);
            else
                win.Hide();

            if (func != null)
            {
                func();
            }
#else

            Main.ResManager.LoadPrefab(abName, assetName, delegate (UnityEngine.Object[] objs)
            {
                if (objs.Length > 0)
                {
                    if (objs[0] != null)
                    {
                        GameObject prefab = objs[0] as GameObject;
                        if (prefab == null)
                        {
                            WindowFactory.instance.OnStopLoad(win.m_winEnumType);
                            return;
                        }
                        if (Parent.Find(name) != null)
                        {
                            Util.Log("��һ�������Ԥ����====" + name);
                            GameObject.Destroy(Parent.Find(name).gameObject);
                        }
                        if (Parent.Find(objName) != null)
                        {
                            Util.Log("�Ѿ�������====" + objName);
                            GameObject.Destroy(Parent.Find(objName).gameObject);
                        }
                        GameObject go =GameObject.Instantiate(prefab) as GameObject;
                        Vector3 anchoredPosition3D = go.GetComponent<RectTransform>().anchoredPosition3D;
                        go.name = assetName + "Panel";
                        go.layer = LayerMask.NameToLayer("UI");
                        go.transform.SetParent(Parent);
                        go.transform.localScale = Vector3.one;
                        go.transform.localPosition = Vector3.zero;
                        RectTransform rtf = go.GetComponent<RectTransform>();
                        rtf.sizeDelta = Vector2.zero;
                        rtf.anchoredPosition3D = anchoredPosition3D;
                       
                        win.m_gameObject = go;
                        WindowManager.instance.InsertWindow(win);
                        WindowFactory.instance.OnCreateWindow(win);
                        Util.Log("finishLoad:{0},show:{1}", win.ToString(), isShow);
                        win.OnLoad();
                        if (isShow)
                            win.Show(param);
                        else
                            win.Hide();

                        if (func != null)
                        {
                            func();
                        }
                    }
                    else
                    {
                        WindowFactory.instance.OnStopLoad(win.m_winEnumType);
                    }
                }
                else
                {
                    WindowFactory.instance.OnStopLoad(win.m_winEnumType);
                }
            });
#endif
        }
        catch (System.Exception e)
        {
            Util.Log(e.Message + " " + e.StackTrace);
        }
    }

}

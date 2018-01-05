using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : Singleton<PanelManager>
{
    private Transform parent;
    Transform Parent
    {
        get
        {

            if (parent == null)
            {
                GameObject go = GameObject.Find("Canvas");
                if (go != null)
                    parent = go.transform;
            }
            return parent;
        }
    }

    public void CreatePanel(Window win, string name, WindowFactory.VoidHandle func = null, bool isShow = true, params object[] param)
    {
        string abName = Util.Upper2LowerAnd_(name) + AppConst.ExtName;
        string assetName = name;
#if UNITY_EDITOR     
        UnityEngine.Object tmp = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(WindowFactory.instance.m_windowPath[name.ToLower()]);

        if (tmp != null)
        {
            GameObject prefab = tmp as GameObject;
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
            string objName = assetName.Replace("_pad", "") + "Panel";
            if (Parent.Find(objName) != null)
            {
                Util.Log("�Ѿ�������====" + objName);
                GameObject.Destroy(Parent.Find(objName).gameObject);
            }
            GameObject go = GameObject.Instantiate(prefab) as GameObject;
            Vector2 sizeDelta = go.GetComponent<RectTransform>().sizeDelta;
            Vector3 anchoredPosition3D = go.GetComponent<RectTransform>().anchoredPosition3D;
            go.name = assetName.Replace("_pad", "") + "Panel";
            go.layer = LayerMask.NameToLayer("Default");
            go.transform.SetParent(Parent);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            RectTransform rtf = go.GetComponent<RectTransform>();
            rtf.sizeDelta = sizeDelta;
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

#else
        try
        {
          
            string objName = assetName.Replace("_pad", "") + "Panel";
            Util.Log("CreatePanel::>>name-->" + name + "--abName-->" + abName + "====DataPath=========" + Util.DataPath);
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
                        Vector2 sizeDelta = go.GetComponent<RectTransform>().sizeDelta;
                        Vector3 anchoredPosition3D = go.GetComponent<RectTransform>().anchoredPosition3D;
                        go.name = assetName.Replace("_pad", "") + "Panel";
                        go.layer = LayerMask.NameToLayer("Default");
                        go.transform.SetParent(Parent);
                        go.transform.localScale = Vector3.one;
                        go.transform.localPosition = Vector3.zero;
                        RectTransform rtf = go.GetComponent<RectTransform>();
                        rtf.sizeDelta = sizeDelta;
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
        }
        catch (System.Exception e)
        {
            Util.Log(e.Message + " " + e.StackTrace);
        }
#endif
    }

}

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



    public class UIHelper : Singleton<UIHelper>
    {
        private Dictionary<string, Texture> m_textureRes = new Dictionary<string, Texture>();

        public void CreateWindow(Window win)
        {
            CreateWindow(win, null);
        }

        public void CreateWindow(Window win, WindowFactory.VoidHandle handle, bool isShow = true, params object[] param)
        {
            // SimpleFramework.Util.Log("Create Window name：{0}，resType:{1}", win.Name, win.resType);
            if (WindowManager.instance.IsCloseingAll)
                return;
            if (win.IsLoad)
            {
                if (isShow)
                    win.Show(param);
                else
                    win.Hide();
                return;
            }

            PanelManager.instance.CreatePanel(win, win.bundle, handle, isShow, param);

            //switch (win.resType)
            //{
            //    case ResourceType.Resource:

            //        break;
            //    case ResourceType.AssetBundle:
            //        // SimpleFramework.LuaHelper.GetPanelManager().CreatePanel();
            //        break;
            //}
        }

        public GameObject Find(GameObject m_go, string goName)
        {
            if (m_go == null)
            {
                // SimpleFramework.Util.LogError("can't find gameObject Name:{0}", goName);
                return null;
            }
            Transform trans = m_go.transform.Find(goName);
            if (trans != null)
                return trans.gameObject;

            //  SimpleFramework.Util.LogError("can't find gameObject Name:{1},parent:{0}", m_go.name, goName);
            return null;
        }


        public T Find<T>(GameObject m_go, string goName) where T : class
        {
            GameObject go = Find(m_go, goName);
            if (null != go)
            {
                T component = go.GetComponent<T>();
                return component;
            }
            //  SimpleFramework.Util.LogError("can't find gameObject component:{0},Name:{1},parent:{2}", typeof(T).Name, m_go.name, goName);
            return null;
        }

        public void SetActive(GameObject go, bool isActive)
        {
            if (go == null)
                return;
            if (go.activeSelf)
            {
                if (isActive)
                    go.transform.localScale = Vector3.one;
                else
                    go.transform.localScale = new Vector3(0.0001f, 0.0001f, 0.001f);
            }
            else
            {
                if (isActive)
                {
                    go.SetActive(true);
                    go.transform.localScale = Vector3.one;
                }
            }

        }
    }



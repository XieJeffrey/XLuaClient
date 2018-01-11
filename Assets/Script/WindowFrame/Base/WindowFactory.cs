using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class WindowFactory : Singleton<WindowFactory>
{
    private List<WindowType> m_winList = new List<WindowType>();
    public delegate void VoidHandle(params object[] param);

    public void CreateWindow(WindowType winType, VoidHandle handle = null, bool isShow = true, params object[] param)
    {       
        if (m_winList.Contains(winType))
            return;
        Window win = WindowManager.instance.GetWindow(winType);
        if (m_winList.Contains(winType) == false)
            m_winList.Add(winType);
        if (win == null)
        {
            win = (Window)Activator.CreateInstance(Type.GetType(WindowManager.instance.GetWindowTypeName(winType)));
            win.m_winEnumType = winType;           

            UIHelper.instance.CreateWindow(win, handle, isShow, param);
        }
        else
        {           
            m_winList.Remove(winType);
            if (win.IsLoad)
            {
                if (isShow)
                {
                    if (handle != null)
                        handle();
                    win.Show(param);
                }
                else
                    win.Hide();
            }
        }
    }

    public void OnCreateWindow(Window win)
    {
        if (m_winList.Contains(win.m_winEnumType))
        {
            m_winList.Remove(win.m_winEnumType);
        }
    }

    public void OnStopLoad(WindowType wintype)
    {
        if (m_winList.Contains(wintype))
        {
            m_winList.Remove(wintype);
        }
    }
}


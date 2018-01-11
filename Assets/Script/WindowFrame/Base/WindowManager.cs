using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class WindowManager : Singleton<WindowManager>
{
    private Dictionary<WindowType, Window> m_winDic = new Dictionary<WindowType, Window>();
    private List<Window> m_winList = new List<Window>();
    private bool m_isCloseingAll;

    private int currentOrder = 1;

    public List<WindowType> staticWinodw = new List<WindowType>();
    public bool IsCloseingAll
    {
        get
        {
            return m_isCloseingAll;
        }
    }

    public int CurrentOrder
    {
        get
        {
            currentOrder++;
            return currentOrder;
        }        
    }

    public WindowManager()
    {
        staticWinodw = new List<WindowType>();

    }

    public void InsertWindow(WindowType wintype, Window win)
    {
        if (m_winDic.ContainsKey(wintype))
            return;
        m_winDic.Add(wintype, win);
        m_winList.Add(win);
    }

    public void InsertWindow(Window win)
    {
        WindowType winType = (WindowType)Enum.Parse(typeof(WindowType), win.Name);
        if (m_winDic.ContainsKey(winType))
            return;
        m_winDic.Add(winType, win);
        m_winList.Add(win);
    }

    public void RemoveWindow(Window win)
    {
        WindowType winType = (WindowType)Enum.Parse(typeof(WindowType), win.Name);
        if (m_winDic.ContainsKey(winType))
            m_winDic.Remove(winType);
        m_winList.Remove(win);
        win = null;
    }

    public string GetWindowTypeName(WindowType wintype)
    {
        string str;
        if (m_winDic.ContainsKey(wintype))
        {
            str = m_winDic[wintype].Name;
            return str;
        }
        str = wintype.ToString();
        return str;
    }

    public Window GetWindowTypeByName(string name)
    {
        WindowType winType = (WindowType)Enum.Parse(typeof(WindowType), name);
        if (m_winDic.ContainsKey(winType))
            return m_winDic[winType];
        for (int i = 0; i < m_winList.Count; i++)
        {
            if (m_winList[i].Name == name)
                return m_winList[i];
        }
        return null;
    }

    public void OnInit()
    {
        m_isCloseingAll = false;
        m_winDic.Clear();
        m_winList.Clear();
    }

    public void OnUpdate()
    {
        for (int i = 0; i < m_winList.Count; i++)
        {
            if (m_winList[i].IsShow)
                m_winList[i].OnUpdate();
        }
    }

    public void OnLateUpdate()
    {
        for (int i = 0; i < m_winList.Count; i++)
        {
            m_winList[i].OnLateUpdate();
        }
    }

    public void SetWindowBrotherLst(WindowType winType, params WindowType[] winList)
    {
        Window win = GetWindow(winType);
        if (win.brotherWindowLst == null)
            win.brotherWindowLst = new List<WindowType>();
        for (int i = 0; i < winList.Length; i++)
            win.brotherWindowLst.Add(winList[i]);
    }

    public void CloseWindow(WindowType winType)
    {
        if (m_winDic.ContainsKey(winType))
        {
            Window tmpWin = m_winDic[winType];
            m_winDic[winType].Close();
            m_winList.Remove(tmpWin);
            m_winDic.Remove(winType);
            currentOrder--;
        }
    }

    public void ShowWindow(WindowType winType, object[] param = null)
    {
        if (m_winDic.ContainsKey(winType))
        {
            m_winDic[winType].Show();
        }
    }

    public Window GetWindow(WindowType winType)
    {
        if (m_winDic.ContainsKey(winType))
            return m_winDic[winType];
        return null;
    }

    public void HideWindow(WindowType winType)
    {
        if (m_winDic.ContainsKey(winType))
            m_winDic[winType].Hide();
    }

    public void CloseAllWindows()
    {
        m_isCloseingAll = true;
        for (int i = 0; i < m_winList.Count; i++)
        {
            if (m_winList[i].m_wndType == winType.TypeCache)
                m_winList[i].Hide();
            if (m_winList[i].m_wndType == winType.TypeNormal)
            {
                m_winList[i].Close();
                i--;
            }
        }
        m_isCloseingAll = false;
    }

    public void CloseAllWindows(params Window[] win)
    {
        List<Window> winList = new List<Window>(win);

        m_isCloseingAll = true;
        for (int i = 0; i < m_winList.Count; i++)
        {
            if (winList.Contains(m_winList[i]))
                continue;
            if (staticWinodw.Contains(m_winList[i].m_winEnumType))
                continue;
            if (m_winList[i].m_wndType == winType.TypeCache)
                m_winList[i].Hide();
            if (m_winList[i].m_wndType == winType.TypeNormal)
            {
                m_winList[i].Close();
                i--;
            }
        }
        m_isCloseingAll = false;
    }

    public void CloseAllWIndows(params WindowType[] wintype)
    {
        Window[] tmp = new Window[wintype.Length];
        for (int i = 0; i < tmp.Length; i++)
        {
            tmp[i] = GetWindow(wintype[i]);
        }

        if (tmp.Length > 0)
            CloseAllWindows(tmp);
        else
            CloseAllWindows();
    }
}


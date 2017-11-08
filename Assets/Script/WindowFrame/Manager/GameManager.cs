using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[Hotfix]
public class GameManager :MonoBehaviour {
    public List<WindowType> m_PreLoadWindow = new List<WindowType>();
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        #region 预加载窗体
        //m_PreLoadWindow.Add();
        #endregion
    }

    public void Start()
    {
        WindowManager.instance.OnInit();

        for (int i = 0; i < m_PreLoadWindow.Count; i++)
        {

        }
        WindowFactory.instance.CreateWindow(WindowType.LoginWindow);   
    }

    public void Update()
    {
        WindowManager.instance.OnUpdate();
    }

    public void LateUpdate()
    {
        WindowManager.instance.OnLateUpdate();
    }

}

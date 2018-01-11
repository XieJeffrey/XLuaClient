using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using UnityEngine.SceneManagement;

[Hotfix]
public class GameManager :MonoBehaviour {
    public List<WindowType> m_PreLoadWindow = new List<WindowType>();
    private GameObject m_MainCamera;
    private float m_readyToExit;

    private void Awake()
    {
        m_MainCamera = GameObject.Find("GUICamera");   

        DontDestroyOnLoad(gameObject);
        #region 预加载窗体
       // m_PreLoadWindow.Add(WindowType.FloatTipWindow);
        #endregion
    }

    public void Start()
    {
        WindowManager.instance.OnInit();

        for (int i = 0; i < m_PreLoadWindow.Count; i++)
        {
            WindowFactory.instance.CreateWindow(m_PreLoadWindow[i], null, false, null);
        }
        WindowFactory.instance.CreateWindow(WindowType.GameWindow);   
    }

    public void Update()
    {       
        WindowManager.instance.OnUpdate();
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (m_readyToExit > 0f)
            {
                Util.Log("返回键退出");
                m_readyToExit = 0f;
                Application.Quit();                
            }
            else
            {
               // FloatTipWindow.instance.Show("再次点击返回键退出游戏");
                m_readyToExit = 1.0f;
            }            
        }
        if (m_readyToExit > 0f)
        {
            m_readyToExit -= Time.deltaTime;
        }
    }

    public void LateUpdate()
    {
        WindowManager.instance.OnLateUpdate();
    }

    public void FixedUpdate()
    {
        Timer.instance.OnUpdate();
    }
}

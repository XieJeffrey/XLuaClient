using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using System.IO;
using UnityEngine.UI;
using System;

public class Main : MonoBehaviour
{
    #region 更新的进度条界面
    public GameObject UpdateWindow;
    public Slider m_value;
    public Text updateTipText;
    #endregion

    public static ResourceManager ResManager;
    public static AudioManager AudioManager;
    public static NetworkManager NetworkManager;

    public static emptyMono Mono;
    public bool isUpdateFile;
    public static LuaEnv luaenv;

#if ISDEBUG
    private float m_LastUpdateShowTime = 0f;  //上一次更新帧率的时间;  
    private float m_UpdateShowDeltaTime = 0.01f;//更新帧率的时间间隔;  
    private int m_FrameUpdate = 0;//帧数;  
    private float m_FPS = 0;
#endif

    public GameObject SkillTool;

    void Awake()
    {
        luaenv = new LuaEnv();
        gameObject.AddComponent<OutLog>();
        Mono = gameObject.AddComponent<emptyMono>();
        Application.targetFrameRate = AppConst.lockFrame;        
#if ISDEBUG
        //gameObject.AddComponent<TestConsole>();
        m_LastUpdateShowTime = Time.realtimeSinceStartup;
#endif

        #region bugly
        //开启SDK的日志打印,发布版本请务必关闭
#if ISDEBUG
        BuglyAgent.ConfigDebugMode(true);
#else
        BuglyAgent.ConfigDebugMode(false);
#endif
        //注册日志回调,替换使用'Application.RegisterLogCallback(Application.LogCallback)'注册日志回调的方式
        BuglyAgent.RegisterLogCallback(OutLog.HandleLog);

#if UNITY_IPHONE || UNITY_IOS
        BuglyAgent.InitWithAppId ("Your App ID");
#elif UNITY_ANDROID
        BuglyAgent.InitWithAppId("7599ce92e1");
#endif
        //如果你确认已在对应的ios工程或Andoird工程初始化SDK,那么在脚本中只需启动C#异常捕获上报功能即可
        BuglyAgent.EnableExceptionHandler();
        #endregion

        EventManager.instance.RegistEvent(NetEventType.OnSetUpdateWindowTipTxt, SetUpdateTipText);
    }
    
    // Use this for initialization
    void Start()
    {
        if (AppConst.IsUpdate)
        {
            isUpdateFile = true;
            UpdateHelper.CheckAppVersion(OnCheckAppVersion);
        }
        else
        {
#if !UNITY_EDITOR
            if(PlayerPrefs.GetInt("IsCopyRes")==0){
                UpdateWindow.SetActive(true);
                UpdateHelper.CopyResToLocal(OnCopy);            
            }
            else{
                UpdateWindow.SetActive(false);
                GameInit();
            }
#else
            GameInit();
#endif
        }
    }

    public void SetUpdateTipText(object[] objs)
    {
        updateTipText.text = objs[0] as  string;
    }

    void OnCopy()
    {
        GameInit();
    }

    void Update()
    {
        if (Util.IsNet == false && isUpdateFile)
        {
            Main.Mono.StopAllCoroutines();
            //弹断网提示，然后退出
        }
#if ISDEBUG
        m_FrameUpdate++;
        if (Time.realtimeSinceStartup - m_LastUpdateShowTime >= m_UpdateShowDeltaTime)
        {
            m_FPS = m_FrameUpdate / (Time.realtimeSinceStartup - m_LastUpdateShowTime);
            m_FrameUpdate = 0;
            m_LastUpdateShowTime = Time.realtimeSinceStartup;
        }
#endif

        if (UpdateWindow.activeSelf)
        {
            m_value.value = (float)UpdateHelper.copyIndex / UpdateHelper.copyFileCount;
        }

        if (Input.GetKeyDown(KeyCode.T) || Input.touchCount == 5)
        {
            SkillTool.SetActive(!SkillTool.activeSelf);
        }
    }

    private void OnDestroy()
    {
        if(luaenv!=null)
            luaenv.Dispose();
    }


    void GameInit()
    {
        UpdateWindow.SetActive(false);
        isUpdateFile = false;
        ResManager = gameObject.AddComponent<ResourceManager>();
        AudioManager = gameObject.AddComponent<AudioManager>();
        NetworkManager = gameObject.AddComponent<NetworkManager>();
        ResManager.Initialize(Util.AssetDirname, delegate ()
        {
            Util.Log("Initialize OK!!!");
            this.OnInit();
        });
    }

    void OnInit()
    {
#if HOTFIXENABLE
        UpdateHelper.LoadLuaHotFix();
#endif
        LoadConfig();
        NetworkManager.InitLogicManager();
        gameObject.AddComponent<GameManager>();
    }

    void LoadConfig()
    {
#if UNITY_EDITOR
        string path = Application.dataPath + "/GameRes/Config/tbl";
#else
        string path = Application.persistentDataPath + "/tbl";      
#endif
        if (SoundConfigBaseManager.instance.Load(path + "/soundconfig.tbl") == false)
        {
            Util.LogError("soundconfig.tbl加载失败");
        }
        
    }

    /// <summary>
    /// 检查app版本号返回结果
    /// </summary>
    /// <param name="result">true:客户端服务器的app版本号一致，false:版本号不一致，需要更新</param>
    void OnCheckAppVersion(bool result)
    {
        if (result)
        {
            int localVersion = PlayerPrefs.GetInt("AppResVersion", 0);
            Util.Log("----------检查本地资源版本号，确定需不需要复制资源->  local:{0}  apk:{1}", localVersion, UpdateHelper.localVersionInfo.ResVersion);
            if (localVersion == 0 || UpdateHelper.localVersionInfo.ResVersion > localVersion)
            {
                //首次安装 || 安装包的资源高于本地版本资源的，将安装包的资源解压的本地去
                UpdateWindow.SetActive(true);
                UpdateHelper.CopyResToLocal(OnCopyResFinish);
            }
            else
            {
                UpdateHelper.UpdateBundleRes(OnUpdateBundleRes);
            }
        }
        else
        {
            Util.LogError("app版本过低，请更新app");
            Application.Quit();
        }
    }

    void OnCopyResFinish()
    {
        Util.Log("资源解压完毕");
       /// UpdateWindow.SetActive(false);
        UpdateHelper.UpdateBundleRes(OnUpdateBundleRes);
    }


    void OnUpdateBundleRes()
    {
        Util.Log("资源更新完成");
        GameInit();
    }

#if ISDEBUG
    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width - 60, 0, 100, 100), string.Format("{0:0} FPS", m_FPS));
    }
#endif
}

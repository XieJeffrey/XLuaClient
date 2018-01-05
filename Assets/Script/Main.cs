using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using System.IO;

public class Main : MonoBehaviour
{
    public static ResourceManager ResManager;
    public static emptyMono Mono;
    public bool isUpdateFile;
    public static LuaEnv luaenv;
    void Awake()
    {
        luaenv = new LuaEnv();
        gameObject.AddComponent<OutLog>();
        Mono = gameObject.AddComponent<emptyMono>();       
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
            GameInit();
    }

    void Update()
    {
        if (Util.IsNet == false && isUpdateFile)
        {
            Main.Mono.StopAllCoroutines();
            //弹断网提示，然后退出
        }
    }

    private void OnDestroy()
    {
        luaenv.Dispose();
    }


    void GameInit()
    {
        isUpdateFile = false;
        ResManager = gameObject.AddComponent<ResourceManager>();
        ResManager.Initialize(Util.AssetDirname, delegate ()
        {
            Util.Log("Initialize OK!!!");
            this.OnInit();
        });
    }

    void OnInit()
    {
        UpdateHelper.LoadLuaHotFix();
        LoadConfig();
        gameObject.AddComponent<GameManager>();
    }

    void LoadConfig()
    {
#if UNITY_EDITOR
        string path = Application.streamingAssetsPath;
#else
        string path = Application.persistentDataPath;
#endif
        if (testBaseManager.instance.Load(path + "/test.tbl") == false)
        {
            Util.LogError("{0}加载失败", "test.tbl");
            return;
        }
        if (test1BaseManager.instance.Load(path + "/test1.tbl") == false)
        {
            Util.LogError("{0}加载失败", "test1.tbl");
            return;
        }
        Util.Log(testBaseManager.instance.Find("10001").heroName);
        Util.Log(testBaseManager.instance.Find("10002").heroName);
        Util.Log(testBaseManager.instance.Find("10003").heroName);
        Util.Log(testBaseManager.instance.Find("10004").heroName);
        Util.Log(testBaseManager.instance.Find("10005").heroName);

    }   
    
    void OnCheckAppVersion(bool result)
    {
        if (result)
        {
            int localVersion = PlayerPrefs.GetInt("AppResVersion", 0);
            Util.Log("----------检查资源版本号->  local:{0}  server:{1}", localVersion, AppConst.ResVersion);
            if (localVersion == 0 || AppConst.ResVersion > localVersion)
            {
                //首次安装或者安装包的资源高于本地版本资源的，将安装包的资源解压的本地去
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
        UpdateHelper.UpdateBundleRes(OnUpdateBundleRes);
    }

    void OnUpdateBundleRes()
    {
        Util.Log("资源更新完成");
        GameInit();
    }
}

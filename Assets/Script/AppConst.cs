using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppConst
{
#if UNITY_EDITOR
    public const bool IsUpdate = false;
#else
      public const bool IsUpdate = true;
#endif

    public const string AppName = "ClientFrame";
    public const int AppVersion = 1712211443;
    public const string ExtName = ".assetbundle";
    public const string hotFixFileName = "manifest.lua";
#if UNITY_ANDROID || UNITY_EDITOR
#if OUTER
    public const string ServerVersionUrl = "http://121.41.24.200:8888/data/WuKong/Android/Version.txt";
    public const string ResMD5File = "http://121.41.24.200:8888/data/WuKong/Android/files.txt";
    public static string UpdateUrl = "http://121.41.24.200:8888/data/WuKong/Android/data";
#else
    public const string ServerVersionUrl = "http://192.168.5.3:8888/data/WuKong/Android/Version.txt";
    public const string ResMD5File = "http://192.168.5.3:8888/data/WuKong/Android/files.txt";
    public static string UpdateUrl = "http://192.168.5.3:8888/data/WuKong/Android/data";
#endif
#elif UNITY_IPHONE
#if OUTER
    public const string ServerVersionUrl = "http://121.41.24.200:8888/data/WuKong/IOS/Version.txt";
    public const string ResMD5File = "http://121.41.24.200:8888/data/WuKong/IOS/files.txt";
    public static string UpdateUrl = "http://121.41.24.200:8888/data/WuKong/IOS/data";
#else
     public const string ServerVersionUrl = "http://192.168.5.3:8888/data/WuKong/IOS/Version.txt";
      public const string ResMD5File = "http://192.168.5.3:8888/data/WuKong/IOS/files.txt";
    public static string UpdateUrl = "http://192.168.5.3:8888/data/WuKong/IOS/data";
#endif   
#endif

    public const int lockFrame = 30;
    public const float SendMessageTimeOut = 500f;

#if FULIN
    public const string ServerIP = "192.168.5.38";
    public const int ServerPort = 9989;
#elif OFFLINE
    public const string ServerIP = "";
    public const int ServerPort = 0;
#elif OUTER
    public const string ServerIP = "121.40.155.10";
    public const int ServerPort = 7089;
#else
    public const string ServerIP = "192.168.5.4";
    public const int ServerPort = 9989;
#endif

    public const int Language = 1;//中文

}

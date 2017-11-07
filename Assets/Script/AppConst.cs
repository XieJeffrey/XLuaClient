using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppConst
{
    public const bool IsUpdate = false;
    public const string AppName = "ClientFrame";
    public const int ResVersion = 20171023;
    public const int AppVersion = 20171023;//服务器版本
    public const string ExtName = ".assetbundle";

    public const string ResUrl = "";//存放最新资源版本号的地址
    public const string ResMD5File = "";//存放资源MD5文件的地址
    public const string AppUrl = "";//存放最新的App版本号的地址


#if ANDROID || UNITY_EDITOR
    public static string UpdateUrl = "http://localhost/Update/Android/";
#else
    public static string UpdateUrl = "http://localhost/Update/IOS/";
#endif
}

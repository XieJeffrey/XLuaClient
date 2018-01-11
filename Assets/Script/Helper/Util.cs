using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using XLua;
using System;

[Hotfix]
[LuaCallCSharp]
public class Util : Singleton<Util>
{
    public static bool IsNet
    {
        get
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }
    }

    public static bool IsWifi
    {
        get
        {
            return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
        }
    }

    public static string DataPath
    {
        get
        {
            string game = AppConst.AppName.ToLower();

            if (Application.isMobilePlatform)
                return Application.persistentDataPath + "/";
            else
                return Application.streamingAssetsPath + "/";
        }
    }

    public static string AssetDirname
    {
        get
        {
            return "StreamingAssets";
        }
    }

    public static void ClearMemory()
    {

    }

    public static string GetColorString(string content, string color)
    {
        content = string.Format("<color=#{0}>{1}</color>", color, content);
        return content;
    }

    public static void ShowComfirmWindow(string content, Action OkAction = null)
    {
        WindowFactory.instance.CreateWindow(WindowType.ConfirmWindow, delegate
        {
            //ConfirmWindow.instance.ShowConfirmWindow(2, content, OkAction, null);
        });
    }

    public static void ShowConfirmAndCancleWindow(string content, Action OkAction = null, Action CancleAction = null)
    {
        WindowFactory.instance.CreateWindow(WindowType.ConfirmWindow, delegate
        {
           // ConfirmWindow.instance.ShowConfirmWindow(1, content, OkAction, CancleAction);
        });
    }

    public static void Log(string str, params object[] msg)
    {
#if ISDEBUG
        Debug.LogFormat(str, msg);
#endif
    }

    public static void LogForMaple(string str, params object[] msg)
    {
#if ISDEBUG
        Debug.LogFormat(GetColorString(str, "00Ff00"), msg);
#endif
    }

    public static void LogForSkill(string str, params object[] msg)
    {
#if ISDEBUG
        Debug.LogFormat(GetColorString(str, "FFFF00"), msg);
#endif
    }

    public static void LogWarnning(string str, params object[] msg)
    {
#if ISDEBUG
        Debug.LogWarningFormat(str, msg);
#endif
    }

    public static void LogError(string str, params object[] msg)
    {
#if ISDEBUG
        Debug.LogErrorFormat(str, msg);
#endif
    }

    public static void LogForNet(string str, params object[] msg)
    {
#if ISDEBUG
        Debug.LogFormat(GetColorString(str, "557788"), msg);
#endif
    }

    public static string MD5file(string file)
    {
        try
        {
            FileStream fs = new FileStream(file, FileMode.Open);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] resVal = md5.ComputeHash(fs);
            fs.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < resVal.Length; i++)
            {
                sb.Append(resVal[i].ToString("X2"));
            }
            return sb.ToString();
        }
        catch (System.Exception ex)
        {
            throw new System.Exception("MD5file() file,error:{0}" + ex.StackTrace);
        }
    }

    public static string Upper2LowerAnd_(string ostr)
    {
        List<char> c = new List<char>(ostr.ToCharArray());
        for (int i = c.Count - 1; i > 0; i--)
        {
            if (c[i] >= 65 && c[i] <= 90)
            {
                c.Insert(i, '_');
            }
        }
        ostr = new string(c.ToArray());
        return ostr.ToLower();
    }

    public static string GetRelativePath()
    {
        if (Application.isMobilePlatform || Application.isConsolePlatform)
            return "file:///" + DataPath;
        else if (Application.isEditor)
            return "file://" + DataPath;
        else
            return "file:///" + DataPath;
    }

    public static string GetContent(string ID)
    {
        LanguageBase tmp = LanguageBaseManager.instance.Find(ID);
        if (tmp == null)
        {
            Util.LogError("Can't find ID:{0} in Language.csv", ID);
            return "";
        }
        if (AppConst.Language == 1)
        {
            string str = tmp.chinese.Replace("/n", "\n\r");
            return str;
        }

        if (AppConst.Language == 2)
        {
            string str = tmp.english.Replace("/n", "\n\r");
            return str;
        }
    }  

}

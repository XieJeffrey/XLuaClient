using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;


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
                return Application.persistentDataPath + "/" + game + "/";
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

    public static void Vibrate()
    {
        Handheld.Vibrate();
    }

    public static void ClearMemory()
    {

    }

    public static void Log(string str, params object[] msg)
    {
#if DEBUG
        Debug.LogFormat(str, msg);
#endif
    }

    public static void LogWarnning(string str, params object[] msg)
    {
#if DEBUG
        Debug.LogWarningFormat(str, msg);
#endif
    }

    public static void LogError(string str, params object[] msg)
    {
#if DEBUG
        Debug.LogErrorFormat(str, msg);
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

}

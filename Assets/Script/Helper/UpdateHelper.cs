using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using LitJson;

public class UpdateHelper
{
    public static int copyIndex = 0;
    public static int copyFileCount = 0;

    public static VersionInfo localVersionInfo;
    public static VersionInfo ServerVersionInfo;

    /// <summary>
    /// 读取本地app版本信息跟资源版本信息
    /// </summary>
    /// <param name="callBack"></param>
    /// <returns></returns>
    public static IEnumerator ReadLocalVersionInfo(Action<bool> callBack)
    {
        Util.LogForSkill("Start:读取本地版本文件");
        string path = Application.streamingAssetsPath + "/version.txt";
#if UNITY_EDITOR
        WWW www = new WWW("file:/" + path);
#else
         WWW www = new WWW(path);
#endif
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            if (www.isDone)
            {
                //byte[] bytes = new byte[www.bytes.Length - 3];
                //for (int i = 3; i < www.bytes.Length; i++)
                //{
                //    bytes[i - 3] = www.bytes[i];
                //}
                string str = System.Text.Encoding.UTF8.GetString(www.bytes);                    
                localVersionInfo = JsonMapper.ToObject<VersionInfo>(str);
            }
        }
        Util.LogForSkill("End:读取本地版本文件......appversion:{0},resVersion:{1}", localVersionInfo.AppVersion, localVersionInfo.ResVersion);
        Main.Mono.StartCoroutine(DoCheckAppVersion(callBack));
    }

    public static void CheckAppVersion(Action<bool> callBack)
    {
        Main.Mono.StartCoroutine(ReadLocalVersionInfo(callBack));
    }

    /// <summary>
    /// 读取服务器app版本信息跟资源版本信息并比对app版本信息
    /// </summary>
    /// <param name="callBack"></param>
    /// <returns></returns>
    public static IEnumerator DoCheckAppVersion(Action<bool> callBack)
    {
        Util.LogForSkill("Start:读取服务器版本文件:{0}",AppConst.ServerVersionUrl);
        WWW www = new WWW(AppConst.ServerVersionUrl);       
        yield return www;
        if (www.isDone)
        {
            if (string.IsNullOrEmpty(www.error))
            {
                //byte[] bytes =new byte[www.bytes.Length-3] ;
                //for (int i = 3; i < www.bytes.Length; i++)
                //{
                //    bytes[i - 3] = www.bytes[i];
                //}         
                string str = System.Text.Encoding.UTF8.GetString(www.bytes);               
                ServerVersionInfo = JsonMapper.ToObject<VersionInfo>(str);
                Util.LogForSkill("End:获取服务器版本文件......appversion:{0},resVersion:{1}", ServerVersionInfo.AppVersion, ServerVersionInfo.ResVersion);
                callBack(localVersionInfo.AppVersion >= ServerVersionInfo.AppVersion);               
            }
            else
            {
                Util.LogError(www.error);
            }
        }
        else
        {
            Util.LogError(www.error);
        }
    }

    public static void CopyResToLocal(Action callBack)
    {
        Main.Mono.StartCoroutine(DoCopyResToLocal(callBack));
    }

    public static IEnumerator DoCopyResToLocal(Action callBack)
    {
        string path = Application.streamingAssetsPath + "/files.txt";
        List<string> fileList = new List<string>();

#if UNITY_EDITOR
        WWW www = new WWW("file:/" + path);
#else
            WWW www = new WWW(path);
#endif
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            if (www.isDone)
            {
                byte[] bytes = www.bytes;
                string str = System.Text.Encoding.UTF8.GetString(bytes);
                string[] md5List = System.Text.Encoding.UTF8.GetString(bytes).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < md5List.Length; i++)
                {
                    string[] tmp = md5List[i].Split('|');
                    fileList.Add(tmp[0].Trim());
                }
            }
        }

        WaitForEndOfFrame weof = new WaitForEndOfFrame();

        List<string> dirList = new List<string>();
        copyFileCount = fileList.Count;
        for (int i = 0; i < fileList.Count; i++)
        {
            int lastIndex = fileList[i].LastIndexOf("/");
            if (lastIndex == -1) continue;
            string dirName = fileList[i].Substring(0, lastIndex);
            if (dirList.IndexOf(dirName) != -1) continue;
            dirList.Add(dirName);
        }
        for (int i = 0; i < dirList.Count; ++i)
        {
            if (File.Exists(Application.persistentDataPath + "/" + dirList[i]) == false)
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/" + dirList[i]);
            }
        }

        for (int i = 0; i < fileList.Count; i++)
        {
            int lastIndex = fileList[i].LastIndexOf("/") + 1;
            string fileName = fileList[i].Substring(lastIndex, fileList[i].Length - lastIndex);

            EventManager.instance.NotifyEvent(NetEventType.OnSetUpdateWindowTipTxt, string.Format("-----文件复制中:{0}",fileName));
            Util.Log("-----文件复制中:{0},{1}", Application.streamingAssetsPath + "/" + fileName, Util.DataPath + fileList[i]);
#if UNITY_ANDROID && !UNITY_EDITOR
            www = new WWW(Application.streamingAssetsPath + "/" + fileList[i]);
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                if (www.isDone)
                {
                    byte[] bytes = www.bytes;
                    File.WriteAllBytes(Util.DataPath + "/" + fileList[i], bytes);
                     copyIndex = i + 1;   
                }
            }
            else
            {
                Util.LogError(www.error);
            }
#else
            File.Copy(Application.streamingAssetsPath + "/" + fileList[i], Application. persistentDataPath + "/" + fileList[i], true);
            copyIndex = i + 1;
#endif
            yield return weof;
        }
        PlayerPrefs.SetInt("IsCopyRes", 1);
        PlayerPrefs.SetInt("AppResVersion", localVersionInfo.ResVersion);
        callBack();
    }

    public static void UpdateBundleRes(Action callBack)
    {
        Main.Mono.StartCoroutine(DoUpdateBundleRes(callBack));
    }

    public static IEnumerator DoUpdateBundleRes(Action callBack)
    {
        Util.Log("------服务器资源对比更新中,local:{0},server:{1}",localVersionInfo.ResVersion,ServerVersionInfo.ResVersion);

        if (localVersionInfo.ResVersion >= ServerVersionInfo.ResVersion)
        {
            Util.Log("本地资源很新了，不需要热更");
            callBack();
            yield break;
        }


        Dictionary<string, string> md5Dic = new Dictionary<string, string>();
        Dictionary<string, string> newMd5Dic = new Dictionary<string, string>();
        byte[] serverMD5bytes = new byte[0];

        string path = Util.DataPath + "/files.txt";
        if (File.Exists(path))
        {
            #region 获取本地资源MD5文件

#if UNITY_EDITOR
            WWW www = new WWW("file:/" + path);
#else
		    WWW www = new WWW(path);
#endif
            string[] serverMD5File = new string[] { };
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                if (www.isDone)
                {
                    byte[] bytes = www.bytes;
                    string str = System.Text.Encoding.UTF8.GetString(bytes);
                    string[] md5List = System.Text.Encoding.UTF8.GetString(bytes).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < md5List.Length; i++)
                    {
                        string[] tmp = md5List[i].Split('|');
                        tmp[1] = tmp[1].Replace("\n", "");
                        md5Dic.Add(tmp[0], tmp[1].Replace("\r", "").Trim());
                    }
                }
            }
            else
            {
                Util.LogError(www.error);
                yield break;
            }
            #endregion

            #region 获取服务器资源MD5文件

            www = new WWW(AppConst.ResMD5File);
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                if (www.isDone)
                {
                    serverMD5bytes = www.bytes;
                    string tmpStr = System.Text.Encoding.UTF8.GetString(serverMD5bytes);
                    serverMD5File = tmpStr.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < serverMD5File.Length; i++)
                    {
                        string[] tmp = serverMD5File[i].Split('|');
                        if (tmp.Length < 2)
                            continue;
                        tmp[1] = tmp[1].Replace("\n", "");
                        Util.LogForMaple(tmp[0]);
                        newMd5Dic.Add(tmp[0], tmp[1].Replace("\r", "").Trim());
                    }
                }
            }
            #endregion

            #region 文件比对并更新
            List<string> updateFileList = new List<string>();
            foreach (var tmp in newMd5Dic)
            {
                if (md5Dic.ContainsKey(tmp.Key))
                {
                    if (md5Dic[tmp.Key].CompareTo(tmp.Value) != 0)
                    {
                        updateFileList.Add(tmp.Key);
                    }
                }
                else
                    updateFileList.Add(tmp.Key);
            }

            for (int i = 0; i < updateFileList.Count; i++)
            {
                string filePath = Util.DataPath + "/" + updateFileList[i];
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                www = new WWW(AppConst.UpdateUrl + "/" + updateFileList[i]);
                yield return www;
                if (string.IsNullOrEmpty(www.error))
                {
                    if (www.isDone)
                    {
                        File.WriteAllBytes(filePath, www.bytes);
                        EventManager.instance.NotifyEvent(NetEventType.OnSetUpdateWindowTipTxt, string.Format("资源下载中{0}", filePath));
                        Util.Log("{0} update finish....", filePath);
                    }
                }
            }

            
            File.WriteAllLines(Util.DataPath + "/files.txt", serverMD5File);
            callBack();
            #endregion

            #region 重新生成本地MD5文件
            if (serverMD5bytes.Length > 0)
            {
                File.Delete(path);
                File.WriteAllBytes(path, serverMD5bytes);
            }
            #endregion

            #region 更新本地资源版本号
            PlayerPrefs.SetInt("AppResVersion", ServerVersionInfo.ResVersion);
            #endregion

        }
    }

    private static byte[] CostomLoader(ref string fileName)
    {

#if UNITY_EDITOR
        fileName = Application.dataPath + "/Script/HotfixLua/" + fileName + ".lua";
#else
        fileName = Util.DataPath + "/" + fileName + ".lua";
#endif
        Util.Log(Util.GetColorString(fileName, "FFFF00"));
        if (File.Exists(fileName))
            return File.ReadAllBytes(fileName);
        else
            return null;

    }

    public static void LoadLuaHotFix()
    {
        string filePath = Util.DataPath + "/" + AppConst.hotFixFileName;
        Main.luaenv.AddLoader(CostomLoader);

        Main.luaenv.DoString(@"require 'util'");

        try
        {
            if (File.Exists(filePath))
            {
                string[] hotFixStr = File.ReadAllLines(filePath);
                for (int i = 0; i < hotFixStr.Length; i++)
                {
                    if (string.IsNullOrEmpty(hotFixStr[i]))
                        continue;
                    string[] tmp = hotFixStr[i].Split('|');
                    string csName = tmp[0];
                    string funName = tmp[1];
                    string luaFileName = tmp[2];
                    string luaCode = "";
#if UNITY_EDITOR
                    if (File.Exists(Application.dataPath + "/Script/HotfixLua/" + luaFileName) == false)
                        continue;
                    luaCode = File.ReadAllText(Application.dataPath + "/Script/HotfixLua/" + luaFileName);
#else
                    if (File.Exists(Util.DataPath+"/"+ luaFileName) == false)
                        continue;
                    luaCode = File.ReadAllText(Util.DataPath + "/" + luaFileName);
#endif

                    Main.luaenv.DoString(string.Format(@"xlua.hotfix({0},'{1}',{2})", csName, funName, luaCode), luaFileName);
                }
            }
        }
        catch (System.Exception e)
        {
            Util.LogError(e.Message);
            Util.LogError(e.StackTrace);
        }

    }


}

public class VersionInfo
{
    public int AppVersion = 0;
    public int ResVersion = 0;

    public VersionInfo(int param1, int param2)
    {
        AppVersion = param1;
        ResVersion = param2;
    }

    public VersionInfo()
    {
        AppVersion = 0;
        ResVersion = 0;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;

public class UpdateHelper
{
    private static int copyIndex = 0;
    private static int copyFileCount = 0;

    public static void CopyResToPersistenDataPath(Action callBack)
    {
        copyIndex = 0;
        copyFileCount = Directory.GetFiles(Util.AssetDirname).Length;
        if (copyFileCount == 0)
        {
            Util.Log("StreamAsset找不到资源");
            Application.Quit();
            return;
        }
    }

    public static void CheckAppVersion(Action<bool> callBack)
    {
        DoCheckAppVersion(callBack);
    }

    public static IEnumerator DoCheckAppVersion(Action<bool> callBack)
    {
        WWW www = new WWW(AppConst.AppUrl);
        yield return www;
        if (www.isDone)
        {
            if (string.IsNullOrEmpty(www.error))
            {
                byte[] bytes = www.bytes;
                string str = System.Text.Encoding.UTF8.GetString(bytes);
                Util.Log("----------检查app版本号->  local:{0}  server:{1}", AppConst.AppVersion, int.Parse(str));
                callBack(AppConst.AppVersion >= int.Parse(str));
            }
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
        if (File.Exists(path))
        {

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
                        fileList.Add(tmp[0].Trim().ToLower());
                    }
                }
            }
        }
        else
        {
            Util.LogError("Apk资源包中不存在版本文件!");
            yield break;
        }
        WaitForEndOfFrame weof = new WaitForEndOfFrame();

        for (int i = 0; i < fileList.Count; i++)
        {
            int lastIndex = fileList[i].LastIndexOf("/") + 1;
            string fileName = fileList[i].Substring(lastIndex, fileList[i].Length - lastIndex);
            try
            {
                Util.Log("-----文件复制中:{0}", fileName);
                File.Copy(fileList[i], Util.DataPath, true);
            }
            catch (System.Exception ex)
            {
                Util.LogError(ex.StackTrace);
            }
            yield return weof;
        }
        PlayerPrefs.SetInt("ResVersion", AppConst.ResVersion);
        callBack();
    }

    public static void UpdateBundleRes(Action callBack)
    {
        Main.Mono.StartCoroutine(DoUpdateBundleRes(callBack));
    }

    public static IEnumerator DoUpdateBundleRes(Action callBack)
    {
        Util.Log("------资源对比更新中----");
        Dictionary<string, string> md5Dic = new Dictionary<string, string>();
        Dictionary<string, string> newMd5Dic = new Dictionary<string, string>();
        byte[] serverMD5bytes=new byte[0];

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
                    string tmpStr = System.Text.Encoding.UTF8.GetString(www.bytes);
                    serverMD5File = tmpStr.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < serverMD5File.Length; i++)
                    {
                        string[] tmp = serverMD5File[i].Split('|');
                        tmp[1] = tmp[1].Replace("\n", "");
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
                        Util.Log("{0} update finish....", filePath);
                    }
                }
            }

            File.WriteAllLines(Util.DataPath + "/version.txt", serverMD5File);
            callBack();
            #endregion

            #region 重新生成本地MD5文件
            if (serverMD5bytes.Length > 0)
            {
                File.Delete(path);
                File.WriteAllBytes(path, serverMD5bytes);
            }
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
        Util.Log(Util.GetColorString(fileName,"FFFF00"));
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

using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using LitJson;
using System.Linq;

public class Packager
{
    public static string platform = string.Empty;
    static List<string> paths = new List<string>();
    static List<string> files = new List<string>();

    /// <summary>
    /// 载入素材
    /// </summary>
    static UnityEngine.Object LoadAsset(string file)
    {
        if (file.EndsWith(".lua")) file += ".txt";
        return AssetDatabase.LoadMainAssetAtPath("Assets/Builds/" + file);
    }

    //[MenuItem("Game/Build iPhone Resource", false, 11)]
    //public static void BuildiPhoneResource()
    //{
    //    BuildTarget target;
    //    target = BuildTarget.iOS;
    //    BuildAssetResource(target, false);
    //}

    [MenuItem("Game/Build Android Resource", false, 12)]
    public static void BuildAndroidResource()
    {
        BuildAssetResource(BuildTarget.Android, true);
        CopyConfig();
        WriteVersion();
    }

    //[MenuItem("Game/Build Windows Resource", false, 13)]
    //public static void BuildWindowsResource()
    //{
    //    BuildAssetResource(BuildTarget.StandaloneWindows, true);
    //}

    [MenuItem("Game/Packer HotFixLua Resource", false, 15)]
    public static void PackerHotFixLua()
    {
        #region 复制热更新的Lua代码到StreamAsset
        string srcPath = Application.dataPath + "/Script/HotfixLua";
        string desPath = Application.streamingAssetsPath;
        string[] files = Directory.GetFiles(srcPath);
        Dictionary<string, string> md5File = new Dictionary<string, string>();
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Contains(".meta"))
                continue;
            string fileName = files[i].Replace(srcPath + "\\", string.Empty);
            md5File.Add(fileName, Util.MD5file(files[i]));
            File.Copy(files[i], desPath + "/" + fileName, true);
        }
        #endregion

        #region 生成MD5File 
        string filesPath = Application.streamingAssetsPath + "/files.txt";
        if (File.Exists(filesPath))
        {
            FileStream fs = new FileStream(filesPath, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            foreach (var tmp in md5File)
            {
                sw.WriteLine(tmp.Key + "|" + tmp.Value);
            }
            sw.Close();
            sw.Dispose();
            fs.Close();
            fs.Dispose();
        }

        AssetDatabase.Refresh();

        #endregion
    }

    [MenuItem("Game/Generate And Copy Config", false, 14)]
    public static void CopyConfig()
    {
        #region 生成配置表
        string protoDirectory = Application.dataPath + "/GameRes/Config/proto";
        if (Directory.Exists(protoDirectory))
        {
            string[] protoFileInfo = Directory.GetFiles(protoDirectory);
            for (int i = 0; i < protoFileInfo.Length; i++)
            {
                if (protoFileInfo[i].Contains("meta"))
                    continue;
                StreamReader sr = new StreamReader(protoFileInfo[i],Encoding.UTF8);
                string str = "";
                string jsonStr = "";
                while ((str = sr.ReadLine()) != null)
                {
                    jsonStr += str;                   
                }
               
                try
                {
                    JsonData jsonData = JsonMapper.ToObject(jsonStr);
                    string version = System.DateTime.Now.ToFileTimeUtc().ToString();
                    string fileName = protoFileInfo[i].Split('\\')[protoFileInfo[i].Split('\\').Length - 1];
                    WriteCss(jsonData, fileName, version);
                    WriteTbl(jsonData, fileName, version);
                }
                catch (System.Exception e)
                {
                    Util.LogError(e.StackTrace);
                }
            }
        }
        #endregion

        #region 复制配置表
        string resPath = Application.dataPath + "/GameRes/Config/tbl";
        string desPath = Application.streamingAssetsPath + "/tbl";
        if (!File.Exists(desPath))
        {
            Directory.CreateDirectory(desPath);
        }
        string[] fileInfos = Directory.GetFiles(resPath);
        Dictionary<string, string> md5File = new Dictionary<string, string>();
        for (int i = 0; i < fileInfos.Length; i++)
        {
            if (fileInfos[i].Contains("meta"))
                continue;
            string key = fileInfos[i].Replace(resPath + "\\", string.Empty);
            File.Copy(fileInfos[i], desPath + "/" + key, true);
            md5File.Add(key, Util.MD5file(desPath + "/" + key));
        }
        #endregion
        string filesPath = Application.streamingAssetsPath + "/files.txt";
        if (File.Exists(filesPath))
        {
            FileStream fs = new FileStream(filesPath, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs,Encoding.UTF8);
            foreach (var tmp in md5File)
            {
                sw.WriteLine("tbl/" + tmp.Key + "|" + tmp.Value);
            }
            sw.Close();
            sw.Dispose();
            fs.Close();
            fs.Dispose();
        }
        AssetDatabase.Refresh();
    }

    public static void WriteCss(JsonData jsonData, string fileName, string version)
    {
        #region
        string cssContext = "using System;";
        cssContext += "\rusing System.Collections;";
        cssContext += "\rusing System.Collections.Generic;";
        cssContext += "\r\rpublic class " + fileName.Split('.')[0] + "Base{";
        string cssName = fileName.Split('.')[0] + "base.cs";

        JsonData memberData = jsonData["member"];
        string className = fileName.Split('.')[0] + "Base";
        for (int i = 0; i < memberData.Count; i++)
        {
            cssContext += "\r\tpublic readonly " + memberData[i]["titleType"] + " " + memberData[i]["memberName"] + ";";
        }
        cssContext += "\r}";
        cssContext += "\r";
        cssContext += "\rpublic class " + fileName.Split('.')[0] + "BaseManager" + ": Singleton<" + fileName.Split('.')[0] + "BaseManager" + ">";
        cssContext += "\r{";
        cssContext += string.Format("\r\tprivate Dictionary<string, {0}> m_dataList = new Dictionary<string, {1}>();", className, className);
        cssContext += "\r\tprivate readonly long version=" + version + ";";
        cssContext += "\r\tpublic int Size";
        cssContext += "\r\t{";
        cssContext += "\r\t\tget { return m_dataList.Count; }";
        cssContext += "\r\t}";
        cssContext += "\r\tpublic " + className + "  Get(int index)";
        cssContext += "\r\t{";
        cssContext += "\r\t\tif (index > -1 && index < m_dataList.Count)";
        cssContext += "\r\t\t{";
        cssContext += "\r\t\t\tint i = 0;";
        cssContext += "\r\t\t\t foreach (var tmp in m_dataList.Values)";
        cssContext += "\r\t\t\t {";
        cssContext += "\r\t\t\t\t if (index == i)";
        cssContext += "\r\t\t\t\t {";
        cssContext += "\r\t\t\t\t\treturn tmp;";
        cssContext += "\r\t\t\t\t }";
        cssContext += "\r\t\t\t\t i++;";
        cssContext += "\r\t\t\t }";
        cssContext += "\r\t\t}";
        cssContext += "\r\t\t  return null;";
        cssContext += "\r\t}";
        cssContext += "\r\tpublic " + className + " Find(int key1,int key2=-1,int key3=-1)";
        cssContext += "\r\t{";
        cssContext += "\r\t\t string key = key1.ToString();";
        cssContext += "\r\t\t if (key2 != -1) { key += key2.ToString(); }";
        cssContext += "\r\t\t if (key3 != -1) { key += key3.ToString(); }";
        cssContext += "\r\t\t if (m_dataList.ContainsKey(key))";
        cssContext += "\r\t\t {";
        cssContext += "\r\t\t\treturn m_dataList[key];";
        cssContext += "\r\t\t }";
        cssContext += "\r\t\t return null;";
        cssContext += "\r\t}";

        cssContext += "\r\tpublic " + className + " Find(string key1, string key2 =\"\", string key3 = \"\")";
        cssContext += "\r\t{";
        cssContext += "\r\t\t string key = key1.ToString();";
        cssContext += "\r\t\t if (key2 != \"\") { key += key2.ToString(); }";
        cssContext += "\r\t\t if (key3 != \"\") { key += key3.ToString(); }";
        cssContext += "\r\t\t if (m_dataList.ContainsKey(key))";
        cssContext += "\r\t\t {";
        cssContext += "\r\t\t\treturn m_dataList[key];";
        cssContext += "\r\t\t }";
        cssContext += "\r\t\t return null;";
        cssContext += "\r\t}";

        cssContext += "\r\t public bool Load(string path)";
        cssContext += "\r\t {";
        cssContext += "\r\t\t return TableUtility.instance.Load<" + className + ">(path,ref m_dataList,version.ToString());";
        cssContext += "\r\t}  ";
        cssContext += "\r}";
        #endregion
        string cssPath = Application.dataPath + "/Script/Config/";
        if (File.Exists(cssPath + cssName))
        {
            File.Delete(cssPath + cssName);
        }

        FileStream fs = new FileStream(cssPath + cssName, FileMode.OpenOrCreate);
        StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
        sw.WriteLine(cssContext);
        sw.Flush();
        sw.Close();
        fs.Close();
    }

    public static void WriteTbl(JsonData jsonData, string filename, string version)
    {
        JsonData memberData = jsonData["member"];
        List<byte> byteData = new List<byte>();
        string typeString = "";
        string memberString = "";
        List<string> keyString = jsonData["key"].ToString().Split('+').ToList<string>();
        string splitChar = jsonData["dataSplitChar"].ToString();       
        for (int i = 0; i < memberData.Count; i++)
        {
            typeString += memberData[i]["titleType"].ToString() + "|";
            memberString += memberData[i]["memberName"].ToString() + "|";
        }
        typeString = typeString.Substring(0, typeString.Length - 1);
        memberString = memberString.Substring(0, memberString.Length - 1);
        byteData = (ByteUtility.Write(version, byteData.ToArray<byte>()).ToList<byte>());
        byteData = (ByteUtility.Write(splitChar, byteData.ToArray<byte>()).ToList<byte>());
        byteData = (ByteUtility.Write(typeString, byteData.ToArray<byte>()).ToList<byte>());
        byteData = (ByteUtility.Write(memberString, byteData.ToArray<byte>()).ToList<byte>());

        string xlsPath = Application.dataPath + "/GameRes/Config/xls/" + filename.Split('.')[0] + ".csv";
        string filePath = Application.dataPath + "/GameRes/Config/tbl/" + filename.Split('.')[0].ToLower() + ".tbl";

        

        //FileStream fs2 = new FileStream(xlsPath, FileMode.Open, FileAccess.Read, FileShare.None);
        //StreamReader sr2 = new StreamReader(fs2, System.Text.Encoding.ASCII);
        //string copyStr = "";
       
        //List<string> utfFileStr = new List<string>();
        //while ((copyStr = sr2.ReadLine()) != null)
        //{
        //    utfFileStr.Add(copyStr);
        //}
        //sr2.Close();
        //fs2.Dispose();

        //File.Delete(xlsPath);

        //FileStream fs1 = new FileStream(xlsPath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None);
        //StreamWriter wr1 = new StreamWriter(fs1, System.Text.Encoding.UTF8);
        //for (int i = 0; i < utfFileStr.Count; i++)
        //{
        //    wr1.WriteLine(utfFileStr[i]);
        //}
        //wr1.Flush();
        //wr1.Close();
        //fs1.Dispose();

        FileStream fs = new FileStream(xlsPath, FileMode.Open, FileAccess.Read, FileShare.None);
        StreamReader sr = new StreamReader(fs, System.Text.Encoding.GetEncoding("GB2312"));
        string str = "";
        int index = 0;
        byte[] outByteData = new byte[0];
        while ((str = sr.ReadLine()) != null)
        {
            if (xlsPath.Contains("Upgrade"))
            {
                Util.LogForSkill(str);
            }
            if (index != 0)
            {
                string[] paramArray = str.Split(',');
                byte[] keyByteData = new byte[0];
                byte[] propetyByteData = new byte[0];
                string key = "";
                for (int j = 0; j < paramArray.Length; j++)
                {
                    if (keyString.Contains(memberData[j]["titleName"].ToString()))
                    {
                        key += paramArray[j];
                    }
                    //Util.LogForSkill(memberData[j]["titleType"].ToString());
                    //Util.LogForSkill(paramArray[j]);
                    switch (memberData[j]["titleType"].ToString())
                    {
                        //case "uint":
                        //    propetyByteData = ByteUtility.Write(uint.Parse(paramArray[j]), propetyByteData);
                        //    break;
                        case "short":
                            propetyByteData = ByteUtility.Write(short.Parse(paramArray[j]), propetyByteData);
                            break;
                        case "string":
                            propetyByteData = ByteUtility.Write(paramArray[j], propetyByteData);
                            break;
                        case "float":
                            propetyByteData = ByteUtility.Write(float.Parse(paramArray[j]), propetyByteData);
                            break;
                        case "int":
                            propetyByteData = ByteUtility.Write(int.Parse(paramArray[j]), propetyByteData);
                            break;
                        case "List<string>":
                            propetyByteData = ByteUtility.WriteListString(paramArray[j], propetyByteData);
                            break;
                        case "List<int>":
                            propetyByteData = ByteUtility.WriteListInt(paramArray[j], propetyByteData);
                            break;
                    }
                }
                keyByteData = ByteUtility.Write(key, keyByteData);              
                byte[] tmp = outByteData;
                outByteData = new byte[keyByteData.Length + tmp.Length + propetyByteData.Length];
                keyByteData.CopyTo(outByteData, 0);
                propetyByteData.CopyTo(outByteData, keyByteData.Length);
                tmp.CopyTo(outByteData, keyByteData.Length + propetyByteData.Length);
            }
            index++;
        }
        fs.Close();
        sr.Close();
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        byteData.AddRange(outByteData.ToList<byte>());
        byte[] byteoutputData = byteData.ToArray<byte>();
        File.WriteAllBytes(filePath, byteoutputData);
        Util.Log("{0} is generated success", filePath);
    }

    /// <summary>
    /// 生成绑定素材
    /// </summary>
    public static void BuildAssetResource(BuildTarget target, bool isWin)
    {
        string dataPath = Application.streamingAssetsPath;
        if (Directory.Exists(dataPath))
        {
            Directory.Delete(dataPath, true);
        }
        string assetfile = string.Empty;  //素材文件名
        string resPath = AppDataPath + "/" + Util.AssetDirname + "/";
        if (!Directory.Exists(resPath)) Directory.CreateDirectory(resPath);
        BuildPipeline.BuildAssetBundles(resPath, BuildAssetBundleOptions.DeterministicAssetBundle | BuildAssetBundleOptions.ChunkBasedCompression, target);

        ///----------------------创建文件列表-----------------------
        string newFilePath = resPath + "/files.txt";
        if (File.Exists(newFilePath)) File.Delete(newFilePath);

        paths.Clear(); files.Clear();
        Recursive(resPath);


        #region 写入bundle的MD5
        FileStream fs = new FileStream(newFilePath, FileMode.CreateNew);
        StreamWriter sw = new StreamWriter(fs);
        for (int i = 0; i < files.Count; i++)
        {
            string file = files[i];
            string ext = Path.GetExtension(file);
            if (file.EndsWith(".meta") || file.Contains(".DS_Store") || file.Contains(".svn")) continue;

            string md5 = Util.MD5file(file);
            string value = file.Replace(resPath, string.Empty);
            sw.WriteLine(value + "|" + md5);
        }
        #endregion

        #region 写入配置表的MD5
        //string configPath = Application.dataPath + "/GameRes/Config/tbl";
        //string[] fileInfos = Directory.GetFiles(configPath);
        //for (int i = 0; i < fileInfos.Length; i++)
        //{
        //    if (fileInfos[i].Contains("meta"))
        //        continue;
        //    string md5 = Util.MD5file(fileInfos[i]);
        //    string key = fileInfos[i].Replace(configPath+"\\",string.Empty);
        //    sw.WriteLine(key + "|" + md5);
        //}

        #endregion
        sw.Close();
        fs.Close();


        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 数据目录
    /// </summary>
    static string AppDataPath
    {
        get { return Application.dataPath.ToLower(); }
    }

    /// <summary>
    /// 遍历目录及其子目录
    /// </summary>
    static void Recursive(string path)
    {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);
        foreach (string filename in names)
        {
            string ext = Path.GetExtension(filename);
            if (ext.Equals(".meta")) continue;
            files.Add(filename.Replace('\\', '/'));
        }
        foreach (string dir in dirs)
        {
            paths.Add(dir.Replace('\\', '/'));
            Recursive(dir);
        }
    }

    static void UpdateProgress(int progress, int progressMax, string desc)
    {
        string title = "Processing...[" + progress + " - " + progressMax + "]";
        float value = (float)progress / (float)progressMax;
        EditorUtility.DisplayProgressBar(title, desc, value);
    }

    /// <summary>
    /// 目录复制
    /// </summary>
    /// <param name="direcSource">源目录</param>
    /// <param name="direcTarget">目标目录</param>
    static void CopyFolder(string direcSource, string direcTarget)
    {
        if (!Directory.Exists(direcTarget))
            Directory.CreateDirectory(direcTarget);

        DirectoryInfo direcInfo = new DirectoryInfo(direcSource);
        FileInfo[] files = direcInfo.GetFiles();
        foreach (FileInfo file in files)
            file.CopyTo(Path.Combine(direcTarget, file.Name), true);

        DirectoryInfo[] direcInfoArr = direcInfo.GetDirectories();
        foreach (DirectoryInfo dir in direcInfoArr)
            CopyFolder(Path.Combine(direcSource, dir.Name), Path.Combine(direcTarget, dir.Name));

    }


    [MenuItem("Game/WriteVersion", false, 18)]
    static void WriteVersion()
    {
        string path= Application.streamingAssetsPath + "/version.txt";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        System.DateTime curTime = System.DateTime.Now;
        string year = (curTime.Year % 100).ToString();
        string month = curTime.Month.ToString().PadLeft(2,'0');
        string day = curTime.Day.ToString().PadLeft(2, '0');
        string hour = curTime.Hour.ToString().PadLeft(2, '0');
        string sec = curTime.Minute.ToString().PadLeft(2, '0');
        string resVersion = year + month + day + hour + sec;

        int appVersion = AppConst.AppVersion;

        VersionInfo m_versionInfo = new VersionInfo( appVersion, int.Parse(resVersion));
        string json = LitJson.JsonMapper.ToJson(m_versionInfo);

       // File.CreateText(path);
        
        File.WriteAllText(path, json,Encoding.Default);       
        
    }
}
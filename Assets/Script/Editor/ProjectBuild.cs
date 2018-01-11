using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEditor;
using UnityEngine;

public class ProjectBuild : Editor {

    public enum PacketType
    {
        RELEASE,
        DEBUG
    }

    public enum NetType
    {
        OFFLINE,
        INTERNAL,
        OUTER
    }

    static PacketType Mode;
    static string version;
    static NetType netType;
    static string buildIdentify;
    static string productName;

    static void GetBuildParams()
    {
        string path = "";
        foreach (string arg in System.Environment.GetCommandLineArgs())
        {
            if (arg.StartsWith("buildParam"))
            {
                path = arg.Split('-')[1];
                path = path.Replace('\\', '/');
            }
        }
        Util.Log("Path:" + path);
        if (path != "")
        {            
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            XmlNode xn = doc.SelectSingleNode("params");
            foreach (XmlNode sxn in xn.ChildNodes)
            {
                switch(sxn.Name)
                {
                    case "mode":
                        if (sxn.InnerText == "Debug")
                            Mode = PacketType.DEBUG;
                        else
                            Mode = PacketType.RELEASE;
                        break;
                    case "net":
                        int t = Convert.ToInt32(sxn.InnerText);
                        if (t == 0)
                            netType = NetType.OFFLINE;
                        else if (t == 1)
                            netType = NetType.INTERNAL;
                        else if (t == 2)
                            netType = NetType.OUTER;
                        break;
                    case "version":
                        version = sxn.InnerText;
                        break;
                    case "packagename":
                        buildIdentify = sxn.InnerText;
                        break;
                    case "productname":
                        productName = sxn.InnerText;
                        break;
                }   
            }
        }        
        else
        {
            Util.LogError("The Path is Empty");
        }
    }

    static string[] GetBuildScenes()
    {
        List<string> names = new List<string>();

        foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
        {
            if (e == null)
                continue;
            if (e.enabled)
                names.Add(e.path);
        }
        return names.ToArray();
    }
    #region build Android
    static void BuildForAndroid()
    {
        GetBuildParams();
        string param = "";//LOCAL
        string identitfyStr = "";
        string productNameExt = "";
        BuildOptions opt = new BuildOptions();
        opt |= BuildOptions.None;
        if (Mode == PacketType.DEBUG)
        {
            param += ";ISDEBUG";
            opt |= BuildOptions.Development;
        }            
        if (netType == NetType.OFFLINE)
        {
            param += ";OFFLINE";
            identitfyStr = "_offline";
            productNameExt = "(单机)";
        }            
        else if (netType == NetType.INTERNAL)
        {
            param += ";INTERNAL";
            identitfyStr = "_internal";
            productNameExt = "(内网)";
        }            
        else if (netType == NetType.OUTER)
        {
            param += ";OUTER";
        }
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, param);
        PlayerSettings.bundleVersion = version;
        PlayerSettings.Android.bundleVersionCode = 1;
        PlayerSettings.applicationIdentifier = buildIdentify + identitfyStr;
        PlayerSettings.productName = productName + productNameExt;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        string timeStr = "_" + Convert.ToInt32(netType) + "_" + (DateTime.Now.Year % 100) + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
        Util.Log("GameName:" + productName + "  Identify:" + buildIdentify + identitfyStr + "  Version:" + version + "  Net:" + netType.ToString() + timeStr);
        string apkName = "XY_" + Mode.ToString().ToLower() + timeStr + ".apk";
        BuildPipeline.BuildPlayer(GetBuildScenes(), "PacketTool/android/bin/" + apkName, BuildTarget.Android, opt);
    }
    #endregion

    #region build Apple
    static void BuildForIPhone()
    {
        //UpdateVersionAndUpdateModeAndLuaEncode(VersionString, false);

        //string param = "ASYNC_MODE;INTERNAL;CROSS_PLATFORM_INPUT;MOBILE_INPUT;";
        //if (Mode == PacketType.DEBUG)
        //    param += "ISDEBUG";

        //PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, param);
        //PlayerSettings.bundleVersion = VersionString;
        //PlayerSettings.iOS.buildNumber = "1";
        //PlayerSettings.applicationIdentifier = "com.ty.platform2";
        //PlayerSettings.productName = "空之域(内网)";

        //string path = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/") + 1) + "XCodeProject";
        //bool flag = Directory.Exists(path);
        //if (!flag)
        //    Directory.CreateDirectory(path);

        //BuildPipeline.BuildPlayer(GetBuildScenes(), "XCodeProject/" + VersionString + "_" + Mode.ToString().ToLower(), BuildTarget.iOS, BuildOptions.None);
    }
    #endregion
}

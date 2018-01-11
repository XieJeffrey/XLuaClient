using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class OutLog : MonoBehaviour
{

    static List<string> mWriteTxt = new List<string>();
    private string outpath;
    private string tmpStr;

    void Awake()
    {
        //Application.logMessageReceived += HandleLog;

        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
            outpath = Application.persistentDataPath + "/log.txt";
        else
        {
            if (!Directory.Exists(Application.streamingAssetsPath))
                Directory.CreateDirectory(Application.streamingAssetsPath);
            outpath = Application.streamingAssetsPath + "/log.txt";
        }

        if (File.Exists(outpath))
        {
            File.Delete(outpath);
        }
        File.Create(outpath);
    }

    private void OnDisable()
    {
        //Application.logMessageReceived -= HandleLog;
    }

    void Update()
    {       
        if (mWriteTxt.Count > 0)
        {
            tmpStr = "";
            for (int i = 0; i < mWriteTxt.Count; i++)
            {
                tmpStr += mWriteTxt[i] + "\r\n";
            }
            File.AppendAllText(outpath, tmpStr);
            mWriteTxt.Clear();
        }
    }

    public static void RegisterLogCallBack()
    {
        
    }


    public static void HandleLog(string logString, string stackTrace, LogType type)
    {
        mWriteTxt.Add(logString);
        mWriteTxt.Add(stackTrace);      
    }
}
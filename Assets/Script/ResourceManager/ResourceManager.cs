using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UObject = UnityEngine.Object;
using Spine.Unity;

public class AssetBundleInfo
{
    public AssetBundle m_AssetBundle;
    public int m_ReferencedCount = 0;

    public AssetBundleInfo(AssetBundle assetBundle)
    {
        m_AssetBundle = assetBundle;
        m_ReferencedCount = 0;
    }
}

public class ResourceManager : MonoBehaviour
{

    class LoadAssetRequest
    {
        public Type assetType;
        public string[] assetNames;
        public Action<UObject[]> sharpFunc;
    }

    string m_BaseDownloadingURL = "";
    string[] m_AllManifest = null;

    AssetBundleManifest m_AssetBundleManifest = null;
    Dictionary<string, string[]> m_Dependencies = new Dictionary<string, string[]>();
    Dictionary<string, AssetBundleInfo> m_LoadedAssetBundles = new Dictionary<string, AssetBundleInfo>();
    Dictionary<string, List<LoadAssetRequest>> m_LoadRequests = new Dictionary<string, List<LoadAssetRequest>>();
    Dictionary<string, int> m_loadingRequests = new Dictionary<string, int>();

    public void Initialize(string manifestName, Action initOK)
    {
        m_BaseDownloadingURL = Util.GetRelativePath();
        Util.Log(m_BaseDownloadingURL);
        LoadAsset<AssetBundleManifest>(manifestName, new string[] { "AssetBundleManifest" }, delegate (UObject[] objs)
        {
            if (objs.Length > 0)
            {
                m_AssetBundleManifest = objs[0] as AssetBundleManifest;
                m_AllManifest = m_AssetBundleManifest.GetAllAssetBundles();
            }
            if (initOK != null) initOK();
        });
    }


    public void LoadPrefab(string abName, string assetName, Action<UObject[]> func)
    {
        LoadAsset<GameObject>(abName, new string[] { assetName }, func);
    }

    public void LoadPrefab(string abName, string[] assetName, Action<UObject[]> func)
    {
        LoadAsset<GameObject>(abName, assetName, func);
    }

    public void LoadImage(string abName, string assetName, Action<UObject[]> func)
    {
        LoadAsset<Sprite>(abName, new string[] { assetName }, func);
    }

    public void LoadImage(string abName, string[] assetName, Action<UObject[]> func)
    {
        LoadAsset<Sprite>(abName, assetName, func);
    }

    public void LoadAudio(string abName, string assetName, Action<UObject[]> func)
    {
        LoadAsset<AudioClip>(abName, new string[] { assetName }, func);
    }

    public void LoadSkelData(string abName, string assetName, Action<UObject[]> func)
    {
        LoadAsset<SkeletonDataAsset>(abName, new string[] { assetName }, func);
    }


    string GetRealAssetPath(string abName)
    {
        if (abName.Equals(Util.AssetDirname))
            return abName;
        abName = abName.ToLower();
        if (!abName.EndsWith(AppConst.ExtName))
        {
            abName += AppConst.ExtName;
        }
        if (abName.Contains("/"))
            return abName;

        for (int i = 0; i < m_AllManifest.Length; i++)
        {
            if (m_AllManifest[i].Equals(abName))
            {
                return m_AllManifest[i];
            }
        }
        Util.LogError("GetRealAssetPath Error:>>" + abName);
        return null;
    }

    void LoadAsset<T>(string abName, string[] assetNames, Action<UObject[]> action = null) where T : UObject
    {
        abName = GetRealAssetPath(abName);
        //请求句柄    
        LoadAssetRequest request = new LoadAssetRequest();
        request.assetType = typeof(T);
        request.assetNames = assetNames;
        request.sharpFunc = action;

        List<LoadAssetRequest> requests = null;
        if (!m_LoadRequests.TryGetValue(abName, out requests))
        {
            requests = new List<LoadAssetRequest>();
            requests.Add(request);
            m_LoadRequests.Add(abName, requests);
        }
        else
        {
            requests.Add(request);
          
            return;
        }    
        StartCoroutine(OnLoadAsset<T>(abName));
    }

    IEnumerator OnLoadAsset<T>(string abName) where T : UObject
    {
        //从已经加载的bundle里获取bundle
        AssetBundleInfo bundleInfo = GetLoadedAssetBundle(abName);
        if (bundleInfo == null)
        {
            //如果bundle为空，执行加载bundle的流程
            yield return StartCoroutine(OnLoadAssetBundle(abName, typeof(T)));

            bundleInfo = GetLoadedAssetBundle(abName);
            //加载完毕的时候如果已经加载的bundle还是空？请求加载的队列删除掉？
            if (bundleInfo == null)
            {
                m_LoadRequests.Remove(abName);
                Util.LogError("OnLoadAsset--->>>" + abName);
                yield break;
            }
        }
        List<LoadAssetRequest> list = null;
        //Util.Log("Get assetbundleInfo");
        if (!m_LoadRequests.TryGetValue(abName, out list))
        {
            m_LoadRequests.Remove(abName);
            yield break;
        }
        //Util.Log("准备实例化并执行请求回调");
        for (int i = 0; i < list.Count; i++)
        {
            string[] assetNames = list[i].assetNames;
            List<UObject> result = new List<UObject>();

            AssetBundle ab = bundleInfo.m_AssetBundle;
            for (int j = 0; j < assetNames.Length; j++)
            {
                string assetPath = assetNames[j];
                AssetBundleRequest request = ab.LoadAssetAsync(assetPath, list[i].assetType);
                yield return request;
                result.Add(request.asset);
//#if !UNITY_EDITOR
//                    AssetBundleRequest request = ab.LoadAssetAsync(assetPath, list[i].assetType);
//                    yield return request;
//                    result.Add(request.asset);
//#else
//                // UnityEngine.Object obj = ab.LoadAsset(assetPath, list[i].assetType);
//                //result.Add(obj);
//#endif

                //T assetObj = ab.LoadAsset<T>(assetPath);
                //result.Add(assetObj);
            }
            if (list[i].sharpFunc != null)
            {
                list[i].sharpFunc(result.ToArray());
                list[i].sharpFunc = null;
            }
            bundleInfo.m_ReferencedCount++;
        }
        m_LoadRequests.Remove(abName);
    }

    AssetBundleInfo GetLoadedAssetBundle(string abName)
    {
        AssetBundleInfo bundle = null;
        m_LoadedAssetBundles.TryGetValue(abName, out bundle);
        if (bundle == null) return null;

        // No dependencies are recorded, only the bundle itself is required.
        string[] dependencies = null;
        if (!m_Dependencies.TryGetValue(abName, out dependencies))
            return bundle;

        // Make sure all dependencies are loaded
        foreach (var dependency in dependencies)
        {
            AssetBundleInfo dependentBundle;
            m_LoadedAssetBundles.TryGetValue(dependency, out dependentBundle);
            if (dependentBundle == null) return null;
        }
        return bundle;
    }

    IEnumerator OnLoadAssetBundle(string abName, Type type)
    {
        string url = m_BaseDownloadingURL + abName;

        WWW download = null;
        if (type == typeof(AssetBundleManifest))
            download = new WWW(url);
        else
        {
            string[] dependencies = m_AssetBundleManifest.GetAllDependencies(abName);
            //获取该bundle的引用bundle
            if (dependencies.Length > 0)
            {
                if (m_Dependencies.ContainsKey(abName) == false)
                    m_Dependencies.Add(abName, dependencies);
                for (int i = 0; i < dependencies.Length; i++)
                {
                    string depName = dependencies[i];
                    AssetBundleInfo bundleInfo = null;
                    if (m_LoadedAssetBundles.TryGetValue(depName, out bundleInfo))
                    {
                        bundleInfo.m_ReferencedCount++;
                    }
                    else if (!m_LoadRequests.ContainsKey(depName))
                    {
                        if (m_loadingRequests.ContainsKey(depName))
                        {
                            m_loadingRequests[depName]++;
                        }
                        else
                        {
                            m_loadingRequests.Add(depName, 1);
                            yield return StartCoroutine(OnLoadAssetBundle(depName, type));
                        }
                    }
                }
            }
            download = WWW.LoadFromCacheOrDownload(url, m_AssetBundleManifest.GetAssetBundleHash(abName), 0);
        }
        yield return download;

        AssetBundle assetObj = download.assetBundle;
        if (assetObj != null)
        {
            m_LoadedAssetBundles.Add(abName, new AssetBundleInfo(assetObj));
            if (m_loadingRequests.ContainsKey(abName))
            {
                m_LoadedAssetBundles[abName].m_ReferencedCount = m_loadingRequests[abName];
                m_loadingRequests.Remove(abName);
            }
        }
    }

    public void UnLoadAssetBundle(string abName, bool isThorough = false)
    {
        abName = GetRealAssetPath(abName);
        Util.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory before unloading " + abName);
        UnloadAssetBundleInternal(abName, isThorough);
        UnloadDependencies(abName, isThorough);
        Util.Log(m_LoadedAssetBundles.Count + " assetbundle(s) in memory after unloading " + abName);
    }

    void UnloadDependencies(string abName, bool isThorough)
    {
        string[] dependencies = null;
        if (!m_Dependencies.TryGetValue(abName, out dependencies))
            return;

        // Loop dependencies.
        foreach (var dependency in dependencies)
        {
            UnloadAssetBundleInternal(dependency, isThorough);
        }
        m_Dependencies.Remove(abName);
    }

    void UnloadAssetBundleInternal(string abName, bool isThorough)
    {
        AssetBundleInfo bundle = GetLoadedAssetBundle(abName);
        if (bundle == null) return;

        if (--bundle.m_ReferencedCount <= 0)
        {
            if (m_LoadRequests.ContainsKey(abName))
            {
                return;     //如果当前AB处于Async Loading过程中，卸载会崩溃，只减去引用计数即可
            }
            bundle.m_AssetBundle.Unload(isThorough);
            m_LoadedAssetBundles.Remove(abName);
            Util.Log(abName + " has been unloaded successfully");
        }
    }
}

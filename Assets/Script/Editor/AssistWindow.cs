using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Spine.Unity;
using Spine;

public class AssistWindow : EditorWindow {

    [MenuItem("Game/Assistant Window")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(AssistWindow));
    }

    private string myassetName = "";

    void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        GUILayout.Label("游戏资源文件", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();        
        myassetName = EditorGUILayout.TextField("资源包名(自定义)", myassetName);
        if (GUILayout.Button("添加资源AB名称"))
        {
            SetABName(myassetName);
        }
        EditorGUILayout.EndHorizontal();      
          
        if (GUILayout.Button("添加资源AB名称(预制体用)"))
        {
            SetABName(myassetName);
        }

        if (GUILayout.Button("添加资源AB名称(以文件夹的名字为AB名字)"))
        {
            SetABNameByFolder();
        }

        if (GUILayout.Button("生成 BOSS 骨骼预置"))
        {
            GenSkelPrefab(SkelType.Boss);
        }
        if (GUILayout.Button("生成 主角 骨骼预置"))
        {
            GenSkelPrefab(SkelType.Role);
        }
        if (GUILayout.Button("生成 子弹 骨骼预置"))
        {
            GenSkelPrefab(SkelType.Bullet);
        }
        if (GUILayout.Button("生成 特效 骨骼预置"))
        {
            GenSkelPrefab(SkelType.Effect);
        }
        if (GUILayout.Button("生成 道具 骨骼预置"))
        {
            GenSkelPrefab(SkelType.Item);
        }
        GUILayout.EndVertical();
    }

    public static void SetABName(string str)
    {
        int count = Selection.instanceIDs.Length;

        string[] paths = new string[count];
        for (int i = 0; i < count; i++)
        {
            paths[i] = AssetDatabase.GetAssetPath(Selection.instanceIDs[i]);
        }
        foreach (string item in paths)
        {
            if (item.EndsWith(".cs")) return;
            string[] ps = item.Split('/');
            int length = ps.Length;
            string name = ps[length - 1];
            if (str == "")
            {
                name = ps[length - 1];
                name = name.ToLower();
            }
            else
            {
                name = str;
            }
            AssetImporter ai = AssetImporter.GetAtPath(item);

            //Debug.LogError(item);
            try
            {
                //Debug.LogError("ai类型：" + ai.GetType());
                ai.assetBundleName = name.Split('.')[0];
                ai.assetBundleVariant = "assetbundle";
            }
            catch (System.Exception)
            {
                //Debug.LogError(item + "不能打包");
            }
        }
    }

    public static void SetABNameByFolder()
    {
        int count = Selection.instanceIDs.Length;

        string[] paths = new string[count];
        for (int i = 0; i < count; i++)
        {
            paths[i] = AssetDatabase.GetAssetPath(Selection.instanceIDs[i]);
        }
        foreach (string item in paths)
        {
            if (item.EndsWith(".cs")) return;
            string[] ps = item.Split('/');
            int length = ps.Length;
            string name = ps[length - 2];
            name = name.Replace('_', '/');

            AssetImporter ai = AssetImporter.GetAtPath(item);

            //Debug.LogError(item);
            try
            {
                //Debug.LogError("ai类型：" + ai.GetType());
                ai.assetBundleName = name.Split('.')[0];
                ai.assetBundleVariant = "assetbundle";
            }
            catch (System.Exception)
            {
                //Debug.LogError(item + "不能打包");
            }
        }
    }

    enum SkelType {
        None = 0,
        Bullet = 1,
        Effect = 2,
        Item = 3,
        Role = 4,
        Boss = 5,
    }

    void GenSkelPrefab(SkelType skelType)
    {
        int count = Selection.instanceIDs.Length;
        if(count == 0)
        {
            Debug.LogError("请选择骨骼所在的文件夹1");
        }
        
        List<string> paths = new List<string>();
        for (int i = 0; i < count; i++)
        {
            string path = AssetDatabase.GetAssetPath(Selection.instanceIDs[i]);
            if (path.Contains(".") || !path.Contains("sk")) continue;
            paths.Add(path);
        }        
        if (paths == null || paths.Count == 0)
        {
            Debug.LogError("请选择骨骼所在的文件夹2");
        }        

        Dictionary<Slot, List<BoundingBoxAttachment>> boundingBoxTable = new Dictionary<Slot, List<BoundingBoxAttachment>>();
        for (int i = 0; i < paths.Count; ++i)
        {
            string path = paths[i];
            string targetpath = "";
            switch(skelType)
            {
                case SkelType.Bullet:
                    targetpath = "Assets/GameRes/Prefab/Battle/Bullet/";
                    break;
                case SkelType.Effect:
                    targetpath = "Assets/GameRes/Prefab/Effect/";
                    break;
                case SkelType.Item:
                    targetpath = "Assets/GameRes/Prefab/Skeleton/";
                    break;
                case SkelType.Role:
                    targetpath = "Assets/GameRes/Prefab/Skeleton/";
                    break;
                case SkelType.Boss:
                    targetpath = "Assets/GameRes/Prefab/Skeleton/";
                    break;
            }
            if (string.IsNullOrEmpty(targetpath))
            {
                Util.LogError("目标路径 Unkown");
                return;
            }

            boundingBoxTable.Clear();
            int lastidx = path.LastIndexOf('/') + 1 + 3; //3跳掉sk_
            string name = path.Substring(lastidx, path.Length - lastidx);
            string skelPath = path.Substring(0, path.LastIndexOf('/') + 1);
            string prefabName = name + ".prefab";
            string prefabPath = targetpath + prefabName;
            Debug.Log("name:" + prefabPath);
            if (File.Exists(prefabPath))
            {
                File.Delete(prefabPath);
            }
            GameObject go = new GameObject();
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.layer = LayerMask.NameToLayer("UI");
            
            //设置RectTransform
            RectTransform rt = go.AddComponent<RectTransform>();
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(80, 80);
            rt.localEulerAngles = Vector3.zero;

            GameObject skelGo = new GameObject();
            skelGo.name = "Skeleton";
            skelGo.transform.localPosition = Vector3.zero;
            skelGo.transform.localScale = new Vector3(100, 100);
            skelGo.transform.SetParent(go.transform);
                        
            //获取骨骼数据
            string skelDataPath = skelPath + "sk_" + name + "/" + name + "_SkeletonData.asset";
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(skelDataPath);
            SkeletonDataAsset skelData = AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>(skelDataPath);
            if(null == skelData)
            {
                Debug.LogError(name + "获取骨骼数据失败");
                return;
            }
            //创建骨骼
            SkeletonAnimation skelAnim = SkeletonAnimation.AddToGameObject(skelGo, skelData);
            //展开spine个骨骼结点到unity
            SkeletonUtility util = skelGo.AddComponent<SkeletonUtility>();
            util.SpawnHierarchy(SkeletonUtilityBone.Mode.Follow, true, true, true);

            if (skelType == SkelType.Bullet)
            {
                BoxCollider2D collider = go.AddComponent<BoxCollider2D>();
                go.tag = "Bullet";
                collider.isTrigger = true;
                Rigidbody2D rbody = go.AddComponent<Rigidbody2D>();
                rbody.gravityScale = 0f;
            }
            if (skelType == SkelType.Effect)
            {
              //  go.AddComponent<SkeletonEffect>();
            }
            if (skelType == SkelType.Boss)
            {
                BoxCollider2D collider = go.AddComponent<BoxCollider2D>();
                go.tag = "MonsterDying";
                collider.isTrigger = true;
                collider.enabled = false;

                //创建多边形碰撞 
                Skeleton skeleton = skelAnim.Skeleton;
                int slotCount = skeleton.Slots.Count;
                Skin skin = skeleton.Skin;
                if (skin == null) skin = skeleton.Data.DefaultSkin;
                //遍历所有的骨骼结点
                for (int m = 0; m < slotCount; ++m)
                {
                    Slot slot = skeleton.Slots.Items[m];
                    List<Attachment> slotAttachments = new List<Attachment>();
                    skin.FindAttachmentsForSlot(skeleton.FindSlotIndex(slot.Data.Name), slotAttachments);
                    for (int n = 0; n < slotAttachments.Count; ++n)
                    {
                        BoundingBoxAttachment bounding = slotAttachments[n] as BoundingBoxAttachment;
                        if (bounding != null)
                        {
                            string childName = GetChildName(slot.Bone);
                            Transform transform = util.boneRoot.Find(childName);
                            if (null != transform)
                            {
                                SkeletonUtility.AddBoundingBoxGameObject(null, bounding, slot, transform);
                            }
                        }
                    }
                }
            }
            if (skelType == SkelType.Role)
            {
                BoxCollider2D collider = go.AddComponent<BoxCollider2D>();
                collider.isTrigger = true;
                go.tag = "Hero";
                collider.size = new Vector2(30, 50);
                Rigidbody2D body = go.AddComponent<Rigidbody2D>();
                body.gravityScale = 0.0f;
                body.velocity = Vector2.zero;
                body.freezeRotation = true;
            }

            //创建预置
            Object prefab = PrefabUtility.CreateEmptyPrefab(prefabPath);
            PrefabUtility.ReplacePrefab(go, prefab, ReplacePrefabOptions.ConnectToPrefab);

            AssetImporter ai = AssetImporter.GetAtPath(prefabPath);
            ai.assetBundleName = name;
            ai.assetBundleVariant = "assetbundle";

            GameObject.DestroyImmediate(go);
        }
    }

    string GetChildName(Bone bone)
    {
        Bone parent = null;
        string name = bone.Data.Name;
        for (int i = 0; i < 100; ++i)
        {
            if (i == 0)
                parent = bone.Parent;
            else
                parent = parent.Parent;    
            if (parent == null)
            {
                break;
            }
            name = parent.Data.Name + "/" + name;
        }
        return name;
    }
}

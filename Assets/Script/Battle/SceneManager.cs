using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour {
    public List<SceneObject> m_sceneObjList = new List<SceneObject>();
    public Transform Parent;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 0; i < m_sceneObjList.Count; i++)
        {
            if (m_sceneObjList[i].IsLoad && m_sceneObjList[i].IsShow)
            {
                m_sceneObjList[i].OnUpdate();
            }
        }
	}

    public void LoadScene(int sceneID)
    {
        string abName = "";
        string assetName = "";
        SceneObject sObj = new SceneObject();
        sObj.ID = sceneID;  
        m_sceneObjList.Add(sObj);
        Main.ResManager.LoadPrefab(abName, assetName, sObj.OnLoad);
    }

    private void LoadHero(int heroID)
    {
        string abName = "";
        string assetName = "";
        Hero sObj = new Hero();
        sObj.ID = heroID;
        m_sceneObjList.Add(sObj);
        Main.ResManager.LoadPrefab(abName, assetName, sObj.OnLoad);
    }

    private void LoadMaster(int masterID)
    {
        string abName = "";
        string assetName = "";
        Monster sObj = new Monster();
        sObj.ID = masterID;
        m_sceneObjList.Add(sObj);
        Main.ResManager.LoadPrefab(abName, assetName, sObj.OnLoad);
    }

    private void LoadBullet(int bulletID)
    {
        string abName = "";
        string assetName = "";
        Bullet sObj = new Bullet();
        sObj.ID = bulletID;
        m_sceneObjList.Add(sObj);
        Main.ResManager.LoadPrefab(abName, assetName, sObj.OnLoad);
    }

}

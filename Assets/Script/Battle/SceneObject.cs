using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObject {
    public int ID;
    public GameObject m_gameObject;
    public Transform m_trans;

    public bool IsLoad = false;
    public bool IsShow {
        get {
            if (m_gameObject != null)
                return m_gameObject.activeSelf;
            return false;
        }
    }

    public SceneObject()
    {
        IsLoad = false; 
    }

    public virtual void OnInit()
    {

    }

    public virtual void OnLoad(UnityEngine.Object [] objs)
    {
        IsLoad = true;     
    }

    public virtual void OnUpdate()
    {

    }

    public virtual void OnClose()
    {
        IsLoad = false;
        GameObject.DestroyImmediate(m_gameObject);        
    }

    public virtual void Close()
    {

    }

}

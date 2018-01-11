using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;



public class Collider2DToSobj : MonoBehaviour
{
    Action<UnityEngine.Object> m_collEnterFunc;
    Action<UnityEngine.Object> m_collExitFunc;

    Action<UnityEngine.Object> m_triEnterFunc;
    Action<UnityEngine.Object> m_triExitFunc;
    Action<UnityEngine.Object> m_triStayFunc;

    public void Close()
    {
        m_collEnterFunc = null;
        m_collExitFunc = null;
        m_triEnterFunc = null;
        m_triExitFunc = null;
        m_triStayFunc = null;
    }

    public void RegisterColliderEnterFunc(Action<UnityEngine.Object> func)
    {
        m_collEnterFunc = func;
    }

    public void RegisterColliderExitFunc(Action<UnityEngine.Object> func)
    {
        m_collExitFunc = func;
    }

    public void RegisterTriggerEnterFunc(Action<UnityEngine.Object> func)
    {
        m_triEnterFunc = func;
    }

    public void RegisterTriggerExitFunc(Action<UnityEngine.Object> func)
    {
        m_triExitFunc = func;
    }

    public void RegisterTriggerStayFunc(Action<UnityEngine.Object> func)
    {
        m_triStayFunc = func;
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (null != m_collEnterFunc)
        {
            m_collEnterFunc(coll.gameObject);
        }
    }

    void OnCollisionExit2D(Collision2D coll)
    {
        if (null != m_collExitFunc)
        {
            m_collExitFunc(coll.gameObject);
        }
    }


    void OnTriggerEnter2D(Collider2D coll)
    {       
        if (null != m_triEnterFunc)
        {
            m_triEnterFunc(coll.gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D coll)
    {
        if (null != m_triStayFunc)
        {
            m_triStayFunc(coll.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (null != m_triExitFunc)
        {
            m_triExitFunc(coll.gameObject);
        }
    }
}


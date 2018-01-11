using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System;

public class UIEventListener : MonoBehaviour, IEventSystemHandler, IPointerDownHandler,
    IPointerUpHandler, IPointerClickHandler
{
    public delegate void VoidHandle(GameObject go, PointerEventData data);

    public VoidHandle onClick;
    public VoidHandle OnDown;
    public VoidHandle OnUp;

    public static UIEventListener Get(GameObject go)
    {
        UIEventListener evt = go.GetComponent<UIEventListener>();
        if (evt == null)
        {
            evt = go.AddComponent<UIEventListener>();
        }
        return evt;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null)
        {
            Main.AudioManager.Play(SoundID.Click);
            onClick(gameObject, eventData);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (OnDown != null)
        {
            OnDown(gameObject, eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (OnUp != null)
        {
            OnUp(gameObject, eventData);
        }
    }
}

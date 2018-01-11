using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class Timer : Singleton<Timer>
{
    private List<timer> timerList = new List<timer>();

    public class timer
    {
        public float duration = 0;
        public float time = 0;
        public int loopCount;
        public EventManager.voidHandle callBack;
        public timer(float _duration, int _loopCount, EventManager.voidHandle _func)
        {
            duration = _duration;
            time = duration;
            loopCount = _loopCount;
            callBack = _func;
        }

        public void Stop()
        {
            loopCount = 0;
            callBack = null;
        }
    }

    public void OnUpdate()
    {
        for (int i = timerList.Count - 1; i >= 0; i--)
        {
            timerList[i].time -= Time.deltaTime;
            if (timerList[i].time <= 0)
            {
                timerList[i].callBack();
                timerList[i].loopCount--;
                if (timerList[i].loopCount <= 0)
                {
                    timerList.RemoveAt(i);
                }
                else
                    timerList[i].time = timerList[i].duration;
            }
        }
    }

    public timer AddTimer(float duration, EventManager.voidHandle func, int loopTime = 1)
    {
        timer m_timer = new timer(duration, loopTime, func);
        timerList.Add(m_timer);
        return m_timer;
    }

    public void RemoveTimer(timer m_timer)
    {
        if(m_timer==null)
            return;
        if (timerList.Contains(m_timer))
            timerList.Remove(m_timer);
    }

    public void ResetTimer()
    {
        timerList.Clear();
    }
}


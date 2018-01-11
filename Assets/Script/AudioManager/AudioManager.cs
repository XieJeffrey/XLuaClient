using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    AudioSource m_bgm;
    AudioSource m_sound;
    List<AudioSource> m_soundASLst;
    GameObject m_audioRoot;
    Dictionary<int, AudioClip> m_clipCache;
    List<int> m_loadingLst; //要求加载,但是..

    void Awake() {
        
    }

    // Use this for initialization
    void Start () {
        m_clipCache = new Dictionary<int, AudioClip>();
        m_loadingLst = new List<int>();
        m_soundASLst = new List<AudioSource>();
        m_audioRoot = new GameObject();
        m_audioRoot.name = "AudioRoot";                
        m_bgm = m_audioRoot.AddComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        //Util.Log("Count:" + m_soundASLst.Count);
	}

    public IEnumerator PlaySound(AudioClip ac, float time, float volume, bool isloop)
    {
        yield return new WaitForSeconds(time);
        AudioSource audio = GetIdleAduioSource();
        audio.volume = volume;
        audio.clip = ac;
        audio.loop = isloop;
        audio.Play();
    }

    public void Play(int id, bool isloop = false)
    {
        if (m_loadingLst.IndexOf(id) != -1)
        {
            return;
        }
        if (m_clipCache.ContainsKey(id))
        {
            AudioClip ac;
            SoundConfigBase cached = SoundConfigBaseManager.instance.Find(id);
            m_clipCache.TryGetValue(id, out ac);
            if (ac != null)
            {
                if (id >= 2800)
                    PlayBgm(ac, cached.Volume);
                else
                    StartCoroutine(PlaySound(ac, cached.Delay, cached.Volume, isloop));
                return;
            }
        }
        SoundConfigBase data = SoundConfigBaseManager.instance.Find(id);
        if (data == null)
        {
            Util.LogError("SoundConfig has not key:" + id);
            return;
        }
        string assName = data.Sound;
        string abName = data.Sound.ToLower();
        m_loadingLst.Add(id);
        Main.ResManager.LoadAudio(abName, assName, (objs) =>
        {
            if (objs[0] == null)
            {
                Util.LogError("获取音频失败" + abName + "  " + assName);
                return;
            }
            AudioClip ac = objs[0] as AudioClip;
            if (ac != null)
            {
                if (id >= 2800)
                    PlayBgm(ac, data.Volume);
                else
                    StartCoroutine(PlaySound(ac, data.Delay, data.Volume, isloop));
                m_loadingLst.Remove(id);
                if (!m_clipCache.ContainsKey(id))
                {
                    m_clipCache.Add(id, ac);
                }                
            }
        });
    }

    public void Stop(int id)
    {
        SoundConfigBase data = SoundConfigBaseManager.instance.Find(id);
        for (int i = 0; i < m_soundASLst.Count; ++i)
        {
            if (m_soundASLst[i].clip.name == data.Sound)
            {
                m_soundASLst[i].Stop();
            }
        }
    }

    private AudioSource GetIdleAduioSource()
    {
        AudioSource audio = null;
        for (int i = 0; i < m_soundASLst.Count; ++i)
        {
            if (!m_soundASLst[i].isPlaying)
            {
                audio = m_soundASLst[i];
                break;
            }            
        }
        if (audio == null)
        {
            audio = m_audioRoot.AddComponent<AudioSource>();
            m_soundASLst.Add(audio);
        }
        return audio;
    }

    public void PlayBgm(AudioClip ac, float volume, bool isloop = true)
    {
        m_bgm.clip = ac;
        m_bgm.loop = true;
        m_bgm.volume = volume;
        m_bgm.Play();
    }

    public void StopBgm()
    {
        m_bgm.Stop();
    }

    public void Clear()
    {
        m_clipCache.Clear();
        m_loadingLst.Clear();
        StopAllCoroutines();
    }
}

using System.Collections.Generic;

using LostOasis;

using UnityEngine;

using Debug = UnityEngine.Debug;

[DefaultExecutionOrder(-1)]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [SerializeField] private List<Audio> audioList = new();
    public Dictionary<string, Audio> audios = new();
    [SerializeField] public float volume;
    private float prevVolume;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        audios = new();
        foreach (var audio in audioList)
        {
            audios.Add(audio.key.MegaTrim(), audio);
        }
    }

    public void Refresh()
    {
        volume = 1;
        foreach (var audio in audioList)
        {
            audio.source.volume = volume;
        }
    }

    private void Update()
    {
        if (volume == prevVolume) 
            return;

        foreach(var audio in audioList)
        {
            audio.source.volume = volume;
        }
        prevVolume = volume;
    }

    public static void Load()
    {
        foreach (var kvp in Instance.audios)
        {
            kvp.Value.Load();
        }
    }

    public static void Play(string key)
    {
        key = key.MegaTrim();
        if (Instance.audios.ContainsKey(key))
        {
            Instance.audios[key].Play();
        }
        else
        {
            Debug.LogWarning($"Audio [{key}] not found!");
        }
    }

    public static void Pause(string key)
    {
        key = key.MegaTrim();
        if (Instance.audios.ContainsKey(key))
        {
            Instance.audios[key].Pause();
        }
        else
        {
            Debug.LogWarning($"Audio [{key}] not found!");
        }
    }

    public static void Stop(string key)
    {
        key = key.MegaTrim();
        if (Instance.audios.ContainsKey(key))
        {
            Instance.audios[key].Stop();
        }
        else
        {
            Debug.LogWarning($"Audio [{key}] not found!");
        }
    }

    [System.Serializable]
    public class Audio
    {
        public string key;
        public AudioSource source;
        public bool sfx;
        public bool loop;
        public bool forcePlay = true;
        public bool mute;
        public void Load()
        {

        }
        public void Play()
        {
            if (!mute)
            {
                if (!forcePlay && source.isPlaying)
                {
                    return;
                }
                if (source.isPlaying && forcePlay)
                {
                    source.Stop();
                }
                source.Play();
            }
            else if(source.isPlaying)
            {
                Stop();
            }
        }
        public void Pause()
        {
            if (source.isPlaying)
            {
                source.Pause();
            }
        }
        public void UnPause()
        {
            if (!source.isPlaying)
            {
                source.UnPause();
            }
        }
        public void Stop()
        {
            if (source.isPlaying)
            {
                source.Stop();
            }
        }
    }
}

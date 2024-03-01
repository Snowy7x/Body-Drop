using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

[Serializable]public class Sound
{
    public string name;
    public float volume;
    public AudioClip[] clips;
    public AudioSource source;
    public AudioMixerGroup mixerGroup;
    [HideInInspector] public AudioSource currentSource;
    
    public void Play(AudioSource src = null, bool loop = false)
    {
        src = src != null? src : source;
        if (clips.Length < 1)
        {
            Debug.LogError("SoundManager [Sound]-[20]: No clips assigned for " + name + " sound!");
            return;
        }

        AudioClip clip = clips[Random.Range(0, clips.Length)];
        src.clip = clip;
        src.volume = volume;
        src.outputAudioMixerGroup = mixerGroup;
        src.pitch = Random.Range(0.95f, 1.05f);
        src.loop = loop;
        currentSource = src;
        src.Play();
    }
    
    
    public IEnumerator FadeOut (float fadeTime)
    {
        if (!currentSource) currentSource = SoundManager.Instance.defaultSource;
        float startVolume = currentSource.volume;
 
        while (currentSource.volume > 0) {
            currentSource.volume -= startVolume * Time.deltaTime / fadeTime;
 
            yield return null;
        }
 
        currentSource.Stop ();
        currentSource.volume = startVolume;
    }
    
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] private List<Sound> sounds;
    public AudioSource defaultSource;
    [SerializeField] AudioSource musicSrc;
    public AudioClip menuTheme;
    public AudioClip gameTheme;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }

    public void Start()
    {
        if (defaultSource == null)
        {
            defaultSource = gameObject.AddComponent<AudioSource>();
        }

        if (gameTheme)
        {
            musicSrc.clip = gameTheme;
            musicSrc.loop = true;
            musicSrc.Play();
        }
    }

    public void Play(string soundName, AudioSource source = null, bool loop = false)
    {
        Sound sound = sounds.Find(x => x.name == soundName);
        if (sound == null)
        {
            Debug.LogWarning("SoundManager [53]: There is not sound with the name: " + soundName + ".\nPlease Add one or check the spelling.");
            return;
        }
        source = source != null ? source : sound.source;
        source.loop = loop;
        sound.Play(sound.source != null ? null : source, loop);
    }

    public void StopLoop(string soundName, string endSound = "")
    {
        Sound sound = sounds.Find(x => x.name == soundName);
        if (sound == null)
        {
            Debug.LogWarning("SoundManager [53]: There is not sound with the name: " + soundName + ".\nPlease Add one or check the spelling.");
            return;
        }
        sound.currentSource.Stop();
        Play(endSound, sound.currentSource);
    }

    public void FadeOut(string soundName, float fadeTime = 1f)
    {
        Sound sound = sounds.Find(x => x.name == soundName);
        if (sound == null)
        {
            Debug.LogWarning("SoundManager [53]: There is not sound with the name: " + soundName + ".\nPlease Add one or check the spelling.");
            return;
        }

        StartCoroutine(sound.FadeOut(fadeTime));
    }
}

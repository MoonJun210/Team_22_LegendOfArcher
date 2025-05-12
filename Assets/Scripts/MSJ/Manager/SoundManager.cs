using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Volume Settings")]
    [SerializeField][Range(0f, 1f)] private float soundEffectVolume;
    [SerializeField][Range(0f, 1f)] private float soundEffectPitchVariance;
    [SerializeField][Range(0f, 1f)] private float musicVolume;

    private AudioSource musicAudioSource;
    [Header("Musics")]
    public AudioClip[] musicClips;

    [Header("Sound Effect Clips")]
    public AudioClip[] soundClips;

    public SoundSource soundSourcePrefab;

    private Dictionary<string, AudioClip> bgmClipDict = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> soundClipDict = new Dictionary<string, AudioClip>();


    public float SoundEffectVolume { get { return soundEffectVolume; } }
    public float SoundEffectPitchVariance { get { return soundEffectPitchVariance; } }
    public float MusicVolume { get { return musicVolume; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            musicAudioSource = GetComponent<AudioSource>();
            musicAudioSource.volume = musicVolume;
            musicAudioSource.loop = true;

            foreach (var clip in musicClips)
            {
                if (clip != null && !bgmClipDict.ContainsKey(clip.name))
                {
                    bgmClipDict.Add(clip.name, clip);
                    Debug.Log("Clip Name = " + clip.name);
                }

            }

            foreach (var clip in soundClips)
            {
                if (clip != null && !soundClipDict.ContainsKey(clip.name))
                    soundClipDict.Add(clip.name, clip);
            }

        }
        else
        {
            Destroy(gameObject); // 중복 방지
        }
    }


    private void Start()
    {
        ChangeBackGroundMusic("TestBgm");
    }

    public void ChangeBackGroundMusic(string clipName)
    {
        AudioClip clip = bgmClipDict[clipName];
        musicAudioSource.Stop();
        musicAudioSource.clip = clip;
        musicAudioSource.Play();
    }

    public static void PlayClip(string clipName)
    {
        if (!instance.soundClipDict.ContainsKey(clipName))
        {
            Debug.LogWarning($"SoundManager: Clip \"{clipName}\" not found!");
            return;
        }

        AudioClip clip = instance.soundClipDict[clipName];
        SoundSource obj = Instantiate(instance.soundSourcePrefab);
        SoundSource soundSource = obj.GetComponent<SoundSource>();
        soundSource.Play(clip, instance.soundEffectVolume, instance.soundEffectPitchVariance);
    }

    public void SetBGMVolume(float volume)
    {
        musicVolume = volume;
        if (musicAudioSource != null)
            musicAudioSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        soundEffectVolume = volume;
    }

}

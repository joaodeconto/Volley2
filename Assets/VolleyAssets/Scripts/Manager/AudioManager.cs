using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioSource musicSource; // For playing background music
    public AudioSource soundEffectSource; // For playing sound effects
    public AudioSource uiSource; // For playing UI sounds

    public Sound[] musicClips; // Array of music clips
    public Sound[] soundEffectClips; // Array of sound effect clips
    public Sound[] uiClips; // Array of UI sound clips

    private bool isMusicPlaying = false;

    [System.Serializable]
    public class Sound
    {
        public AudioType type;
        public AudioClip clip;
        [Range(0f, 2f)]
        public float volume = 1f;
        [Range(0f, 2f)]
        public float pitch = 1f;
        public bool randomizePitch = false;
        [Range(0f, 2f)]
        public float delay = 0f;
        [Range(0f, 2f)]
        public float reverb = 0f;
        public bool loop = false; // Add loop option
    }

    public enum AudioType
    {
        Music,
        SoundEffect,
        UI
    }
    private void Awake()
    {
        // Ensure only one instance of AudioManager exists
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        GameManager.OnStateEnter += PlayInGameMusic;
        GameOver.OnStateEnter += PlayGameOverMusic;
    }
    private void OnDestroy()
    {
        GameManager.OnStateEnter -= PlayInGameMusic;
        GameOver.OnStateEnter -= PlayGameOverMusic;
    }

    private void PlayInGameMusic()
    {
        if(!isMusicPlaying)
        {
            PlaySound(AudioType.Music, 1, musicSource);
            isMusicPlaying = true;
        }
    }
    public void PlayIntroMusic()
    {
        PlaySound(AudioType.Music, 0, musicSource);
    }
    private void PlayGameOverMusic()
    {
        isMusicPlaying = false;
        PlaySound(AudioType.Music, 2, musicSource);
    }

    public void PlaySound(AudioType type, int index, AudioSource source = null)
    {
        Sound[] sounds = null;

        switch (type)
        {
            case AudioType.Music:
                sounds = musicClips;
                source = musicSource;
                break;
            case AudioType.SoundEffect:
                sounds = soundEffectClips;
                break;
            case AudioType.UI:
                sounds = uiClips;
                source = uiSource;
                break;
                // Add more cases for additional audio categories if needed
        }

        if (sounds != null && index >= 0 && index < sounds.Length)
        {
            Sound sound = sounds[index];
            source.clip = sound.clip;
            source.volume = sound.volume;
            source.pitch = sound.randomizePitch ? Random.Range(0.9f * sound.pitch, 1.1f * sound.pitch) : sound.pitch;
            StartCoroutine(PlayDelayedSound(source, sound.delay, sound.reverb, sound.loop));
        }
    }

    private IEnumerator PlayDelayedSound(AudioSource source, float delay, float reverb, bool loop)
    {
        yield return new WaitForSeconds(delay);

        // Apply reverb effect
        AudioReverbFilter reverbFilter = source.gameObject.GetComponent<AudioReverbFilter>();
        if (reverbFilter == null)
        {
            reverbFilter = source.gameObject.AddComponent<AudioReverbFilter>();
        }
        reverbFilter.reverbPreset = AudioReverbPreset.Generic;
        reverbFilter.reverbLevel = reverb;

        source.loop = loop; // Set loop option for music
        source.Play();

        // Wait until the clip finishes playing (if not looping)
        if (!loop)
        {
            yield return new WaitForSeconds(source.clip.length);

            // Remove the reverb filter
            Destroy(reverbFilter);
        }
    }
}


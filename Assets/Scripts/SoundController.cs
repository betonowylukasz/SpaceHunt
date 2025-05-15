using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController Instance { get; private set; }

    public AudioClip walkClip;
    public AudioClip musicClip;

    private AudioSource musicSource;
    private AudioSource sfxSource;
    private AudioSource walkSource;

    private float sfxVolume = 0.4f;

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            GameObject.Destroy(Instance);
        }

        Instance = this;

        AudioSource[] audioSources = GetComponents<AudioSource>();
        Debug.Log("Liczba AudioSource: " + audioSources.Length);
        musicSource = audioSources[0];
        sfxSource = audioSources[1];
        walkSource = audioSources[2];

        musicSource.clip = musicClip;
        musicSource.loop = true;

        walkSource.clip = walkClip;
        walkSource.loop = true;
        walkSource.Pause();

        sfxVolume = GameSettings.Instance.GetSFXVolume();
        sfxSource.volume = sfxVolume;

        musicSource.volume = GameSettings.Instance.GetMusicVolume();
        walkSource.volume = GameSettings.Instance.GetSFXVolume() * 0.75f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySound(AudioClip audioClip, float volume = 0.4f, float pitch = 1f)
    {
        sfxSource.volume = volume * sfxVolume;
        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(audioClip);
    }

    public void StartWalking()
    {
        walkSource.UnPause();
    }

    public void StopWalking()
    {
        walkSource.Pause();
    }
}

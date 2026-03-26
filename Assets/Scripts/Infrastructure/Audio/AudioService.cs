using UnityEngine;
using UnityEngine.Audio;

public class AudioService : MonoBehaviour, IAudioService
{
    private const string BGM_KEY = "BGMVolume";
    private const string SFX_KEY = "SFXVolume";

    [Header("Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource bgmSource;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip clickClip;
    [SerializeField] private AudioClip placeSFX;
    [SerializeField] private AudioClip mergeSFX;
    [SerializeField] private AudioClip spawnSFX;

    [Header("SFX Clips")]
    [SerializeField] private AudioClip gameBGM;
    [SerializeField] private AudioClip mainMenuBGM;
    [SerializeField] private AudioClip gameOverBGM;

    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;

    private void Awake()
    {
        LoadVolume();
    }

    // ---------------- PLAY ----------------

    public void PlayClickSFX()
    {
        if (clickClip != null)
            sfxSource.PlayOneShot(clickClip);
    }

    public void PlayPlaceSFX()
    {
        if (placeSFX != null)
            sfxSource.PlayOneShot(placeSFX);
    }

    public void PlayMergeSFX()
    {
        if (placeSFX != null)
            sfxSource.PlayOneShot(placeSFX);
    }

    public void PlaySpawnSFX()
    {
        if (placeSFX != null)
            sfxSource.PlayOneShot(placeSFX);
    }

    public void PlayGameBGM()
    {
        bgmSource.clip = gameBGM;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlayGameOverBGM()
    {
        bgmSource.clip = gameOverBGM;
        bgmSource.loop = false;
        bgmSource.Play();
    }

    public void PlayMainMenuBGM()
    {
        bgmSource.clip = mainMenuBGM;
        bgmSource.loop = false;
        bgmSource.Play();
    }
    
    // ---------------- VOLUME ----------------

    public void SetBGMVolume(float value)
    {
        mixer.SetFloat("BGMVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(BGM_KEY, value);
    }

    public void SetSFXVolume(float value)
    {
        mixer.SetFloat("SFXVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat(SFX_KEY, value);
    }

    public float GetBGMVolume()
    {
        return PlayerPrefs.GetFloat(BGM_KEY, 1f);
    }

    public float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat(SFX_KEY, 1f);
    }

    private void LoadVolume()
    {
        SetBGMVolume(GetBGMVolume());
        SetSFXVolume(GetSFXVolume());
    }
}
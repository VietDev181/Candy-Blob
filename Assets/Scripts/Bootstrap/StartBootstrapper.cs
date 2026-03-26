using UnityEngine;

public class StartBootstrapper : MonoBehaviour
{
    [SerializeField] private AudioService audioService;
    IAudioService _audio;

    private void Awake()
    {
        _audio = audioService;
        _audio.PlayMainMenuBGM();
    }
}
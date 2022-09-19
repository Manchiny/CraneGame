using System.Collections.Generic;
using UnityEngine;

public class GameSound : MonoBehaviour
{
    [SerializeField] private List<AudioClip> _splashesClips;
    [SerializeField] private List<AudioClip> _hitClips;
    [SerializeField] private AudioClip _seaNoise;
    [Space]
    [SerializeField] private AudioClip _carNoise;
    [SerializeField] private AudioClip _carSignal;

    private const float SplashesVolume = 1f;
    private const float HitVolume = 0.1f;
    private const float SeaNoiseVolume = 0.45f;

    private const float CarNoiseVolume = 0.15f;
    private const float CarSignalVolume = 0.15f;

    private AudioSource _water;

    private LevelLoader _levelLoader;
    private bool _isLevelStarted;

    private void OnDestroy()
    {
        _levelLoader.LevelLoaded -= OnLevelLoaded;
        _levelLoader.LevelExited -= OnLevelExited;
    }

    public void Init(LevelLoader levelLoader)
    {
        _levelLoader = levelLoader;
        _levelLoader.LevelLoaded += OnLevelLoaded;
        _levelLoader.LevelExited += OnLevelExited;

        SetSound(Game.User.IsSoundOn());
    }

    public void SetSound(bool needActivate)
    {
        Game.User.SetSoundActive(needActivate);
        AudioListener.volume = needActivate ? 1 : 0;
    }

    public void SetWater(AudioSource source)
    {
        _water = source;
    }

    public void PlayCarNoiseSound(AudioSource source)
    {
        source.loop = true;
        PlaySound(source, _carNoise, CarNoiseVolume);
    }

    public void PlayCarSignalSound (AudioSource source)
    {
        source.loop = false;
        PlaySound(source, _carSignal, CarSignalVolume);
    }

    public void PlaySplashSound(AudioSource source)
    {
        var clip = _splashesClips[Random.Range(0, _splashesClips.Count)];
        PlaySound(source, clip, SplashesVolume);
    }

    public void PlayHitSound(AudioSource source)
    {
        if (_isLevelStarted == false)
            return;

        var clip = _hitClips[Random.Range(0, _hitClips.Count)];
        PlaySound(source, clip, HitVolume);
    }

    private void PlaySound(AudioSource source, AudioClip clip, float volume)
    {
        source.volume = volume;
        source.clip = clip;
        source.Play();
    }

    private void OnLevelLoaded()
    {
        _isLevelStarted = true;

        if (_water != null)
            PlaySeaNoise(_water);
    }

    private void OnLevelExited()
    {
        _isLevelStarted = false;

        StopSeaNoise();
    }

    private void PlaySeaNoise(AudioSource source)
    {
        source.loop = true;
        PlaySound(source, _seaNoise, SeaNoiseVolume);
    }

    private void StopSeaNoise()
    {
        if (_water != null)
            _water.Stop();
    }
}

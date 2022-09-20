using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ContainerSound : MonoBehaviour
{
    private const float MuteSoundAfterInitSeconds = 2f;
    private const float WaterSoundDuration = 0.4f;
    private const float HitSoundDuration = 0.4f;

    private bool _isWaterSoundPlaying;
    private bool _canPlayHitSound;
    private bool _isSoundInited;

    public AudioSource AudioSource { get; private set; }
    public bool CanPlayHitSound => _canPlayHitSound &&  _isSoundInited && _isWaterSoundPlaying == false;

    private void Start()
    {
        AudioSource = GetComponent<AudioSource>();
    }

    public void Init()
    {
        Utils.WaitSeconds(MuteSoundAfterInitSeconds)
            .Then(() =>
            {
                _isSoundInited = true;
                _canPlayHitSound = true;
            });
    }

    public void PlayHitSound()
    {
        if (_canPlayHitSound)
        {
            _canPlayHitSound = false;
            Game.Sound.PlayHitSound(AudioSource);

            Utils.WaitSeconds(HitSoundDuration)
                .Then(() => _canPlayHitSound = true);
        }
    }

    public void PlaySplashSound()
    {
        Game.Sound.PlaySplashSound(AudioSource);

        _isWaterSoundPlaying = true;

        Utils.WaitSeconds(WaterSoundDuration)
            .Then(() => _isWaterSoundPlaying = false);
    }
}

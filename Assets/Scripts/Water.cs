using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Water : MonoBehaviour, INoiseless
{
    [SerializeField] private ParticleSystem _splashesFX;
    
    private AudioSource _audioSource;
    private Quaternion _fxRotation = Quaternion.Euler(-90f, 0, 0);

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        Game.Sound.SetWater(_audioSource);
    }

    public void PlaySplashesEffect(Vector3 position)
    {
        var effect = Instantiate(_splashesFX, position, _fxRotation);
    }
}

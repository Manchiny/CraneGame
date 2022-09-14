using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] private ParticleSystem _splashesFX;

    private Quaternion _fxRotation = Quaternion.Euler(-90f, 0, 0);

    public void PlaySplashesEffect(Vector3 position)
    {
        var effect = Instantiate(_splashesFX, position, _fxRotation);
    }
}

using UnityEngine;

public class CarPlatform : MonoBehaviour
{
    public Car Car { get; private set; }

    private void Awake()
    {
        Car = GetComponentInParent<Car>();
    }
}

using UnityEngine;

public class ConteinerCarChecker : Checker
{
    public bool IsOnCar { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out CarPlatform platform))
            IsOnCar = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out CarPlatform platform))
            IsOnCar = false;
    }
}

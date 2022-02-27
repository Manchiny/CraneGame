using UnityEngine;

public class ConteinerCarChecker : MonoBehaviour
{
    public bool IsOnCar { get; private set; }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<CarPlatform>() == true)
        {
            IsOnCar = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<CarPlatform>() == true)
        {
            IsOnCar = false;
        }
    }
}

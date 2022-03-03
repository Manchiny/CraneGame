using System;
using UnityEngine;

public class ParkingPlace : MonoBehaviour
{
    public int Id { get; set; }
    public Car Car { get; private set; }
    public bool IsFree => Car == null && IsAwate == false;
    public bool IsAwate { get; set; }

    public Action OnFree;

    public void SetCar(Car car)
    {
        Car = car;
        IsAwate = false;

        if (car == null)
        {
            OnFree?.Invoke();
        }
            
    }
}

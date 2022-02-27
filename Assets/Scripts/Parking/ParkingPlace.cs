using System;
using UnityEngine;

public class ParkingPlace : MonoBehaviour
{
    public Car Car { get; private set; }
    public bool IsFree => Car == null;
  //  public bool IsAwate { get; set; }

    public Action OnFree;
    [HideInInspector] public int Id;
    public void SetCar(Car car)
    {
        Car = car;
   //     IsAwate = false;

        if (car == null)
            OnFree?.Invoke();
    }
}

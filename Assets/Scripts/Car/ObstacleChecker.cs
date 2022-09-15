using System.Collections.Generic;
using UnityEngine;

public class ObstacleChecker : Checker
{
    private Car _car;
    private HashSet<Collider> _obstacles = new HashSet<Collider>();

    public void Init(Car car)
    {
        _car = car;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<Container>() != null || other.gameObject.GetComponent<CarPlatform>() != null)
        {
            _obstacles.Add(other);
            _car.SetFreeWay(false);
        }
       
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Container>() != null || other.gameObject.GetComponent<CarPlatform>() != null)
        {
            _obstacles.Remove(other);

            if (_obstacles.Count == 0)
                _car.SetFreeWay(true);
        }
    }
}

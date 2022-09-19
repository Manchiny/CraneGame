using System;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleChecker : Checker
{
    private HashSet<Collider> _obstacles = new HashSet<Collider>();

    public bool HasObstacles => _obstacles.Count > 0;

    public event Action ObstaclesChanged;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<Container>() != null || other.gameObject.GetComponent<CarPlatform>() != null)
        {
            _obstacles.Add(other);
            ObstaclesChanged?.Invoke();
        }
       
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Container>() != null || other.gameObject.GetComponent<CarPlatform>() != null)
        {
            _obstacles.Remove(other);
            ObstaclesChanged?.Invoke();
        }
    }
}

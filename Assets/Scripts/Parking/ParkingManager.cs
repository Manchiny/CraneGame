using System.Collections.Generic;
using UnityEngine;

public class ParkingManager : MonoBehaviour
{
    public static ParkingManager Instance { get; private set; }

    [SerializeField] private ParkingPlace[] _parkingPlaces;
    [SerializeField] private Transform _exitPoint;
    [SerializeField] private CarSpawner _spawner;
    public Transform GetExitPoint() => _exitPoint;

    private List<Car> _activeCars = new List<Car>();
    private List<Car> _allCarsOnParking = new List<Car>();
    private Queue<Container> _crushedContainers = new Queue<Container>();

    private bool _isCrushProcessed;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            return;
        }

        Destroy(this);
    }

    public void Init()
    {
        InitParkingPlaces();
        _spawner.Init(OnNewCarCreated);
    }

    public void SubscribeOnCrush(Ship ship)
    {
        ship.OnCrush += OnContainerCrush;
    }
    private void InitParkingPlaces()
    {
        for (int i = 0; i < _parkingPlaces.Length; i++)
        {
            _parkingPlaces[i].Id = i;
        }
    }
    private void OnContainerCrush(Container container)
    {
        _crushedContainers.Enqueue(container);

        if (_isCrushProcessed == false)
            ProcessCrushConteiner();
    }
    private void ProcessCrushConteiner()
    {
        if (_crushedContainers.Count <= 0)
            return;

        _isCrushProcessed = true;
        Car matchedColorCar = null;
        Container container = _crushedContainers.Dequeue();

        foreach (var car in _activeCars)
        {
            if (container.ContainerColor == car.NeedColor)
            {
                matchedColorCar = car;
                continue;
            }
        }

        if (matchedColorCar != null)
        {
            matchedColorCar.OnContainerCrush();
        }

        _isCrushProcessed = false;

        if (_crushedContainers.Count > 0)
            OnContainerCrush(_crushedContainers.Peek());
    }

    public void OnNewCarCreated(Car car)
    {
        _activeCars.Add(car);
        _allCarsOnParking.Add(car);
        car.OnMoveExit += OnCarMoveExit;
        car.OnCompleteLoading += OnCarCompleteLoading;
    }
    private void OnCarMoveExit(Car car)
    {
        if (_allCarsOnParking.Count > 0)
        {
            _allCarsOnParking.Remove(car);
            car.OnMoveExit -= OnCarMoveExit;

            _spawner.AddCarToQueue();
        }
    }
    private void OnCarCompleteLoading(Car car, bool isSuccess)
    {
        car.OnCompleteLoading -= OnCarCompleteLoading;
        _activeCars.Remove(car);

        if (car.CurrentPlace.Id == 0)
        {
            car.MoveToExit();
            MoveNextCar(car.CurrentPlace.Id + 1, true);
        }
    }

    private void MoveNextCar(int nextPlaceId, bool isPreviousExit)
    {
        if (nextPlaceId < _parkingPlaces.Length)
        {
            Car car = _parkingPlaces[nextPlaceId].Car;
            if (car != null)
            {
                if (car.IsComplete && isPreviousExit)
                {
                    car.MoveToExit();
                    MoveNextCar(nextPlaceId + 1, true);
                }
                else
                {
                    car.MoveToParking(GetFreeParkingPlaceOnHead());
                    MoveNextCar(nextPlaceId + 1, false);
                }
            }            
        }
    }
    public ParkingPlace GetFreeParkingPlaceOnEnd()
    {
        ParkingPlace place = null;
        for (int i = _parkingPlaces.Length - 1; i >= 0; i--)
        {
            if (_parkingPlaces[i].IsFree)
                place = _parkingPlaces[i];
            else
                return place;
        }
        return place;
    }
    private ParkingPlace GetFreeParkingPlaceOnHead()
    {
        ParkingPlace place = null;
        for (int i = 0; i < _parkingPlaces.Length; i++)
        {
            if (_parkingPlaces[i].IsFree)
            {
                if (place == null)
                {
                    place = _parkingPlaces[i];
                    return place;
                }
            }
        }
        return place;
    }
}

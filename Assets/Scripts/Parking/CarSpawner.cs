using System;
using UniRx;
using UnityEngine;
using static ColorManager;

public class CarSpawner : MonoBehaviour
{
    [SerializeField] private Car _carPrefab;
    [SerializeField] private Transform _spawnPoint;

    private const float Cooldawn = 2f;
    private const int MaxCars = 3;

    private ParkingManager _parkingManager;

    private int _needCars = 3;
    private float _timer;

    private IDisposable _timerDispose;
    private Car _car = null;

    public event Action<Car> NewCarCreated;

    private void OnDestroy()
    {
        RemoveSubscribes();
    }

    public void Init(Action<Car> onCreateCar)
    {
        Game.LevelLoader.LevelExited += OnLevelExit;
        _parkingManager = Game.LevelManager.ParkingManager;
        _car = null;

        NewCarCreated += onCreateCar;

        _needCars = MaxCars;
        CreateCar();
    }

    public void AddCarToQueue()
    {
        if (_needCars < 1 && _timer <= 0)
        {
            _needCars++;
            CreateCar();
        }
        else
        {
            StartTimer();
            _needCars++;
        }
    }

    private void CreateCar()
    {
        if (Game.LevelManager.HasAvailibleContainers == false || Game.LevelManager.HasAnyAvailibleColor == false)
            return;

        var place = _parkingManager.GetFreeParkingPlaceOnEnd();

        if (place != null && _needCars > 0)
        {
            ContainerColor color = Game.LevelManager.GetRandomAvailibleContainerColor;

            var position = _spawnPoint.position;
            position.y = 0;

            _car = Instantiate(_carPrefab, position, _spawnPoint.rotation).GetComponent<Car>();
            _needCars--;
            _car.Init(place, color);
            NewCarCreated?.Invoke(_car);

            StartTimer();
        }
    }

    private void StartTimer()
    {
        if (_timerDispose != null)
            return;

        _timer = Cooldawn;

        _timerDispose = Observable.EveryUpdate().Subscribe(_ =>
            {
                Timer();
            }).AddTo(gameObject);
    }

    private void Timer()
    {
        if (_timer >= 0)
            _timer -= Time.deltaTime;
        else
        {
            if (_timerDispose != null)
            {
                _timerDispose.Dispose();
                _timerDispose = null;
            }

            if (_needCars > 0)
                CreateCar();
        }
    }

    private void OnLevelExit()
    {
        RemoveSubscribes();
        _needCars = 0;
    }

    private void RemoveSubscribes()
    {
        if (_timerDispose != null)
        {
            _timerDispose.Dispose();
            _timerDispose = null;
        }

        Game.LevelLoader.LevelExited -= OnLevelExit;
    }
}

using System;
using UniRx;
using UnityEngine;
using static ColorManager;

public class CarSpawner : MonoBehaviour
{
    [SerializeField] private Car _carPrefab;
    [SerializeField] private Transform _spawnPoint;

    private ParkingManager _parkingManager;

    private int _needCars = 3;
    private const float COOLDAWN = 2f;
    private float _timer;
    private CompositeDisposable _timerDispose = new CompositeDisposable();
    private Car _car = null;

    public Action<Car> OnNewCarCreate;
    public void Init(Action<Car> onCreateCar)
    {
        _parkingManager = ParkingManager.Instance;
        OnNewCarCreate += onCreateCar;

        _needCars = 3;
        CreateCar();
    }
    private void CreateCar()
    {
        if (LevelManager.Instance.HasAvailibleContainers() == false || LevelManager.Instance.HasAvailibleColors() == false)
            return;

        var place = _parkingManager.GetFreeParkingPlaceOnEnd();

        if (place != null && _needCars > 0)
        {
            ContainerColor color = LevelManager.Instance.GetRandomAvailibleContainerColor();

            var position = _spawnPoint.position;
            position.y = 0;

            _car = Instantiate(_carPrefab, position, _spawnPoint.rotation).GetComponent<Car>();
            _needCars--;
            _car.Init(place, color);
            OnNewCarCreate?.Invoke(_car);
            StartTimer();
        }
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
    private void StartTimer()
    {
        if (_timerDispose.Count > 0)
            return;

        _timer = COOLDAWN;

        Observable.EveryUpdate().Subscribe(_ =>
        {
            Timer();
        }).AddTo(_timerDispose);
    }

    private void Timer()
    {
        if (_timer >= 0)
            _timer -= Time.deltaTime;
        else
        {
            _timerDispose.Clear();

            if(_needCars > 0)
                CreateCar();
        }
    }
}

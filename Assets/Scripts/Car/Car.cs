using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using static ColorManager;

public class Car : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private MeshRenderer _colorSignal;
    [SerializeField] private ObstacleChecker _obstacleChecker;

    //TODO: Acceleration speed
    private float _destinationDistance = 0.1f;

    private IDisposable _moveDispose;
    private HashSet<Container> _wrongContainers = new HashSet<Container>();

    private ContainerColor _needColor;
    public ContainerColor NeedColor
    {
        get => _needColor;
        private set
        {
            _needColor = value;
            SetColorSignal(value);
            ColorSeted?.Invoke(value);
        }
    }

    public ParkingPlace CurrentPlace { get; private set; }
    public bool IsComplete { get; private set; }
    public bool IsWayFree { get; private set; } = true;

    public bool HasWrongsContainers => _wrongContainers.Count > 0;

    public event Action<Car> Exited;
    public event Action<Car, bool> LoadingCompleted;
    public event Action<ContainerColor> ColorSeted;
    public event Action FreeWayChanged;

    private void OnDestroy()
    {
        Game.LevelLoader.LevelExited -= OnLevelExit;

        if(_moveDispose != null)
            _moveDispose.Dispose();
    }

    public void Init(ParkingPlace place, ContainerColor needColor)
    {
        Game.LevelLoader.LevelExited += OnLevelExit;

        _obstacleChecker.Init(this);

        NeedColor = needColor;
        MoveToParking(place);
    }

    public void MoveToParking(ParkingPlace place)
    {
        if (_moveDispose != null)
            _moveDispose.Dispose();

        place.IsAwate = true;

        if (CurrentPlace != null)
            CurrentPlace.SetCar(null);

        CurrentPlace = place;

        _moveDispose = Observable.EveryUpdate().Subscribe(_ =>
                            {
                                MoveTo(place.transform, () => OnParking(place));
                            }).AddTo(gameObject);
    }

    public void CopmpleteLoading(bool isSuccess)
    {
        IsComplete = true;
        _colorSignal.gameObject.SetActive(false);
        LoadingCompleted?.Invoke(this, isSuccess);
    }

    public void MoveToExit()
    {
        if (_moveDispose != null)
            _moveDispose.Dispose();

        CurrentPlace.SetCar(null);

        Transform exit = Game.LevelManager.ParkingManager.GetExitPoint();

        _moveDispose = Observable.EveryUpdate().Subscribe(_ =>
                            {
                                MoveTo(exit, OnExit);
                            }).AddTo(gameObject);
    }

    public void OnContainerCrush()
    {
        var levelManager = Game.LevelManager;
        bool isHasAvailibleColorContainer = levelManager.HasAvailibleColor(NeedColor);

        if (isHasAvailibleColorContainer == false) // если ожидаемого цвета контейнеров больше нет - получаем новый
        {
            if (levelManager.HasAvailibleColors() && levelManager.HasAvailibleContainers)
            {
                NeedColor = levelManager.GetRandomAvailibleContainerColor();
            }
            else
            {
                CopmpleteLoading(false);
            }
        }
    }

    public void SetFreeWay(bool value)
    {
        IsWayFree = value;
        FreeWayChanged?.Invoke();
    }

    public void AddWrongContainer(Container container)
    {
        _wrongContainers.Add(container);
        FreeWayChanged?.Invoke();
    }

    public void RemoveWrongContainer(Container container)
    {
        _wrongContainers.Remove(container);
        FreeWayChanged?.Invoke();
    }

    private void MoveTo(Transform point, Action onComplete = null)
    {
        Vector3 target = point.position;
        target.y = 0;
        var heading = target - transform.position;

        if (heading.sqrMagnitude < _destinationDistance * _destinationDistance)
        {
            if (onComplete != null)
                onComplete?.Invoke();

            if(_moveDispose != null)
            {
                _moveDispose.Dispose();
                _moveDispose = null;
            }

            return;
        }

        if (IsWayFree && HasWrongsContainers == false)
        {
            transform.position += (target - transform.position).normalized * _speed * Time.deltaTime;
        }
    }

    private void OnParking(ParkingPlace place)
    {
        if (_moveDispose != null)
            _moveDispose.Dispose();

        place.SetCar(this);
    }

    private void OnExit()
    {
        if (_moveDispose != null)
            _moveDispose.Dispose();

        Exited?.Invoke(this);
        Destroy(gameObject);
    }

    private void SetColorSignal(ContainerColor color)
    {
        Material material = Game.LevelManager.GetSignalMaterial(color);
        _colorSignal.sharedMaterial = material;
    }

    private void OnLevelExit()
    {
        Destroy(gameObject);
    }
}

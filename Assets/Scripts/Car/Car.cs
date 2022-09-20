using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using static ColorDataBase;

[RequireComponent(typeof(AudioSource))]
public class Car : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private MeshRenderer _colorSignal;
    [SerializeField] private ObstacleChecker _obstacleChecker;
    [SerializeField] private ParticleSystem _containerPlaceFX;
    [Space]
    [SerializeField] private AudioSource _carNoiseSource;
    [SerializeField] private AudioSource _carSignalSource;

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

    public bool HasWrongsContainers => _wrongContainers.Count > 0;
    public bool IsWayFree => _obstacleChecker.HasObstacles == false;
    public bool CanMove => IsWayFree && HasWrongsContainers == false;

    public event Action<Car> Exited;
    public event Action<Car, bool> LoadingCompleted;
    public event Action<ContainerColor> ColorSeted;
    public event Action FreeWayChanged;

    private void OnDestroy()
    {
        Game.LevelLoader.LevelExited -= OnLevelExit;
        _obstacleChecker.ObstaclesChanged -= OnObstaclesChanged;

        if (_moveDispose != null)
            _moveDispose.Dispose();
    }

    public void Init(ParkingPlace place, ContainerColor needColor)
    {
        Game.LevelLoader.LevelExited += OnLevelExit;
        Game.Sound.PlayCarNoiseSound(_carNoiseSource);

        _obstacleChecker.ObstaclesChanged += OnObstaclesChanged;

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

        Transform exit = Game.Level.Parking.GetExitPoint();

        _moveDispose = Observable.EveryUpdate().Subscribe(_ =>
                            {
                                MoveTo(exit, OnExit);
                            }).AddTo(gameObject);
    }

    public void OnContainerCrush()
    {
        var levelManager = Game.Level;
        bool isHasAvailibleColorContainer = levelManager.HasAvailibleColor(NeedColor);

        if (isHasAvailibleColorContainer == false) // если ожидаемого цвета контейнеров больше нет - получаем новый
        {
            if (levelManager.HasAnyAvailibleColor && levelManager.HasAvailibleContainers)
                NeedColor = levelManager.GetRandomAvailibleContainerColor;
            else
                CopmpleteLoading(false);
        }
    }

    public void OnObstaclesChanged()
    {
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
        var distance = target - transform.position;

        if (distance.sqrMagnitude < _destinationDistance * _destinationDistance)
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

        if (CanMove)
            transform.position += (target - transform.position).normalized * _speed * Time.deltaTime;
    }

    private void OnParking(ParkingPlace place)
    {
        if (_moveDispose != null)
            _moveDispose.Dispose();

        place.SetCar(this);
        Game.Sound.PlayCarSignalSound(_carSignalSource);
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
        Material material = Game.Level.GetSignalMaterial(color);
        _colorSignal.sharedMaterial = material;
        _containerPlaceFX.startColor = material.color;
    }

    private void OnLevelExit()
    {
        Destroy(gameObject);
    }
}

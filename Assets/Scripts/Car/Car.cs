using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using static ColorManager;

public class Car : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private MeshRenderer _colorSignal;
    [SerializeField] private ObstacleChecker _obstacleChecker;

    private ContainerColor _needColor;
    public ContainerColor NeedColor
    {
        get => _needColor;
        private set
        {
            _needColor = value;
            SetColorSignal(value);
            OnColorSet?.Invoke(value);
        }
    }
    private float _destinationDistance = 0.1f;
    //TODO: Acceleration speed
    public ParkingPlace CurrentPlace { get; private set; }
    private CompositeDisposable _disposeMove = new CompositeDisposable();
    public bool IsComplete { get; private set; }
    public bool IsWayFree { get; private set; } = true;
    public bool IsWrongContainer { get; private set; }
    private HashSet<Container> _wrongContainers = new HashSet<Container>();
    public bool HasWrongsContainers => _wrongContainers.Count > 0;

    public Action<Car> OnMoveExit;
    public Action<Car, bool> OnCompleteLoading;
    public Action<ContainerColor> OnColorSet;

    public void Init(ParkingPlace place, ContainerColor needColor)
    {
        _obstacleChecker.Init(this);
        NeedColor = needColor;
        MoveToParking(place);
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

            _disposeMove.Clear();
            return;
        }

        if (IsWayFree && HasWrongsContainers == false)
        {
            transform.position += (target - transform.position).normalized * speed * Time.deltaTime;
        }
    }

    public void MoveToParking(ParkingPlace place)
    {
        _disposeMove.Clear();
        place.IsAwate = true;

        if (CurrentPlace != null)
        {
            CurrentPlace.SetCar(null);
        }
        CurrentPlace = place;
        Observable.EveryUpdate().Subscribe(_ =>
        {
            MoveTo(place.transform, () => OnParking(place));
        }).AddTo(_disposeMove);
    }
    private void OnParking(ParkingPlace place)
    {
        _disposeMove.Clear();
        place.SetCar(this);
    }

    public void CopmpleteLoading(bool isSuccess)
    {
        IsComplete = true;
        _colorSignal.gameObject.SetActive(false);
        OnCompleteLoading?.Invoke(this, isSuccess);
    }

    public void MoveToExit()
    {
        _disposeMove.Clear();
        CurrentPlace.SetCar(null);

        Transform exit = ParkingManager.Instance.GetExitPoint();
        Observable.EveryUpdate().Subscribe(_ =>
        {
            MoveTo(exit, OnExit);
        }).AddTo(_disposeMove);
    }
    private void OnExit()
    {
        _disposeMove.Clear();

        OnMoveExit?.Invoke(this);
        Destroy(gameObject);
    }
    private void SetColorSignal(ContainerColor color)
    {
        Material material = LevelManager.Instance.GetSignalMaterial(color);
        _colorSignal.sharedMaterial = material;
    }

    public void OnContainerCrush()
    {
        var constructor = LevelManager.Instance;
        bool isHasAvailibleColorContainer = constructor.HasAvailibleColor(NeedColor);

        if (isHasAvailibleColorContainer == false) // если ожидаемого цвета контейнеров больше нет - получаем новый
        {
            if (constructor.HasAvailibleColors() == true && constructor.HasAvailibleContainers() == true)
            {
                NeedColor = constructor.GetRandomAvailibleContainerColor();
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
    }

    public void AddWrongContainer(Container container)
    {
        _wrongContainers.Add(container);
    }

    public void RemoveWrongContainer(Container container)
    {
        _wrongContainers.Remove(container);
    }
}

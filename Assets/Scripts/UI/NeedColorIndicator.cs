using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ColorManager;

public class NeedColorIndicator : MonoBehaviour
{
    [SerializeField] private Image _indicatorImage;
    [SerializeField] private GameObject _doneIndicator;
    [SerializeField] private CanvasGroup _canvasGroup;

    public Car Car { get; private set; }
    private void Awake()
    {
        _canvasGroup.alpha = 0;
    }
    public void Init(Car car)
    {
        _doneIndicator.SetActive(false);
        Car = car;

        Car.OnCompleteLoading += OnCompleteLoading;
        Car.OnMoveExit += OnCarMoveExit;
        Car.OnColorSet += SetNeedColor;

        SetNeedColor(Car.NeedColor);
        _canvasGroup.alpha = 1;
    }

    private void SetNeedColor(ContainerColor color)
    {
        var newColor = LevelManager.Instance.GetConformigSignalColor(color);
        _indicatorImage.color = newColor;
    }

    private void OnCarMoveExit(Car car = null)
    {
        Destroy(gameObject);
    }

    private void OnCompleteLoading(Car car, bool isSucces)
    {
        if (isSucces)
            _doneIndicator.SetActive(true);
        else
            _indicatorImage.color = Color.gray;
    }

    private void OnDestroy()
    {
        Car.OnCompleteLoading -= OnCompleteLoading;
        Car.OnMoveExit -= OnCarMoveExit;
        Car.OnColorSet -= SetNeedColor;
    }
}

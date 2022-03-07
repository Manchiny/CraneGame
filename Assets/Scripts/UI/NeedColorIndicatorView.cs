using UnityEngine;
using UnityEngine.UI;
using static ColorManager;

public class NeedColorIndicatorView : MonoBehaviour
{
    [SerializeField] private Image _indicatorImage;
    [SerializeField] private Image _doneIndicator;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private GameObject _notify;
    public Car Car { get; private set; }
    private Color _needColor;
    private void Awake()
    {
        _canvasGroup.alpha = 0;
    }
    public void Init(Car car)
    {
        _notify.SetActive(false);
        _doneIndicator.gameObject.SetActive(false);
        Car = car;

        Car.OnCompleteLoading += OnCompleteLoading;
        Car.OnMoveExit += OnCarMoveExit;
        Car.OnColorSet += SetNeedColor;
        Car.CheckNotify += ShowHideNotify;

        SetNeedColor(Car.NeedColor);
        _canvasGroup.alpha = 1;
    }

    private void SetNeedColor(ContainerColor color)
    {
        var newColor = Game.LevelManager.GetConformigSignalColor(color);
        _needColor = newColor;
        _indicatorImage.color = newColor;
    }

    private void OnCarMoveExit(Car car = null)
    {
        Destroy(gameObject);
    }

    private void OnCompleteLoading(Car car, bool isSucces)
    {
        if (isSucces)
        {
            _doneIndicator.color = _needColor;
            _doneIndicator.gameObject.SetActive(true);
        }  
        else
            _indicatorImage.color = Color.gray;
    }

    private void OnDestroy()
    {
        if (Car != null)
        {
            Car.OnCompleteLoading -= OnCompleteLoading;
            Car.OnMoveExit -= OnCarMoveExit;
            Car.OnColorSet -= SetNeedColor;
            Car.CheckNotify -= ShowHideNotify;
        }
    }

    private void ShowHideNotify()
    {
        bool isNeed = Car.IsWayFree == false || Car.HasWrongsContainers;
        _notify.SetActive(isNeed);
    }
}

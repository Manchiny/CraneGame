using UnityEngine;
using UnityEngine.UI;
using static ColorManager;

public class NeedColorIndicatorView : MonoBehaviour
{
    [SerializeField] private Image _indicatorImage;
    [SerializeField] private Image _doneIndicator;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private GameObject _notify;
    
    private Color _needColor;
    public Car Car { get; private set; }

    private void Awake()
    {
        _canvasGroup.alpha = 0;
    }

    private void OnDestroy()
    {
        if (Car != null)
        {
            Car.LoadingCompleted -= OnCompleteLoading;
            Car.Exited -= OnCarMoveExit;
            Car.ColorSeted -= SetNeedColor;
            Car.FreeWayChanged -= ShowHideNotify;
        }
    }

    public void Init(Car car)
    {
        _notify.SetActive(false);
        _doneIndicator.gameObject.SetActive(false);
        Car = car;

        Car.LoadingCompleted += OnCompleteLoading;
        Car.Exited += OnCarMoveExit;
        Car.ColorSeted += SetNeedColor;
        Car.FreeWayChanged += ShowHideNotify;

        SetNeedColor(Car.NeedColor);
        _canvasGroup.alpha = 1;
    }

    private void SetNeedColor(ContainerColor color)
    {
        var newColor = Game.Level.GetConformigSignalColor(color);
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

    private void ShowHideNotify()
    {
        _notify.SetActive(Car.CanMove == false);
    }
}

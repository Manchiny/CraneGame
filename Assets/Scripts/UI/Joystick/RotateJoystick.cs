using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RotateJoystick : Joystick
{
    [SerializeField] private Button _detachBtn;

    private Magnit _magnit;
    public override void Init(Crane crane)
    {
        base.Init(crane);
        _magnit = crane.Magnit;

        _detachBtn.onClick.RemoveAllListeners();
        _detachBtn.onClick.AddListener(OnButtonDetachClick);
    }

    private CompositeDisposable _dispose = new CompositeDisposable();
    public override void OnPointerDown(PointerEventData eventData)
    {
        Observable.EveryUpdate().Subscribe(_ =>
        {
            Controller.RotateContainer(Horizontal());
        }).AddTo(_dispose);

        base.OnPointerDown(eventData);
    }
    public override void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_joystickBg.rectTransform, eventData.position, eventData.pressEventCamera, out pos))
        {
            pos.x = (pos.x / _joystickBg.rectTransform.rect.size.x);
            //pos.y = (pos.y / _joystickBg.rectTransform.rect.size.y);
        }
        InputVector = new Vector2(pos.x * 2, 0);
        InputVector = (InputVector.magnitude > 1.0f) ? InputVector.normalized : InputVector;

        _joystick.rectTransform.localPosition = new Vector2(InputVector.x * (_joystickBg.rectTransform.rect.size.x / 2), InputVector.y);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        _dispose.Clear();
    }
    private void OnButtonDetachClick()
    {
        _magnit.Free();
    }
}

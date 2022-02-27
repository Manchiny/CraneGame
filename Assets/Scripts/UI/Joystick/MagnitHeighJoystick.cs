using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class MagnitHeighJoystick : Joystick
{
    private CompositeDisposable _cableMoveDispose = new CompositeDisposable();
    public override void OnPointerDown(PointerEventData eventData)
    {
        _controller.JointRB.WakeUp();
        _controller.MagnitRB.WakeUp();
        Observable.EveryUpdate().Subscribe(_ =>
        {
            _controller.SetMagnitHeigh(Vertical());
        }).AddTo(_cableMoveDispose);

        base.OnPointerDown(eventData);
    }
    public override void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_joystickBg.rectTransform, eventData.position, eventData.pressEventCamera, out pos))
        {
        //    pos.x = (pos.x / _joystickBg.rectTransform.rect.size.x);
            pos.y = (pos.y / _joystickBg.rectTransform.rect.size.y);
        }
        inputVector = new Vector2(0, pos.y * 2);
        inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

        _joystick.rectTransform.localPosition = new Vector2(inputVector.x , inputVector.y * (_joystickBg.rectTransform.rect.size.y / 2));
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        _cableMoveDispose.Clear();
    }
}

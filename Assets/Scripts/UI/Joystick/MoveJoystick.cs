using UnityEngine;
using UniRx;
using UnityEngine.EventSystems;

public class MoveJoystick : Joystick
{
    private CompositeDisposable _craneMoveDispose = new CompositeDisposable();

    public override void OnPointerDown(PointerEventData eventData)
    {
        Observable.EveryUpdate().Subscribe(_ =>
        {
            _controller.MoveSide(Horizontal());
            _controller.ArrowMove(Vertical());
        }).AddTo(_craneMoveDispose);

        base.OnPointerDown(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_joystickBg.rectTransform, eventData.position, eventData.pressEventCamera, out pos))
        {
            pos.x = (pos.x / _joystickBg.rectTransform.rect.size.x);
            pos.y = (pos.y / _joystickBg.rectTransform.rect.size.y);
        }
        inputVector = new Vector2(pos.x * 2, pos.y * 2);
        inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

        _joystick.rectTransform.localPosition = new Vector2(inputVector.x * (_joystickBg.rectTransform.rect.size.x / 2), inputVector.y * (_joystickBg.rectTransform.rect.size.y / 2));
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        _craneMoveDispose.Clear();
    }
}

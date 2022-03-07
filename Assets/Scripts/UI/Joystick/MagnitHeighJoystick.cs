using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

public class MagnitHeighJoystick : Joystick
{
    private CompositeDisposable _cableMoveDispose;
    private Crane _crane;
    public override void Init(Crane crane)
    {
        base.Init(crane);
        _cableMoveDispose?.Clear();
        _cableMoveDispose = new CompositeDisposable();

        _crane = crane;
       // crane.OnDawnMoveFreeze += OnDownFreeze;
    }
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
        if (_controller.Magnit.IsFreezed)
        {
            OnPointerUp(eventData);
            return;
        }
            
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_joystickBg.rectTransform, eventData.position, eventData.pressEventCamera, out pos))
        {
            pos.y = (pos.y / _joystickBg.rectTransform.rect.size.y);
        }
        _inputVector = new Vector2(0, pos.y * 2);
        _inputVector = (_inputVector.magnitude > 1.0f) ? _inputVector.normalized : _inputVector;

        _joystick.rectTransform.localPosition = new Vector2(_inputVector.x , _inputVector.y * (_joystickBg.rectTransform.rect.size.y / 2));
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        _cableMoveDispose.Clear();
    }

    private void OnDownFreeze()
    {
        _cableMoveDispose.Clear();
    }
}

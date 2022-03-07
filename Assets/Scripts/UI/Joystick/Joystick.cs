using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] protected Image _joystickBg;
    [SerializeField] protected Image _joystick;
    
    protected CraneController _controller;
    protected Vector2 _inputVector;

    private CompositeDisposable _dispose;
    public virtual void Init(Crane crane)
    {
        _dispose?.Clear();
        _dispose = new CompositeDisposable();

        _controller = crane.Controller;
        _joystick.transform.localPosition = Vector3.zero;
    }
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }
    public abstract void OnDrag(PointerEventData eventData);
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        _inputVector = Vector2.zero;
        _joystick.rectTransform.anchoredPosition = Vector2.zero;
        _dispose.Clear();
    }

    public float Horizontal()
    {
        if (_inputVector.x != 0)
            return _inputVector.x;
        else
            return Input.GetAxisRaw("Horizontal");
    }
    public float Vertical()
    {
        if (_inputVector.y != 0)
            return _inputVector.y;
        else
            return Input.GetAxisRaw("Vertical");
    }
}


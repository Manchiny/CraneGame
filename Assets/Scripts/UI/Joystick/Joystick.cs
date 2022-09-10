using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] protected Image _joystickBg;
    [SerializeField] protected Image _joystick;
    
    protected CraneController Controller;
    protected Vector2 InputVector;

    private CompositeDisposable _dispose;

    public virtual void Init(Crane crane)
    {
        _dispose?.Clear();
        _dispose = new CompositeDisposable();

        Controller = crane.Controller;
        _joystick.transform.localPosition = Vector3.zero;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public virtual void OnPointerUp(PointerEventData eventData)
    {
        InputVector = Vector2.zero;
        _joystick.rectTransform.anchoredPosition = Vector2.zero;
        _dispose.Clear();
    }
    public abstract void OnDrag(PointerEventData eventData);

    public float Horizontal()
    {
        if (InputVector.x != 0)
            return InputVector.x;
        else
            return Input.GetAxisRaw("Horizontal");
    }

    public float Vertical()
    {
        if (InputVector.y != 0)
            return InputVector.y;
        else
            return Input.GetAxisRaw("Vertical");
    }
}


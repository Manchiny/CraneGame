using System;
using UniRx;
using UnityEngine;

public class CraneController : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _arrowSpeed = 2f;
    [SerializeField] private float _cableSpeed = 0.02f;
    [SerializeField] private float _magnitRotationSpeed = 75f;
    [SerializeField] private Vector3 _craneMoveDirection = new Vector3(1, 0, 0);

    [Space]
    [SerializeField] private Transform _arrow1;
    [SerializeField] private Transform _arrow2;
    [SerializeField] private Transform _arrow3;

    [Space]
    [SerializeField] private ConfigurableJoint _joint;
    [SerializeField] private float _minMagnitY = 0f;
    [SerializeField] private float _maxMagnitY = 13f;

    private const float MaxArrowDistance = 13f;
    private const float MinArrowDistance = 0.01f;
    private const float MinFirstArrowDistance = -3f;

    private const float MaxSideValue = 70f;

    private Vector3 _arrowMoveDirection = new Vector3(0, 0, 1);

    private Crane _crane;
    private Magnit _magnit;

    private Vector3 _cranStartPosition;
    private IDisposable _forceDispose;
    private bool _isHeightBlocked;

    public Magnit Magnit  => _magnit;
    public Rigidbody JointRB { get; private set; }
    public Rigidbody MagnitRB { get; private set; }

    public void Init(Crane crane)
    {
        JointRB = _joint.GetComponent<Rigidbody>();

        _crane = crane;
        _magnit = _crane.Magnit;
        MagnitRB = _magnit.GetComponent<Rigidbody>();

        if (_cranStartPosition == null)
            _cranStartPosition = crane.transform.position;
        else
            crane.transform.position = _cranStartPosition;

        _joint.autoConfigureConnectedAnchor = false;
        ResetCrane();
    }

    public void ArrowMove(float vertical)
    {
        if (vertical >= 0)
            ArrowExtend(vertical);
        else if (vertical <= 0)
            ArrowShorten(vertical);
    }

    public void SetMagnitHeigh(float factor)
    {
        if (_isHeightBlocked)
            return;

        if (factor <= 0)
            MagnitDown(factor);
        else if (factor >= 0)
            MagnitUp(factor);
    }

    public void MoveSide(float horizontal)
    {
        bool canMove = horizontal < 0 ? transform.position.z < MaxSideValue : transform.position.z > -MaxSideValue;

        if (canMove)
            transform.Translate(_craneMoveDirection * _speed * horizontal * Time.deltaTime);
    }

    public void RotateContainer(float factor = 1)
    {
        if (factor != 0)
            _magnit.Rotate(_magnitRotationSpeed * factor);
    }

    public void StartForceMagnitUp(float deltaHeight, Action onComplete)
    {
        _isHeightBlocked = true;

        float height = (_joint.connectedAnchor.y - deltaHeight > _minMagnitY)
            ? _joint.connectedAnchor.y - deltaHeight
            : _minMagnitY;

        _forceDispose = ForceSetHeightUp(height, onComplete);
    }

    private void ResetCrane()
    {
        _magnit.Init(_crane);
        _isHeightBlocked = false;

        _arrow3.transform.localPosition = _arrowMoveDirection * 0.1f;
        _arrow2.transform.localPosition = _arrowMoveDirection * 0.1f;
        _arrow1.transform.localPosition = new Vector3(0, 0, 0);

        JointRB.WakeUp();
        MagnitRB.WakeUp();

        _joint.connectedAnchor = new Vector3(0, 3, 0);
    }

    private void ArrowExtend(float factor = 1)
    {
        if (_arrow2.localPosition.z < MaxArrowDistance)
            _arrow2.Translate(_arrowMoveDirection * _arrowSpeed * factor * Time.deltaTime);
        else if (_arrow3.localPosition.z < MaxArrowDistance)
            _arrow3.Translate(_arrowMoveDirection * _arrowSpeed * factor * Time.deltaTime);
        else if (_arrow1.localPosition.z < MaxArrowDistance / 2f)
            _arrow1.Translate(_arrowMoveDirection * _arrowSpeed * factor * Time.deltaTime);
    }

    private void ArrowShorten(float factor = -1)
    {
        if (_arrow3.localPosition.z > MinArrowDistance)
            _arrow3.Translate(_arrowMoveDirection * _arrowSpeed * factor * Time.deltaTime);
        else if (_arrow2.localPosition.z > MinArrowDistance)
            _arrow2.Translate(_arrowMoveDirection * _arrowSpeed * factor * Time.deltaTime);
        else if (_arrow1.localPosition.z > MinFirstArrowDistance)
            _arrow1.Translate(_arrowMoveDirection * _arrowSpeed * factor * Time.deltaTime);
    }

    private void MagnitUp(float factor = 1)
    {
        if (_joint.connectedAnchor.y > _minMagnitY)
        {
            JointRB.WakeUp();
            MagnitRB.WakeUp();

            Vector3 moveVector = new Vector3(0, _cableSpeed, 0) * Time.deltaTime * factor;
            _joint.connectedAnchor -= moveVector;
        }
    }

    private void MagnitDown(float factor = -1)
    {
        if (_joint.connectedAnchor.y < _maxMagnitY && _magnit.CanMoveDown)
        {
            JointRB.WakeUp();
            MagnitRB.WakeUp();

            Vector3 moveVector = new Vector3(0, _cableSpeed, 0) * Time.deltaTime * factor;
            _joint.connectedAnchor -= moveVector;
        }
    }

    private void ForceMagnitUp(float needHeight, Action onComplete)
    {
        if (_joint.connectedAnchor.y > needHeight)
            MagnitUp();
        else
        {
            _forceDispose.Dispose();
            onComplete.Invoke();
            _isHeightBlocked = false;
        }
    }

    private IDisposable ForceSetHeightUp(float needHeight, Action onComplete)
    {
        return Observable.EveryUpdate().Subscribe(_ =>
        {
            ForceMagnitUp(needHeight, onComplete);
        }).AddTo(this);
    }
}

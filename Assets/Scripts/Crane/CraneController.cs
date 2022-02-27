using UnityEngine;
using UnityEngine.UI;

public class CraneController : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    [SerializeField] private float _arrowSpeed = 2f;
    [SerializeField] private float _cableSpeed = 0.02f;
    [SerializeField] private float _magnitRotationSpeed = 75f;
    [SerializeField] private Vector3 _moveVector = new Vector3(0, 0, 1);

    [Space]
    [SerializeField] private Transform _arrow2;
    [SerializeField] private Transform _arrow3;

    [Space]
    [SerializeField] private ConfigurableJoint _joint;
    [SerializeField] private float _minMagnitY = 0f;
    [SerializeField] private float _maxMagnitY = 13f;

    [Space]
    [SerializeField] private Button _detachBtn;

    private Crane _crane;
    private Magnit _magnit;
    public Magnit Magnit { get => _magnit; }
    public Rigidbody JointRB { get; private set; }
    public Rigidbody MagnitRB { get; private set; }
    private void Awake()
    {
        _crane = GetComponent<Crane>();
        _magnit = _crane.Magnit;
        JointRB = _joint.GetComponent<Rigidbody>();
        MagnitRB = _magnit.GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _joint.autoConfigureConnectedAnchor = false;
        _detachBtn.onClick.AddListener(OnButtonDetachClick);
    }
    public void ArrowMove(float vertical)
    {
        if (vertical >= 0)
            ArrowExtend(vertical);
        else if (vertical <= 0)
            ArrowShorten(vertical);
    }
    private void ArrowExtend(float factor = 1)
    {
        if (_arrow2.localPosition.x < 0.99f)
        {
            _arrow2.Translate(new Vector3(1, 0, 0) * _arrowSpeed * factor * Time.deltaTime);
        }
        else if (_arrow3.localPosition.x < 0.99f)
        {
            _arrow3.Translate(new Vector3(1, 0, 0) * _arrowSpeed * factor * Time.deltaTime);
        }
    }

    private void ArrowShorten(float factor = -1)
    {
        if (_arrow3.localPosition.x > 0.01f)
        {
            _arrow3.Translate(new Vector3(1, 0, 0) * _arrowSpeed * factor * Time.deltaTime);
        }
        else if (_arrow2.localPosition.x > 0.01f)
        {
            _arrow2.Translate(new Vector3(1, 0, 0) * _arrowSpeed * factor * Time.deltaTime);
        }
    }


    public void SetMagnitHeigh(float factor)
    {
        JointRB.WakeUp();
        MagnitRB.WakeUp();
        if (factor <= 0)
            MagnitDown(factor);
        else if (factor >= 0)
            MagnitUp(factor);
    }
    private void MagnitUp(float factor = -1)
    {
        Vector3 moveVector = new Vector3(0, _cableSpeed, 0) * Time.deltaTime * factor;

        if (_joint.connectedAnchor.y > _minMagnitY)
            _joint.connectedAnchor -= moveVector;
    }

    private void MagnitDown(float factor = 1)
    {
        Vector3 moveVector = new Vector3(0, _cableSpeed, 0) * Time.deltaTime * factor;

        if (_joint.connectedAnchor.y < _maxMagnitY && _crane.IsDownMoveFreeze == false)
            _joint.connectedAnchor -= moveVector;
    }

    public void MoveSide(float horizontal)
    {
        transform.Translate(-_moveVector * _speed * horizontal * Time.deltaTime);
    }

    public void RotateContainer(float factor = 1)
    {
        if(factor != 0)
        {
            _magnit.Rotate(_magnitRotationSpeed * factor);
        }          
    }

    private void OnButtonDetachClick()
    {
        _magnit.Free();
    }
}

using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using static ColorDataBase;

[RequireComponent(typeof(BoxCollider))]
public class Container : MonoBehaviour
{
    [SerializeField] private List<ConteinerCarChecker> _carCheckers;
    [SerializeField] private MagnitChecker _magnitChecker;
    [SerializeField] private MeshRenderer _body;
    [SerializeField] private ExcessContainerChecker _excessContainerChecker;
    [Space]
    [SerializeField] private float _containerHeight = 3f;

    private const float MaxAngleXToFlip = 15f;

    private Ship _ship;
    private CarPlatform _carPlatform;

    private bool _onCar;
    private bool _isMagnited;

    private bool _isCrushed;
    private IDisposable _checkFlipDispose;

#region RigidbodySettings

    private float _mass;
    private float _drag;
    private bool _useGravity;
    private bool _isKinematic;
    private RigidbodyInterpolation _interpolation;

#endregion

    public ContainerSound Sound { get; private set; }
    public ContainerColor ContainerColor { get; private set; }
    public Rigidbody Rigidbody { get; private set; }

    public float ContainerHeight => _containerHeight;

    public bool OnCar
    {
        get => _onCar;
        private set
        {
            _onCar = value;

            if (value == true)
                CheckForCorrectCollorAndPosition();
            else
            {
                RemoveContainerAsWrong(this);
                _carPlatform = null;
            }
        }
    }

    public bool IsMagnited
    {
        get => _isMagnited;
        private set
        {
            if (value != _isMagnited && value == true)
                Sound.PlayHitSound();

            _isMagnited = value;

            if (value == true)
                OnCar = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.TryGetComponent(out Magnit magnit) && magnit.IsFreezed == false) || (magnit == null && collision.gameObject.GetComponent<INoiseless>() == null))
            Sound.PlayHitSound();

        if (collision.gameObject.TryGetComponent(out _carPlatform))
            OnCar = true;

        if (IsMagnited == false)
            StartCheckFlip();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsMagnited == false)
        {
            if (other.gameObject.TryGetComponent(out MapBorder border))
                Crush();
            else if (other.gameObject.TryGetComponent(out Water water))
                OnWaterEnter(water);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<CarPlatform>() == true)
        {
            OnCar = false;
            _carPlatform = null;
        }
    }

    private void OnDestroy()
    {
        if (_checkFlipDispose != null)
            _checkFlipDispose.Dispose();

        Game.LevelLoader.LevelExited -= OnExitLevel;
    }

    public void Init(Ship ship, ContainerColor color)
    {
        Game.LevelLoader.LevelExited += OnExitLevel;
        Rigidbody = GetComponent<Rigidbody>();

        _ship = ship;
        SetConteinerColor(color);
        _excessContainerChecker.gameObject.SetActive(false);
        SaveRiggedbody();

        if (Sound == null)
            Sound = gameObject.AddComponent<ContainerSound>();

        Sound.Init();
    }

    public void SetConteinerColor(ContainerColor color)
    {
        if (Game.ColorDatabase.TryGetConteinerMaterial(color, out Material material))
        {
            _body.sharedMaterial = material;
            ContainerColor = color;
        }
    }

    public void Crush()
    {
        if (_isCrushed)
            return;

        _isCrushed = true;
        _magnitChecker.gameObject.SetActive(false);
        _ship?.OnContainerCrush(this);
        transform.SetParent(null);
    }

    public void AddContainerAsWrong(Container container)
    {
        _carPlatform.Car.AddWrongContainer(container);
    }

    public void RemoveContainerAsWrong(Container container)
    {
        if (_carPlatform != null)
            _carPlatform.Car.RemoveWrongContainer(container);
    }

    public void SetFree()
    {
        transform.parent = null;
        AddRigidbody();
        IsMagnited = false;
    }

    public void OnMagniteze()
    {
        if (Rigidbody != null)
            Destroy(Rigidbody);

        IsMagnited = true;
    }

    private void AddRigidbody()
    {
        Rigidbody = gameObject.AddComponent<Rigidbody>();
        Rigidbody.mass = _mass;
        Rigidbody.drag = _drag;
        Rigidbody.interpolation = _interpolation;
        Rigidbody.useGravity = _useGravity;
        Rigidbody.isKinematic = _isKinematic;
    }

    private void SaveRiggedbody()
    {
        _mass = Rigidbody.mass;
        _drag = Rigidbody.drag;
        _interpolation = Rigidbody.interpolation;
        _useGravity = Rigidbody.useGravity;
        _isKinematic = Rigidbody.isKinematic;
    }

    private void OnExitLevel()
    {
        _checkFlipDispose.Dispose();
        Destroy(gameObject);
    }

    private void OnWaterEnter(Water water)
    {
        var position = transform.position;
        position.y = water.transform.position.y;

        water.PlaySplashesEffect(transform.position);
        Sound.PlaySplashSound();

        Crush();
    }

    private bool CheckForCorrectCollorAndPosition()
    {
        if (ContainerColor != _carPlatform.Car.NeedColor)
        {
            Debug.Log("Color not correct");
            AddContainerAsWrong(this);
            return false;
        }

        foreach (var checker in _carCheckers)
        {
            if (checker.IsOnCar == false)
            {
                Debug.Log("Position not correct");
                AddContainerAsWrong(this);
                return false;
            }
        }

        Debug.Log("Succes!");
        RemoveContainerAsWrong(this);
        CompleteLoadig(true);

        return true;
    }

    private void CompleteLoadig(bool isSucces)
    {
        Destroy(Rigidbody);
        _magnitChecker.gameObject.SetActive(false);
        _ship.RemoveContainer(this);

        transform.parent = _carPlatform.Car.transform;
        _carPlatform.Car.CopmpleteLoading(isSucces);

        _excessContainerChecker.gameObject.SetActive(true);
        _excessContainerChecker.Init(this);
    }

    private void StartCheckFlip()
    {
        if (Rigidbody == null)
            return;

        _checkFlipDispose = CheckForFlip(Rigidbody);
    }

    private IDisposable CheckForFlip(Rigidbody rb)
    {
        return Observable.EveryUpdate().Subscribe(_ =>
        {
            if (rb == null)
                return;

            if (_isCrushed == false && rb.velocity == Vector3.zero)
            {
                _checkFlipDispose.Dispose();

                var rotation = transform.rotation;

                if ((Mathf.Abs(180 - Mathf.Abs(rotation.eulerAngles.x)) < MaxAngleXToFlip) || (Mathf.Abs(180 - Mathf.Abs(rotation.eulerAngles.z)) < MaxAngleXToFlip))
                {
                    Crush();
                    Debug.Log("CONTAINER FLIPED!!!");
                }
            }
        }).AddTo(this);
    }
}


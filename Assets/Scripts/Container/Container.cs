using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityStandardAssets.Water;
using static ColorManager;

[RequireComponent(typeof(BoxCollider))]
public class Container : MonoBehaviour
{
    [SerializeField] private List<ConteinerCarChecker> _carCheckers;
    [SerializeField] private MagnitChecker _magnitChecker;
    [SerializeField] private MeshRenderer _body;
    [SerializeField] private ExcessContainerChecker _excessContainerChecker;
    [Space]
    [SerializeField] private float _containerHeight = 3f;

    private Ship _ship;
    private CarPlatform _carPlatform;

    private bool _onCar;
    private bool _isMagnitize;

    private bool _isCrushed;
    private IDisposable _flipCheckDispose;

    private float _mass;
    private float _drag;
    private bool _useGravity;
    private bool _isKinematic;
    private RigidbodyInterpolation _interpolation;
    public ContainerColor ContainerColor { get; private set; }
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

    public bool IsMagnitize
    {
        get => _isMagnitize;
        private set
        {
            _isMagnitize = value;
            if (value == true)
                OnCar = false;
        }
    }

    private void OnDestroy()
    {
        Game.LevelLoader.LevelExited -= OnExitLevel;
    }

    public void Init(Ship ship, ContainerColor color)
    {
        Game.LevelLoader.LevelExited += OnExitLevel;
        _ship = ship;
        SetConteinerColor(color);
        _excessContainerChecker.gameObject.SetActive(false);
        SaveRiggedbody();
    }

    public void AddRigidbody()
    {
        var rb = gameObject.AddComponent<Rigidbody>();
        rb.mass = _mass;
        rb.drag = _drag;
        rb.interpolation = _interpolation;
        rb.useGravity = _useGravity;
        rb.isKinematic = _isKinematic;
    }

    public void SetMagnitizeStatus(bool isMagnitized)
    {
        IsMagnitize = isMagnitized;
    }

    public void SetConteinerColor(ContainerColor color)
    {
        Material material = null;

        if (Game.ColorManager.ContainerMaterials.TryGetValue(color, out material))
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

    private void SaveRiggedbody()
    {
        var rb = GetComponent<Rigidbody>();
        _mass = rb.mass;
        _drag = rb.drag;
        _interpolation = rb.interpolation;
        _useGravity = rb.useGravity;
        _isKinematic = rb.isKinematic;
    }

    private void OnExitLevel()
    {
        _flipCheckDispose.Dispose();
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<CarPlatform>() == true)
        {
            _carPlatform = collision.gameObject.GetComponent<CarPlatform>();
            OnCar = true;
        }

        if (IsMagnitize == false)
        {
            StartCheckFlip();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.TryGetComponent(out MapBorder border) || other.gameObject.TryGetComponent(out Water water)) && IsMagnitize == false)
            Crush();
    }

    private void OnCollisionExit(Collision collision)
    {

        if (collision.gameObject.GetComponent<CarPlatform>() == true)
        {
            OnCar = false;
            _carPlatform = null;
        }
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
        Destroy(GetComponent<Rigidbody>());
        _magnitChecker.gameObject.SetActive(false);
        _ship.RemoveContainer(this);

        transform.parent = _carPlatform.Car.transform;
        _carPlatform.Car.CopmpleteLoading(isSucces);

        _excessContainerChecker.gameObject.SetActive(true);
        _excessContainerChecker.Init(this);
    }

    private void StartCheckFlip()
    {
        var rb = GetComponent<Rigidbody>();
        if (rb == null)
            return;

        _flipCheckDispose = CheckForFlip(rb);
    }

    private IDisposable CheckForFlip(Rigidbody rb)
    {
        return Observable.EveryUpdate().Subscribe(_ =>
        {
            if (rb == null)
            {
                _flipCheckDispose.Dispose();
                return;
            }
            if (_isCrushed == false && rb.velocity == Vector3.zero)
            {
                _flipCheckDispose.Dispose();

                var rotation = transform.rotation;
                if (Mathf.Abs(180 - Mathf.Abs(rotation.x)) < 15)
                {
                    Crush();
                    Debug.Log("CONTAINER FLIPED!!!");
                }
            }
        }).AddTo(this);
    }
}


using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using static ColorManager;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(AudioSource))]
public class Container : MonoBehaviour
{
    [SerializeField] private List<ConteinerCarChecker> _carCheckers;
    [SerializeField] private MagnitChecker _magnitChecker;
    [SerializeField] private MeshRenderer _body;
    [SerializeField] private ExcessContainerChecker _excessContainerChecker;
    [Space]
    [SerializeField] private float _containerHeight = 3f;
   
    private const float MuteSoundAfterInitSeconds = 2f;
    private const float MaxAngleXToFlip = 15f;

    private const float WaterSoundDuration = 0.4f;
    private const float HitSoundDuration = 0.4f;
    private bool _isWaterSoundPlaying;
    private bool _canPlayHitSound;

    private Ship _ship;
    private CarPlatform _carPlatform;

    private bool _onCar;
    private bool _isMagnited;

    private bool _isCrushed;
    private IDisposable _checkFlipDispose;

    private Rigidbody _rigidbody;
    private float _mass;
    private float _drag;
    private bool _useGravity;
    private bool _isKinematic;
    private RigidbodyInterpolation _interpolation;

    private bool _isSoundInited;

    public AudioSource AudioSource { get; private set; }
    public ContainerColor ContainerColor { get; private set; }
    public float ContainerHeight => _containerHeight;
    public Rigidbody Rigidbody => _rigidbody;

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
                Game.Sound.PlayHitSound(AudioSource);

            _isMagnited = value;

            if (value == true)
                OnCar = false;
        }
    }

    private void Start()
    {
        AudioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isSoundInited && _isWaterSoundPlaying == false 
            && ((collision.gameObject.TryGetComponent(out Magnit magnit) == true && magnit.IsFreezed == false) 
            || (magnit == null && collision.gameObject.GetComponent<INoiseless>() == null)))
        {
            PlayHitSound();
        }

        if (collision.gameObject.GetComponent<CarPlatform>() == true)
        {
            _carPlatform = collision.gameObject.GetComponent<CarPlatform>();
            OnCar = true;
        }

        if (IsMagnited == false)
        {
            StartCheckFlip();
        }
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
        _rigidbody = GetComponent<Rigidbody>();

        _ship = ship;
        SetConteinerColor(color);
        _excessContainerChecker.gameObject.SetActive(false);
        SaveRiggedbody();

        Utils.WaitSeconds(MuteSoundAfterInitSeconds)
            .Then(() =>
            {
                _isSoundInited = true;
                _canPlayHitSound = true;
            });
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

    public void PlayHitSound()
    {
        if(_canPlayHitSound)
        {
            _canPlayHitSound = false;
            Game.Sound.PlayHitSound(AudioSource);

            Utils.WaitSeconds(HitSoundDuration)
                .Then(() => _canPlayHitSound = true);
        }
    }

    private void AddRigidbody()
    {
        _rigidbody = gameObject.AddComponent<Rigidbody>();
        _rigidbody.mass = _mass;
        _rigidbody.drag = _drag;
        _rigidbody.interpolation = _interpolation;
        _rigidbody.useGravity = _useGravity;
        _rigidbody.isKinematic = _isKinematic;
    }

    private void SaveRiggedbody()
    {
        _mass = _rigidbody.mass;
        _drag = _rigidbody.drag;
        _interpolation = _rigidbody.interpolation;
        _useGravity = _rigidbody.useGravity;
        _isKinematic = _rigidbody.isKinematic;
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
        Game.Sound.PlaySplashSound(AudioSource);

        _isWaterSoundPlaying = true;

        Utils.WaitSeconds(WaterSoundDuration)
            .Then(() => _isWaterSoundPlaying = false);

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
        Destroy(_rigidbody);
        _magnitChecker.gameObject.SetActive(false);
        _ship.RemoveContainer(this);

        transform.parent = _carPlatform.Car.transform;
        _carPlatform.Car.CopmpleteLoading(isSucces);

        _excessContainerChecker.gameObject.SetActive(true);
        _excessContainerChecker.Init(this);
    }

    private void StartCheckFlip()
    {
        if (_rigidbody == null)
            return;

        _checkFlipDispose = CheckForFlip(_rigidbody);
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


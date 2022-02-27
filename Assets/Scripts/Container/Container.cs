using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityStandardAssets.Water;
using static ColorManager;

public class Container : MonoBehaviour
{
    [SerializeField] private List<ConteinerCarChecker> _carCheckers;
    [SerializeField] private MagnitChecker _magnitChecker;
    [SerializeField] private MeshRenderer _body;

    [HideInInspector] public ContainerColor ContainerColor { get; private set; }

    private Ship _ship;
    private CarPlatform _carPlatform;
    private bool _onCar;

    public bool OnCar
    {
        get => _onCar;
        private set
        {
            _onCar = value;

            if (value == true)
                CheckForCorrectCollorAndPosition();
            else
                _carPlatform = null;
        }
    }

    private bool _isMagnitize;
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

    public void Init(Ship ship, ContainerColor color)
    {
        _ship = ship;
        SetConteinerColor(color);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<CarPlatform>() == true)
        {
            _carPlatform = collision.gameObject.GetComponent<CarPlatform>();
            OnCar = true;
            Debug.Log("OnCar ");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Water>() == true)
        {
            OnWaterEnter();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<CarPlatform>() == true)
        {
            OnCar = false;
            _carPlatform = null;
            Debug.Log("OutOfCar");
        }
    }

    public void SetMagnitizeStatus(bool isMagnitized)
    {
        IsMagnitize = isMagnitized;
    }

    private bool CheckForCorrectCollorAndPosition()
    {
        if(ContainerColor != _carPlatform.Car.NeedColor)
        {
            Debug.Log("Color not correct");
            return false;
        }

        foreach (var checker in _carCheckers)
        {
            if (checker.IsOnCar == false)
            {
                Debug.Log("Position not correct");
                return false;
            }
        }

        Debug.Log("Succes!");
        CompleteLoadig(true);
        return true;
    }

    private void CompleteLoadig(bool isSucces)
    {
        DeactivateConteiner();
        Destroy(GetComponent<Rigidbody>());

        transform.parent = _carPlatform.Car.transform;
        _carPlatform.Car.CopmpleteLoading(isSucces);
    }

    public void SetConteinerColor(ContainerColor color)
    {
        Material material = null;
        if (LevelManager.Instance.ColorManager.ContainerMaterials.TryGetValue(color, out material))
        {
            _body.sharedMaterial = material;
            ContainerColor = color;
        }
    }

    private void DeactivateConteiner()
    {
        _magnitChecker.gameObject.SetActive(false);
        _ship.RemoveContainer(this);
    }
    private void OnWaterEnter()
    {
        DeactivateConteiner();
        _ship?.OnContainerCrush(this);

        Debug.Log("OnWater " + ContainerColor);
    }
}


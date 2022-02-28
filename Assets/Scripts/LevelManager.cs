using System.Collections.Generic;
using UnityEngine;
using static ColorManager;
using static LevelConfigs;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private ColorManager _colors;
    [SerializeField] private ShipSpawner _shipSpawner;

    public LevelConfig LevelConfig { get; private set; }
    public Ship CurrentShip { get; private set; }
    private int _nextShipId;
    public ColorManager ColorManager => _colors;
    public Color GetConformigSignalColor(ContainerColor color) => ColorManager.GetConformingColor(color);
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            return;
        }
        Destroy(this);
    }

    public void Init(LevelConfig config)
    {
        _colors.Init();
        _nextShipId = 0;
        LevelConfig = config;
        CreateShipOrLevelComplete();
        ParkingManager.Instance.Init();
    }

    private void CreateShipOrLevelComplete()
    {
        if(_nextShipId < LevelConfig?.ShipConfig?.Length)
        {
            CurrentShip = _shipSpawner.CreateShip(LevelConfig.ShipConfig[_nextShipId]);
            CurrentShip.MoveTo(_shipSpawner.ParkingPoint);
            _nextShipId++;
            CurrentShip.OnContainersEnded += OnContainersEnded;
            ParkingManager.Instance.SubscribeOnCrush(CurrentShip);
        }
        else
        {
            Debug.Log("Level complete");
        }
    }

    public ContainerColor GetRandomAvailibleContainerColor()
    {
        var shipColors = CurrentShip.AvailibleCollors;
        List<ContainerColor> availibleCollors = new List<ContainerColor>();
        foreach (var color in shipColors)
        {
            if (color.Value > 0)
                availibleCollors.Add(color.Key);
        }

        int count = availibleCollors.Count;
        int random = Random.Range(0, count);
        ContainerColor randomColor = availibleCollors[random];
        CurrentShip.RemoveAvailibleColorsCount(randomColor);
        return randomColor;
    }

    public Material GetSignalMaterial(ContainerColor color)
    {
        return ColorManager.GetSignalMaterial(color);
    }

    public bool HasAvailibleContainers()
    {
        return (CurrentShip != null && CurrentShip.Containers.Count > 0);
    }

    public bool HasAvailibleColors()
    {
        var shipColors = CurrentShip.AvailibleCollors;
        foreach (var color in shipColors)
        {
            if (color.Value > 0)
                return true;
        }
        return false;
    }

    public bool HasAvailibleColor(ContainerColor color)
    {
        return CurrentShip.AvailibleCollors[color] >= 0;
    }

    private void OnContainersEnded()
    {
        CurrentShip.OnContainersEnded -= OnContainersEnded;
        CurrentShip.MoveTo(_shipSpawner.ExitPoint);

        CreateShipOrLevelComplete();
    }

}

using RSG;
using System;
using System.Collections.Generic;
using UnityEngine;
using static ColorManager;
using static LevelConfigs;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private ShipSpawner _shipSpawner;
    [SerializeField] private ParkingManager _parkingManager;

    private int _nextShipId;
    private LevelLoader _levelLoader;
    private GameHUDWindow _window;
    
    private int _canCrushContainers;
    private bool _isEnded;

    public int CanCrushContainers
    {
        get => _canCrushContainers;
        set
        {
            _canCrushContainers = value;
        }
    }

    public LevelConfig LevelConfig { get; private set; }
    public Ship CurrentShip { get; private set; }

    public ParkingManager ParkingManager => _parkingManager;

    public void StartLevel(LevelConfig config, LevelLoader loader, GameHUDWindow window)
    {
        _isEnded = false;

        _window = window;

        _nextShipId = 0;
        LevelConfig = config;
        _levelLoader = loader;
        _levelLoader.LoadingCompleted += OnLoadingLevelComplete;

        CanCrushContainers = LevelConfig.MaxContainerCrushes;

        CreateShipOrLevelComplete();
        _parkingManager.Init();
    }    

    public bool OnContainerCrush()
    {
        bool canContinueGame = true;
        CanCrushContainers -= 1;
        _window.UpdateCrushContainersInfo();

        if (CanCrushContainers < 0)
        {
            canContinueGame = false;
            LevelFailed();
        }

        return canContinueGame;
    }

    public Color GetConformigSignalColor(ContainerColor color) => Game.ColorManager.GetConformingColor(color);

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
        int random = UnityEngine.Random.Range(0, count);
        ContainerColor randomColor = availibleCollors[random];
        CurrentShip.RemoveAvailibleColorsCount(randomColor);

        return randomColor;
    }

    public Material GetSignalMaterial(ContainerColor color) => Game.ColorManager.GetSignalMaterial(color);
    public bool HasAvailibleColor(ContainerColor color) => CurrentShip.AvailibleCollors[color] >= 0;
    public bool HasAvailibleContainers => (CurrentShip != null && CurrentShip.ContainersCount > 0);

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

    private void LevelComplete()
    {
        Game.User.SetCurrentLevel();
        LevelCompleteWindow.Show();
    }
   
    public void ExitLevel(Action onComplete)
    {
        _levelLoader.ExitLevel(onComplete);
    }

    private void CreateShipOrLevelComplete()
    {
        if (_isEnded == false)
        {
            if (_nextShipId < LevelConfig?.ShipConfigs?.Length)
            {
                CurrentShip = _shipSpawner.CreateShip(LevelConfig.ShipConfigs[_nextShipId]);
                CurrentShip.MoveTo(_shipSpawner.ParkingPoint);
                _nextShipId++;
                CurrentShip.ContainersEnded += OnContainersEnded;
                ParkingManager.SubscribeOnCrush(CurrentShip);

                _window.SetShipsInfo(_nextShipId, LevelConfig.ShipConfigs.Length);
            }
            else
            {
                LevelComplete();
            }
        }
    }

    private void LevelFailed()
    {
        if (_isEnded)
            return;

        _isEnded = true;
        LevelFailedWindow.Show();
    }

    private void OnContainersEnded()
    {
        CurrentShip.ContainersEnded -= OnContainersEnded;
        CurrentShip.MoveTo(_shipSpawner.ExitPoint);

        CreateShipOrLevelComplete();
    }

    private void OnLoadingLevelComplete()
    {
        _levelLoader.LoadingCompleted -= OnLoadingLevelComplete;
    }
}



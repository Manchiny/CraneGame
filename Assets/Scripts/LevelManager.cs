using RSG;
using System;
using System.Collections.Generic;
using UnityEngine;
using static ColorManager;
using static LevelConfigs;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private ShipSpawner _shipSpawner;
    public LevelConfig LevelConfig { get; private set; }
    public Ship CurrentShip { get; private set; }
    private int _nextShipId;

    private LevelLoader _levelLoader;
    private GameMainWindow _window;
    public int CanCrushContainers { get => _canCrushContainers;
        set
        {
            _canCrushContainers = value;
        }
    }
    private int _canCrushContainers;
    private bool _isEnded;
    public IPromise StartLevel(LevelConfig config, LevelLoader loader, GameMainWindow window)
    {
        Promise promise = new Promise();
        _isEnded = false;

        _window = window;

        _nextShipId = 0;
        LevelConfig = config;
        _levelLoader = loader;
        _levelLoader.OnLoadingComplete += OnLoadingLevelComplete;

        CanCrushContainers = LevelConfig.MaxContainerCrushes;
 
        CreateShipOrLevelComplete()
        .Then(() => ParkingManager.Instance.Init())
        .Then(() => promise.Resolve());

        return promise;
    }

    private IPromise CreateShipOrLevelComplete()
    {
        if (_isEnded == false)
        {
            if (_nextShipId < LevelConfig?.ShipConfigs?.Length)
            {
                CurrentShip = _shipSpawner.CreateShip(LevelConfig.ShipConfigs[_nextShipId]);
                CurrentShip.MoveTo(_shipSpawner.ParkingPoint);
                _nextShipId++;
                CurrentShip.OnContainersEnded += OnContainersEnded;
                ParkingManager.Instance.SubscribeOnCrush(CurrentShip);

                _window.SetShipsInfo(_nextShipId, LevelConfig.ShipConfigs.Length);
            }
            else
            {
                LevelComplete();
            }
        }
        return Promise.Resolved();
    }
    private void LevelComplete()
    {
        Game.User.SetCurrentLevel();

        LevelCompleteWindow.Of();
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
    private void LevelFailed()
    {
        if (_isEnded)
            return;

        _isEnded = true;
        LevelFailedWindow.Of();
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

    public Material GetSignalMaterial(ContainerColor color)
    {
        return Game.ColorManager.GetSignalMaterial(color);
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

    private void OnLoadingLevelComplete()
    {
        _levelLoader.OnLoadingComplete -= OnLoadingLevelComplete;
    }
    public void ExitLevel(Action onComplete)
    {
        _levelLoader.ExitLevel(onComplete);
    }
}



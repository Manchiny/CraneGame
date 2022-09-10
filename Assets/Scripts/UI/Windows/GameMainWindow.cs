using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static LevelConfigs;

public class GameMainWindow : AbstractWindow
{
    [SerializeField] private CarsInfoPanel _carsInfoPanel;
    [SerializeField] private InfoScreenView _infoScreenView;
    [Space]
    [SerializeField] private Button _optionsButton;
    [SerializeField] private OptionsPanel _optionsPanel;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _infoButton;
    [Space]
    [Header("���������:")]
    [SerializeField] private List<Joystick> _joysticks;

    private LevelConfig _levelConfig;

    public static GameMainWindow Show(Crane crane, LevelConfig levelConfig) =>
                    Game.Windows.ScreenChange<GameMainWindow>(false, w => w.Init(crane, levelConfig));
   
    private void Init(Crane crane, LevelConfig config)
    {
        ForceHide();
        Game.LevelLoader.LoadingCompleted += OnLevelLoaded;
        Game.LevelLoader.LevelExited += OnExitLevel;

        _carsInfoPanel.Init();

        _levelConfig = config;
        _infoScreenView.Init(crane.Magnit, config.ShipConfigs.Length, config.MaxContainerCrushes);

        _optionsButton.onClick.AddListener(ShowHideOptions);
        _exitButton.onClick.AddListener(OnExitButtonClick);
        _infoButton.onClick.AddListener(OnInfoButtonClick);

        _joysticks.ForEach(x => x.Init(crane));
    }

    public void SetShipsInfo(int currentShipNumber, int shipsCount)
    {
        _infoScreenView.SetShipsInfo(currentShipNumber, shipsCount);
    }

    public void UpdateCrushContainersInfo()
    {
        _infoScreenView.OnContainerCrush();
    }

    private void OnLevelLoaded()
    {
        Game.LevelLoader.LoadingCompleted -= OnLevelLoaded;
        Unhide();
    }
    private void ShowHideOptions()
    {
        if (_optionsPanel.IsActive)
            _optionsPanel.Hide();
        else
            _optionsPanel.Show();
    }

    private void OnExitButtonClick()
    {
        ExitLevelWindow.Show();
    }

    private void OnInfoButtonClick()
    {
        Debug.Log("info btn click");
    }

    private void OnExitLevel()
    {
        Game.LevelLoader.LevelExited -= OnExitLevel;
        Close();
    }
}

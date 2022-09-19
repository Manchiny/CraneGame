using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameInfoWindow : AbstractWindow
{
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _nextPage;
    [SerializeField] private Button _previousPage;
    [Space]
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _pageNumber;
    [SerializeField] private Image _image;
    [Space]
    [SerializeField] private GameInfo _gameInfo;

    private const string LockKey = "GameInfoWindow";

    private GameInfoPage _currentGameInfoPage;
    private int _currentPageNumber;

    private event Action _onClose;

    public static GameInfoWindow Show(Action onClose) =>
                  Game.Windows.ScreenChange<GameInfoWindow>(false, w => w.Init(onClose));

    protected override void OnClose()
    {
        base.OnClose();

        if (_onClose != null)
            _onClose?.Invoke();
    }

    private void Init(Action onClose)
    {
        _onClose = onClose;
        _closeButton.onClick.AddListener(CloseAnimated);

        _currentPageNumber = 0;
        _gameInfo.TryGetPage(_currentPageNumber, out _currentGameInfoPage);
        SetPage();

        _nextPage.onClick.AddListener(OnNextPageButtonClick);
        _previousPage.onClick.AddListener(OnPreviousPageButtonClick);
    }

    private void OnNextPageButtonClick()
    {
        if (_gameInfo.TryGetPage(_currentPageNumber + 1, out GameInfoPage page))
        {
            _currentGameInfoPage = page;
            _currentPageNumber++;
        }
        else
        {
            _currentPageNumber = 0;
            _gameInfo.TryGetPage(_currentPageNumber, out _currentGameInfoPage);
        }

        SetPage();
    }

    private void OnPreviousPageButtonClick()
    {
        if (_gameInfo.TryGetPage(_currentPageNumber - 1, out GameInfoPage page))
        {
            _currentGameInfoPage = page;
            _currentPageNumber--;
        }
        else
        {
            _currentPageNumber = _gameInfo.PagesCount - 1;
            _gameInfo.TryGetPage(_currentPageNumber, out _currentGameInfoPage);
        }

        SetPage();
    }

    private void SetPage()
    {
        _image.sprite = _currentGameInfoPage.Sprite;
        _descriptionText.text = _currentGameInfoPage.Description;
        _pageNumber.text = $"{_currentPageNumber + 1}/{_gameInfo.PagesCount}";
    }
}
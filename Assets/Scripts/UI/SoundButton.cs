using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SoundButton : MonoBehaviour
{
    [SerializeField] private Image _contemtImage;
    [SerializeField] private Sprite _sountOnSprite;
    [SerializeField] private Sprite _soundOffSprite;

    private Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();

        _button.onClick.RemoveAllListeners();
        SetActualImage();

        _button.onClick.AddListener(OnButtonClick);
    }
    
    private void OnButtonClick()
    {
        Game.Sound.SetSound(!Game.User.IsSoundOn());
        SetActualImage();
    }

    private void SetActualImage()
    {
        _contemtImage.sprite = Game.User.IsSoundOn() ? _sountOnSprite : _soundOffSprite;
    }

}

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoScreenView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _cargoAngelText;
    [SerializeField] private TextMeshProUGUI _shipsText;
    [SerializeField] private TextMeshProUGUI _crushedContainersText;

    private Magnit _magnit;
    private int _canCrushContainers;
    public void Init(Magnit magnit, int shipsCount, int canCrushContainers)
    {
        if(_magnit != null)
            _magnit.OnRotationSet -= OnCargoAngelSet;

        _magnit = magnit;
        _magnit.OnRotationSet += OnCargoAngelSet;

        SetShipsInfo(canCrushContainers, shipsCount);
        _canCrushContainers = canCrushContainers;
        UpdateCrushContainersInfo();

    }
    public void OnCargoAngelSet(bool isLoaded, float angel)
    {
        if (!isLoaded)
        {
            _cargoAngelText.text = "Rotation: --";
        }
        else
        {
            if (angel == 0)
                _cargoAngelText.text = "Rotation: 00";
            else
            {
                // _cargoAngelText.text = $"Rotation: {Mathf.Abs(angel).ToString()}";
                float rotation = angel > 180 ? Mathf.Abs(angel - 180 -90) : Mathf.Abs(angel-90);
                _cargoAngelText.text = $"Rotation: {rotation.ToString().Substring(0, Mathf.Abs(rotation).ToString().LastIndexOf(',') + 2)}";
            }
        }
    }

    public void SetShipsInfo(int currentShipNumber, int shipsCount)
    {
        _shipsText.text = $"{currentShipNumber}/{shipsCount}";
    }
    public void OnContainerCrush()
    {
        _canCrushContainers--;
        UpdateCrushContainersInfo();
    }
    private void UpdateCrushContainersInfo()
    {
        _crushedContainersText.text = _canCrushContainers.ToString();
    }
}

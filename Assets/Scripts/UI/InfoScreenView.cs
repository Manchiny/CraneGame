using TMPro;
using UnityEngine;

public class InfoScreenView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _cargoAngelText;
    [SerializeField] private TextMeshProUGUI _shipsText;
    [SerializeField] private TextMeshProUGUI _crushedContainersText;

    private const float MaxRotationAngle = 180f;
    private const float TargetAngle = 90f;

    private Magnit _magnit;
    private int _canCrushContainers;

    public void Init(Magnit magnit, int shipsCount, int canCrushContainers)
    {
        if (_magnit != null)
            _magnit.RotationChanged -= OnCargoAngelSet;

        _magnit = magnit;
        _magnit.RotationChanged += OnCargoAngelSet;

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
                float rotation = angel > MaxRotationAngle ? Mathf.Abs(angel - MaxRotationAngle - TargetAngle) : Mathf.Abs(angel - TargetAngle);
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

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public class InfoScreenView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _cargoAngelText;

    public static InfoScreenView Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }

        Destroy(this);
    }
    public void OnCargoAngelSet(bool isLoaded = true, float angel = 0)
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

}

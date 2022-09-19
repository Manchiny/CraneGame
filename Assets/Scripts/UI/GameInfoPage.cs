using System;
using UnityEngine;

[Serializable]
public class GameInfoPage 
{
    [SerializeField] private Sprite _sprite;
    [SerializeField] private string _text;

    public Sprite Sprite => _sprite;
    public string Description => _text;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    [Header("Материалы для контейнеров")]
    [SerializeField] private Material _black;
    [SerializeField] private Material _blue;
    [SerializeField] private Material _red;
    [SerializeField] private Material _green;
    [Space]
    [Header("Материалы для сигнала машин")]
    [SerializeField] private Material _blackSignal;
    [SerializeField] private Material _blueSignal;
    [SerializeField] private Material _redSignal;
    [SerializeField] private Material _greenSignal;

    public Dictionary<ContainerColor, Material> ContainerMaterials { get; private set; }
    public Dictionary<ContainerColor, Material> SignalMaterials { get; private set; }

    public void Init()
    {
        ContainerMaterials = new Dictionary<ContainerColor, Material>()
                {
                    {ContainerColor.Black, _black },
                    {ContainerColor.Blue, _blue },
                    {ContainerColor.Red, _red },
                    {ContainerColor.Green, _green }
                };

        SignalMaterials = new Dictionary<ContainerColor, Material>()
                {
                    {ContainerColor.Black, _blackSignal },
                    {ContainerColor.Blue, _blueSignal },
                    {ContainerColor.Red, _redSignal },
                    {ContainerColor.Green, _greenSignal }
                };
    }
    public enum ContainerColor
    {
        Black,
        Blue,
        Red,
        Green
    }

    public Material GetSignalMaterial(ContainerColor color)
    {
        if (SignalMaterials.TryGetValue(color, out Material value))
        {
            return value;
        }
        else return null;
    }

    public Color GetConformingColor(ContainerColor color)
    {
        var material = SignalMaterials[color];
        var result = material.color;
        return result;
    }
}

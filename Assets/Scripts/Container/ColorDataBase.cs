using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ColorDataBase;

[CreateAssetMenu]
public class ColorDataBase : ScriptableObject
{
    [SerializeField] private List<ColoredMaterial> _containerMaterials;
    [SerializeField] private List<ColoredMaterial> _signalMaterials;

    public enum ContainerColor
    {
        Black,
        Blue,
        Red,
        Green,
        Yellow,
        Magenta
    }

    public Material GetSignalMaterial(ContainerColor color)
    {
       var coloredMaterial = GetColoredSignalMaterial(color);

        if (coloredMaterial != null)
            return coloredMaterial.Material;

        return null;
    }

    public Color GetConformingColor(ContainerColor color)
    {
        var coloredMaterial = GetColoredSignalMaterial(color);
        return coloredMaterial.Material.color;
    }

    public bool TryGetConteinerMaterial(ContainerColor color, out Material material)
    {
        var coloredMaterial = _containerMaterials.Where(material => material.Color == color).FirstOrDefault();

        if (coloredMaterial != null)
        {
            material = coloredMaterial.Material;
            return true;
        }

        material = null;
        return false;
    }

    private ColoredMaterial GetColoredSignalMaterial(ContainerColor color) => _signalMaterials.Where(material => material.Color == color).FirstOrDefault();
}

[Serializable]
public class ColoredMaterial
{
    [SerializeField] private ContainerColor _color;
    [SerializeField] private Material _material;

    public ContainerColor Color => _color;
    public Material Material => _material;
}

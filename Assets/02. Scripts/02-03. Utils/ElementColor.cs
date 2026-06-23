using UnityEngine;

public static class ElementColor
{
    public static Color GetElementColor(EElement element)
    {
        switch (element)
        {
            case EElement.Fire:
                return Color.red;
            case EElement.Water:
                return Color.blue;
            case EElement.Grass:
                return Color.green;
            case EElement.Electric:
                return Color.orange;
        }
        return Color.white;
    }

}

using UnityEngine;

[CreateAssetMenu(fileName = "Line", menuName = "Scriptable Objects/Line")]
public class Line : ScriptableObject
{
    public string Name;
    public Color color;

    public Color GetColor()
    {
        return color;
    }
}

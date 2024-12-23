using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "Scriptable Objects/Settings")]
public class Settings : ScriptableObject
{
    public float centerLOT;
    public float centerLAT;
    public float degToUnit;
    
    [Header("Node Zoom")]
    public float targetNodeSize;
    public float maxNodeSize;
    
    [Header("Camera")]
    public float scrollSpeed;
    public float minZoom;
    public float maxZoom;

    [Header("Route Finding")] 
    public float transferTime;
}

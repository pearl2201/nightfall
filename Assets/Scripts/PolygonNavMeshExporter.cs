using UnityEngine;
using Unity.Properties;


#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "PolygonNavMeshExporter", menuName = "Scriptable Objects/PolygonNavMeshExporter")]
public class PolygonNavMeshExporter : ScriptableObject
{
    [Header("Simple binding")]
    public int mapId = 0;
    public int startX = 0;
    public int startZ = 0;
    public int endX = 0;
    public int endZ = 0;
    public int width => endX - startX;
    public int height => endZ - startZ;
    [CreateProperty]
    public string widthLabel => "Width: " + (endZ - startZ);
    [CreateProperty]
    public string heightLabel => "Height: " + (endZ - startZ);
}

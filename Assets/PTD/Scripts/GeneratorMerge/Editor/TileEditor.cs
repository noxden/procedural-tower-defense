using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Tile))]
public class TileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Tile handler = target as Tile;

        base.OnInspectorGUI();

        if (GUILayout.Button("Apply Bulk Add / Remove"))
        {
            handler.ApplyBulkChanges();
        }
    }
}

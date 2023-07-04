using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileDefinition))]
public class TileDefinitionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TileDefinition handler = target as TileDefinition;

        base.OnInspectorGUI();

        if (GUILayout.Button("Apply Bulk Add / Remove"))
        {
            handler.ApplyBulkChanges();
        }
    }
}

//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Daniel Heilmann (771144)
//========================================================================

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

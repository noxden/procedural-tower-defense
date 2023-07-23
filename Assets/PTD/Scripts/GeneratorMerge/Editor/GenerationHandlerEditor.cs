//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Daniel Heilmann (771144)
//========================================================================

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerationHandler))]
public class GenerationHandlerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var handler = target as GenerationHandler;

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate All"))
            if (handler != null)
                handler.GenerateLevel();
        if (GUILayout.Button("Reset Level"))
            if (handler != null)
                handler.nodeManager.ResetGrid();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Path"))
            if (handler != null)
                handler.pathGenerator.Generate(generateInstantly: false); //< To visualize path generation when pressing the "Generate Path" button.
        if (GUILayout.Button("Generate Tilemap"))
            if (handler != null)
                handler.GenerateTilemap();
        if (GUILayout.Button("Iterate Tilemap"))
            if (handler != null)
                handler.waveFunctionSolver.Iterate();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        base.OnInspectorGUI();

        if (handler == null) return;
        Vector2Int pathMinMax = handler.GetPathMinMax();
        handler.pathLength = EditorGUILayout.IntSlider("Path Length", handler.pathLength, pathMinMax.x, pathMinMax.y);
    }
}
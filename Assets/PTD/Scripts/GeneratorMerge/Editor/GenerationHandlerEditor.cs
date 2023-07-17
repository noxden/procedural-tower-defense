using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GenerationHandler))]
public class GenerationHandlerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GenerationHandler handler = target as GenerationHandler;

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate All"))
        {
            handler.GenerateLevel();
        }
        if (GUILayout.Button("Reset Level"))
        {
            handler.nodeManager.ResetGrid();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Path"))
        {
            handler.pathGenerator.Generate(generateInstantly: false);   //< To visualize path generation when pressing the "Generate Path" button.
        }
        if (GUILayout.Button("Generate Tilemap"))
        {
            handler.GenerateTilemap();
        }
        if (GUILayout.Button("Iterate Tilemap"))
        {
            handler.waveFunctionSolver.Iterate();
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        base.OnInspectorGUI();

        Vector2Int pathMinMax = handler.GetPathMinMax();
        handler.pathLength = EditorGUILayout.IntSlider("Path Length", handler.pathLength, pathMinMax.x, pathMinMax.y);
    }
}

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
        if (GUILayout.Button("Generate All"))
        {
            handler.GenerateLevel();
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Path"))
        {
            handler.pathGenerator.Generate(generateInstantly: false);
        }

        if (GUILayout.Button("Generate Tilemap"))
        {
            handler.GenerateTilemap();
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        base.OnInspectorGUI();

        handler.pathLength = EditorGUILayout.IntSlider("Path Length", handler.pathLength, (Mathf.Abs(handler.endPositionIndex.x - handler.startPositionIndex.x) + Mathf.Abs(handler.endPositionIndex.y - handler.startPositionIndex.y)) + 1, handler.gridSize.x * handler.gridSize.y);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CreateAssetMenu(fileName = "RotatedTile", menuName = "Wave Function Collapse/Rotated Tile")]
[ExecuteInEditMode]
public class RotatedTile : Tile
{
    [Space(20)]
    [Header("RotatedTile Config, don't modify anything above this!")]
    public Tile originalTile;
    [Range(1f, 3f)]
    public int amountRotatedClockwise;

    //# Methods 
    public override void InstantiatePrefab(Transform parentTransform)
    {
        base.InstantiatePrefab(parentTransform);
        instantiatedPrefab.transform.Rotate(0f, 90f * amountRotatedClockwise, 0f, Space.Self);
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        if (!EditorApplication.isPlayingOrWillChangePlaymode)  //< So that it only updates them when they are modified, not when play mode is started.
            UpdateTileFields();

        base.OnValidate();
    }
#endif

    [ContextMenu("Manually Update Sockets")]
    private void UpdateTileFields()
    {
        if (originalTile == null)
            return;

        prefab = originalTile.prefab;
        isPath = originalTile.isPath;

        northSockets = new List<Socket>(originalTile.northSockets);
        eastSockets = new List<Socket>(originalTile.eastSockets);
        southSockets = new List<Socket>(originalTile.southSockets);
        westSockets = new List<Socket>(originalTile.westSockets);

        RotateSockets(amountRotatedClockwise);
    }

    private void RotateSockets(int rotationIterations)
    {
        List<Socket> buffer1 = new List<Socket>();
        List<Socket> buffer2 = new List<Socket>();
        for (int i = 0; i < amountRotatedClockwise; i++)
        {
            buffer1.AddRange(eastSockets);
            eastSockets.Clear();
            eastSockets.AddRange(northSockets);
            northSockets.Clear();

            buffer2.AddRange(southSockets);
            southSockets.Clear();
            southSockets.AddRange(buffer1);
            buffer1.Clear();

            buffer1.AddRange(westSockets);
            westSockets.Clear();
            westSockets.AddRange(buffer2);
            buffer2.Clear();

            northSockets.AddRange(buffer1);
            buffer1.Clear();
        }
    }
}

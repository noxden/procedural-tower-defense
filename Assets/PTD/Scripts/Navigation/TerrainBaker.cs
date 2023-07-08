using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshSurface))]
public class TerrainBaker : MonoBehaviour
{
    [SerializeField] private GameEventManagerScriptableObject gameEventManager;
    private NavMeshSurface navMeshSurface;

    private void Start()
    {
        navMeshSurface = GetComponent<NavMeshSurface>();
    }

    private void OnEnable()
    {
        gameEventManager.startGameEvent.AddListener(BakeSurface);
    }

    private void OnDisable()
    {
        gameEventManager.startGameEvent.RemoveListener(BakeSurface);
    }

    private void BakeSurface()
    {
        navMeshSurface.BuildNavMesh();
    }
}

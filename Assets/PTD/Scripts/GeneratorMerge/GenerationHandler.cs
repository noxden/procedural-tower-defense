using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationHandler : MonoBehaviour
{
    public static GenerationHandler instance { get; set; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        // Get references to the nodemanager and the different generators
    }

    public void GenerateLevel()
    {
        // Here, potential pre- or post-generation methods could be called as well
        GenerateHeight();
        GeneratePath();
        GenerateTilemap();
    }

    private void GenerateHeight()
    {

    }

    private void GeneratePath()
    {

    }

    private void GenerateTilemap()
    {

    }
}

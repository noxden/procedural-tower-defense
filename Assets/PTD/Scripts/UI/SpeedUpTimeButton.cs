using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Jan Rau (769214)
//========================================================================

using UnityEngine.UI;

public class SpeedUpTimeButton : MonoBehaviour
{
    [SerializeField] private TimeManagerScriptableObject timeManager;
    [SerializeField] private Image gameSpeedImage;
    [SerializeField] private Sprite halfSpeedSprite;
    [SerializeField] private Sprite normalSpeedSprite;
    [SerializeField] private Sprite doubleSpeedSprite;
    [SerializeField] private Sprite tripleSpeedSprite;
    
    private enum GameSpeed
    {
        HalfSpeed,
        NormalSpeed,
        DoubleSpeed,
        TripleSpeed
    }
    private GameSpeed currentSpeed = GameSpeed.NormalSpeed;

    public void SwitchSpeedState()
    {
        if (timeManager.IsPaused())
            return;

        switch(currentSpeed)
        {
            case GameSpeed.HalfSpeed:
                timeManager.SetTimeScale(1f);
                gameSpeedImage.sprite = normalSpeedSprite;
                currentSpeed = GameSpeed.NormalSpeed;
                break;
            case GameSpeed.NormalSpeed:
                timeManager.SetTimeScale(2f);
                gameSpeedImage.sprite = doubleSpeedSprite;
                currentSpeed = GameSpeed.DoubleSpeed;
                break;
            case GameSpeed.DoubleSpeed:
                timeManager.SetTimeScale(3f);
                gameSpeedImage.sprite = tripleSpeedSprite;
                currentSpeed = GameSpeed.TripleSpeed;
                break;
            case GameSpeed.TripleSpeed:
                timeManager.SetTimeScale(0.5f);
                gameSpeedImage.sprite = halfSpeedSprite;
                currentSpeed = GameSpeed.HalfSpeed;
                break;
        }
    }
}

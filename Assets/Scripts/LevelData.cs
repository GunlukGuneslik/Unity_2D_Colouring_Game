/**
 * Engineer: Cagla Gunes Ocakli
 * Date: 23.04.2026
 * Project: Colouring Game
 * */

using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    [Header("Put the completetion percentage")]
    [SerializeField] private CompletetionPercentage percentage;

    public string LevelName = "undefined";
    public Sprite painting;
    //public Sprite completedMark;

    private bool completed = false;

    public bool pixelValuesUpdated = false;
    public long totalPixel = 0;
    public long changedPixels = 0;


    public void UpdateCompleted() {
        if (totalPixel <= 0) {
            completed = false;
            return;
        }
        float ratio = (float) changedPixels / (float) totalPixel;
        completed = ratio >= ( percentage.percentage / 100f);
    }

    public bool isCompleted() { 
        return completed;
    }
}

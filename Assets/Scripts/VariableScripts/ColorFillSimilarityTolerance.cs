/**
 * Engineer: Cagla Gunes Ocakli
 * Date: 24.04.2026
 * Project: Colouring Game
 * */
using Unity.Burst.CompilerServices;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorFillTolerance", menuName = "Scriptable Objects/ColorFillTolerance")]
public class ColorFillSimilarityTolerance : ScriptableObject
{
    [SerializeField] private float similarityTolerance = 0.2f;

    public float getSimilarityTolerance() { 
        return similarityTolerance;
    }
}

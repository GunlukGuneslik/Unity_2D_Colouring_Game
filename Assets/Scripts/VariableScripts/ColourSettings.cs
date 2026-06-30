/**
 * Engineer: Cagla Gunes Ocakli
 * Date: 24.04.2026
 * Project: Colouring Game
 * */

using UnityEngine;

[CreateAssetMenu(fileName = "ColourSettings", menuName = "Scriptable Objects/ColourSettings")]
public class ColourSettings : ScriptableObject
{
    [Header("Cahnge in these values can lead crash (check your painting assets)")]
    public Color boundaryColor = Color.black;
    public Color initalColor = Color.white;
}

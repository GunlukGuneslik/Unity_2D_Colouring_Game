/**
 * Engineer: Cagla Gunes Ocakli
 * Date: 23.04.2026
 * Project: Colouring Game
 * */

using UnityEngine;

[CreateAssetMenu(fileName = "ColourPalleteData", menuName = "Scriptable Objects/ColourPalleteData")]
public class ColourPalleteData : ScriptableObject
{
    public string ColorName = "undefined";
    public Color color;
}

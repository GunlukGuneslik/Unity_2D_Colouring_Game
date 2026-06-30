/**
 * Engineer: Cagla Gunes Ocakli
 * Date: 24.04.2026
 * Project: Colouring Game
 * */

using UnityEngine;

[CreateAssetMenu(fileName = "GameLevelList", menuName = "Scriptable Objects/GameLevelList")]
public class GameLevelList : ScriptableObject
{
    [Header("Put the levels here")]
    public LevelData[] AllLevels;
}

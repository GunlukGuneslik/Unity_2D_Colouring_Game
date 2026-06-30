/**
 * Engineer: Cagla Gunes Ocakli
 * Date: 23.04.2026
 * Project: Colouring Game
 * */

using UnityEngine;

[CreateAssetMenu(fileName = "CurrentLevelData", menuName = "Scriptable Objects/CurrentSceneData")]
public class CurrentLevelData : ScriptableObject
{
    public LevelData currentlySelectedLevel;
}

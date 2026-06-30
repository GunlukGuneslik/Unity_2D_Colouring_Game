/**
 * Engineer: Cagla Gunes Ocakli
 * Date: 24.04.2026
 * Project: Colouring Game
 * */

using UnityEngine;
using UnityEditor;
using System.IO;

public class ResetSavedProgress
{
    [MenuItem("Tools/Reset Coloring App Data")]
    public static void ResetAllProgress() {
        // just the colour preference is hold but for any case delete all.
        PlayerPrefs.DeleteAll();

        // deleting the painted versions
        string saveDirectory = Application.persistentDataPath;
        string[] savedFiles = Directory.GetFiles(saveDirectory, "*_progress.png");

        foreach (string file in savedFiles)
        {
            File.Delete(file);
        }

        // clean the calculated level data pixel values.
        string[] guids = AssetDatabase.FindAssets("t:LevelData");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            LevelData level = AssetDatabase.LoadAssetAtPath<LevelData>(path);

            if (level != null)
            {
                level.pixelValuesUpdated = false;
                level.totalPixel = 0;
                level.changedPixels = 0;

                EditorUtility.SetDirty(level);
            }
        }
        AssetDatabase.SaveAssets();
    }
    
}

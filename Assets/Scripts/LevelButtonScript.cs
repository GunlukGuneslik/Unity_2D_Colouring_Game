
/**
 * Engineer: Cagla Gunes Ocakli
 * Date: 23.04.2026
 * Project: Colouring Game
 * */

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class LevelButtonScript : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] public Image thumbnailImage;
    [SerializeField] public Image completedBadge;
    [SerializeField] public TextMeshProUGUI levelText;

    private void Start()
    {
        completedBadge.gameObject.SetActive(false);
    }


    public LevelData currentLevelData;
    public void loadLevelData(LevelData levelData, int id) {
        currentLevelData = levelData;

        string savePath = Application.persistentDataPath + "/" + levelData.LevelName + "_progress.png";
        if (File.Exists(savePath))
        {
            byte[] savedBytes = File.ReadAllBytes(savePath);
            Texture2D loadedTexture = new Texture2D(2,2);
            loadedTexture.LoadImage(savedBytes);
            Sprite loadedSprite = Sprite.Create(loadedTexture, 
                new Rect(0, 0, loadedTexture.width, loadedTexture.height), 
                new Vector2(0.5f, 0.5f));
            thumbnailImage.sprite = loadedSprite;
        }
        else {
            thumbnailImage.sprite = levelData.painting;
        }

        if (levelData.isCompleted())
        {
            completedBadge.gameObject.SetActive(true);
        }
        else { 
            completedBadge.gameObject.SetActive(false);
        }

            levelText.text = "" + id;
    }
}

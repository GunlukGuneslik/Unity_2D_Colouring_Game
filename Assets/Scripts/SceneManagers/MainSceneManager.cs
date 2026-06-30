/**
 * Engineer: Cagla Gunes Ocakli
 * Date: 23.04.2026
 * Project: Colouring Game
 * */

using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainSceneManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button forwardButton;
    [SerializeField] private Button backwardButton;
    [SerializeField] private LevelButtonScript[] levelSlots;

    [Header("Data Referances")]
    [SerializeField] private GameLevelList gameLevelList;
    [SerializeField] private CurrentLevelData selectedLevel;
    [SerializeField] private CompletetionPercentage completetionPercentage;
    [SerializeField] private ColourSettings colourSettings;
    [SerializeField] private ColorFillSimilarityTolerance similarityTolerance;

    private int currentPageIndex;
    private int allPageNumber;
    private LevelData[] AllLevels;

    private void Start()
    {
        AllLevels = gameLevelList.AllLevels;

        currentPageIndex = 0; 
        allPageNumber = AllLevels.Length / levelSlots.Length;
        if (AllLevels.Length % levelSlots.Length > 0) {
            allPageNumber++;
        }

        backwardButton.gameObject.SetActive(false);
        forwardButton.gameObject.SetActive(false);

        // if there is no other scene no meaning to add clik events
        if (allPageNumber > 1) {
            forwardButton.gameObject.SetActive(true);

            forwardButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (currentPageIndex < allPageNumber - 1)
                {
                    backwardButton.gameObject.SetActive(true);
                    currentPageIndex++;
                    if (currentPageIndex >= allPageNumber - 1)
                    {
                        forwardButton.gameObject.SetActive(false);
                    }
                    LoadData();
                }

            });

            backwardButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (currentPageIndex > 0)
                {
                    forwardButton.gameObject.SetActive(true);
                    currentPageIndex--;
                    if (currentPageIndex <= 0)
                    {
                        backwardButton.gameObject.SetActive(false);
                    }
                    LoadData();
                }
            });
        }
        
        LoadData();
    }

    private void LoadData() {
        StopAllCoroutines();
        StartCoroutine(UpdateSlots());
    }

    private IEnumerator UpdateSlots() {
        forwardButton.interactable = false;
        backwardButton.interactable = false;

        int offset = currentPageIndex * levelSlots.Length;

        for (int i = 0; i < levelSlots.Length; i++) { 
            int index = i + offset;
            if (index < AllLevels.Length) {
                LevelData currentLevel = AllLevels[index];

                levelSlots[i].transform.localScale = Vector3.zero;
                levelSlots[i].gameObject.SetActive(true);
                levelSlots[i].transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutBack);

                levelSlots[i].GetComponent<Button>().onClick.RemoveAllListeners();
                levelSlots[i].loadLevelData(currentLevel, index + 1);

                // check completetion
                if (!currentLevel.pixelValuesUpdated)
                {
                    string savePath = Application.persistentDataPath + "/" + currentLevel.LevelName + "_progress.png";

                    Color[] safePixels = currentLevel.painting.texture.GetPixels();
                    Color[] PaintedPixels = null;

                    if (System.IO.File.Exists(savePath))
                    {
                        // get the painted version
                        byte[] savedBytes = System.IO.File.ReadAllBytes(savePath);
                        Texture2D changedPainting = new Texture2D(currentLevel.painting.texture.width, currentLevel.painting.texture.height);
                        changedPainting.LoadImage(savedBytes);
                        PaintedPixels = changedPainting.GetPixels();
                        Destroy(changedPainting);
                    }

                    // 1. CRUCIAL: Reset the counters to 0 before counting!
                    currentLevel.totalPixel = 0;
                    currentLevel.changedPixels = 0;

                    for (int j = 0; j < safePixels.Length; j++)
                    {
                        Color originalColor = safePixels[j];

                        float boundaryDiff = Mathf.Abs(originalColor.r - colourSettings.boundaryColor.r)
                                           + Mathf.Abs(originalColor.g - colourSettings.boundaryColor.g)
                                           + Mathf.Abs(originalColor.b - colourSettings.boundaryColor.b);

                        // 2. CRUCIAL: Use OR (||) instead of AND (&&)
                        // This says: "If it is transparent, OR if it is not a black line"
                        if (originalColor.a < 0.1f || boundaryDiff > similarityTolerance.getSimilarityTolerance())
                        {
                            // This is a paintable white space or transparent background!
                            currentLevel.totalPixel++;

                            // 3. Did the player paint this specific spot?
                            if (PaintedPixels != null)
                            {
                                Color paintedColor = PaintedPixels[j];
                                float paintedDiff = Mathf.Abs(paintedColor.r - originalColor.r)
                                                  + Mathf.Abs(paintedColor.g - originalColor.g)
                                                  + Mathf.Abs(paintedColor.b - originalColor.b);

                                // If the color changed, count it!
                                if (paintedDiff > similarityTolerance.getSimilarityTolerance())
                                {
                                    currentLevel.changedPixels++;
                                }
                            }
                        }
                    }

                    currentLevel.pixelValuesUpdated = true;
                    currentLevel.UpdateCompleted();
                }

                // important!
                levelSlots[i].GetComponent<Button>().onClick.AddListener(() =>
                {
                    selectedLevel.currentlySelectedLevel = AllLevels[index];
                    // transection to the colouring scene
                    SceneManager.LoadScene(2);
                });
            }
            else
            {
                levelSlots[i].gameObject.SetActive(false);
            }
            yield return new WaitForSeconds(0.1f);
        }

        forwardButton.interactable = true;
        backwardButton.interactable = true;
    }
}

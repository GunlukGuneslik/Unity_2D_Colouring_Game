/**
 * Engineer: Cagla Gunes Ocakli
 * Date: 23.04.2026
 * Project: Colouring Game
 * */

using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class ColoringSceneManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image paintingArea;
    [SerializeField] private Image ClickBlocker;
    [SerializeField] private Button closeButton;
    [SerializeField] private ColourPalleteButtonScript[] palettes;
    [SerializeField] private TextMeshProUGUI LevelNameText;

    [Header("Data Referances")]
    [SerializeField] private ColourPalleteData[] AllColours;
    [SerializeField] private CurrentLevelData data;
    [SerializeField] private ColorFillSimilarityTolerance Tolerance;

    [Header("FloodFill Performance")]
    [SerializeField] private float maxFrameTimeMs = 8;

    [Header("Boundary Colour")]
    [SerializeField] private ColourSettings colorSetting;

    [Header("Celebration UI")]
    [SerializeField] private CanvasGroup celebrationPanel;
    [SerializeField] private RectTransform popupBox;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button backToMenuButton;

    private Color boundaryColour;
    private float tolerance;

    private LevelData levelData;
    // -1 is a flag to indicate there is no colour is sellected yet!
    private int selectedColourIndex;
    // flag to show congradilations part
    private bool isLevelWon = false;

    private Texture2D safePainting;
    private Texture2D changedPainting;
    private string savePath;

    // flag to check is the ienumerator working currently.
    private bool isPainting = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        

        if (PlayerPrefs.HasKey("SelectedColour"))
        {
            selectedColourIndex = PlayerPrefs.GetInt("SelectedColour");
            palettes[selectedColourIndex].chooseSlot();
        }
        else {
            selectedColourIndex = -1;
        }

        boundaryColour = colorSetting.boundaryColor;
        tolerance = Tolerance.getSimilarityTolerance();

        levelData = data.currentlySelectedLevel;

        // display level name
        LevelNameText.text = levelData.LevelName;

        savePath = Application.persistentDataPath + "/" + levelData.LevelName + "_progress.png";
        safePainting = levelData.painting.texture;
        changedPainting = new Texture2D(safePainting.width, safePainting.height);

        if (File.Exists(savePath))
        {
            byte[] savedBytes = File.ReadAllBytes(savePath);
            changedPainting.LoadImage(savedBytes);
        }
        else
        {
            changedPainting.SetPixels(safePainting.GetPixels());
            changedPainting.Apply();
        }

        Sprite canvasSprite = Sprite.Create(changedPainting, 
            new Rect(0, 0, changedPainting.width, changedPainting.height), new Vector2(0.5f, 0.5f));
        paintingArea.sprite = canvasSprite;


        //TODO: swaping the colour pallete
        for (int i = 0; i < palettes.Length; i++) {
            ColourPalleteButtonScript currentSlot = palettes[i];
            if (i < AllColours.Length)
            {
                currentSlot.initalizeSlot(AllColours[i]);
                int index = i;

                currentSlot.ColourButton.onClick.RemoveAllListeners();
                currentSlot.ColourButton.onClick.AddListener(() =>
                {
                    // there was no selection before.
                    if (selectedColourIndex < 0){
                        currentSlot.chooseSlot();
                        PlayerPrefs.SetInt("SelectedColour", index);
                        selectedColourIndex = index;
                    } 
                    // there was a collor sellected but it is not the one currently sellected.
                    else if (index != PlayerPrefs.GetInt("SelectedColour")) {
                        palettes[selectedColourIndex].unchooseSlot();
                        palettes[index].chooseSlot();
                        PlayerPrefs.SetInt("SelectedColour", index);
                        selectedColourIndex = index;
                    }
                    // previously sellected colour is sellected again.
                    else {
                        PlayerPrefs.DeleteKey("SelectedColour");
                        currentSlot.unchooseSlot();
                        selectedColourIndex = -1;
                    }
                });
            }
            else {
                // if there is no colour then there is no pallete slot
                currentSlot.gameObject.SetActive(false);
            }
        }

        // close button
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() =>
        {
            // if it is painting you need to stop other wise it cannot copmplete the colouring
            if (isPainting) return;

            data.currentlySelectedLevel.UpdateCompleted();

            byte[] imageBytes = changedPainting.EncodeToPNG();
            File.WriteAllBytes(savePath, imageBytes);

            // transection to the main scene
            DOTween.KillAll();
            SceneManager.LoadScene(1);
        });

        backToMenuButton.onClick.RemoveAllListeners();
        backToMenuButton.onClick.AddListener(() =>
        {
            byte[] imageBytes = changedPainting.EncodeToPNG();
            File.WriteAllBytes(savePath, imageBytes);

            ClickBlocker.gameObject.SetActive(false);

            data.currentlySelectedLevel.UpdateCompleted();

            DOTween.KillAll();
            SceneManager.LoadScene(1);
        });

        continueButton.onClick.RemoveAllListeners();
        continueButton.onClick.AddListener(() => {

            celebrationPanel.blocksRaycasts = false;
            popupBox.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InBack);
            celebrationPanel.DOFade(0f, 0.4f).SetDelay(0.1f).OnComplete(() =>
            {
                ClickBlocker.gameObject.SetActive(false);
                celebrationPanel.gameObject.SetActive(false);
            });

        });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (ClickBlocker.gameObject.activeInHierarchy)
            {
                return;
            }

            // when two IEnumerators work at the same time it cause a semantic error on the coloured image
            if (isPainting) return;

            // check if there is a valid colour sellected
            if (selectedColourIndex < 0 || selectedColourIndex >= AllColours.Length)
            {
                return;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                paintingArea.rectTransform,
                Input.mousePosition,
                null,
                out Vector2 localCursorPoint
            );

            Rect uiRect = paintingArea.rectTransform.rect;
            float normalizedX = (localCursorPoint.x - uiRect.x) / uiRect.width;
            float normalizedY = (localCursorPoint.y - uiRect.y) / uiRect.height;

            int pixelX = Mathf.RoundToInt(normalizedX * changedPainting.width);
            int pixelY = Mathf.RoundToInt(normalizedY * changedPainting.height);

            if (pixelX < 0 || pixelX >= changedPainting.width || pixelY < 0 || pixelY >= changedPainting.height)
            {
                return;
            }

            Color clickedColor = safePainting.GetPixel(pixelX, pixelY);
            if (clickedColor.a >= 0.1f)
            {
                float colourDiff = Mathf.Abs(clickedColor.r - boundaryColour.r)
                                 + Mathf.Abs(clickedColor.g - boundaryColour.g)
                                 + Mathf.Abs(clickedColor.b - boundaryColour.b);

                if (colourDiff <= Tolerance.getSimilarityTolerance())
                {
                    return;
                }
            }

            StartCoroutine(FloodFill(pixelX, pixelY, AllColours[selectedColourIndex].color));
            // this  is crucial!!!
            data.currentlySelectedLevel.UpdateCompleted();
        }
    }

    // In here i have used basically Breadth-First Traversal Algorithm that i have learned in the class.
    // It comes intuative to me and i think it fits perfectly with "flood" fill
    private IEnumerator FloodFill(int StartX, int StartY, Color selectedColour) {
        // IF THE SELECTED POINT IS ALREADY PAINTED BEFORE DONT DO ANYTHING IT IS JUST A WASTE OF ENERGY
        if (changedPainting.GetPixel(StartX, StartY) == selectedColour)
        {
            yield break;
        }

        isPainting = true;

        int width = changedPainting.width;
        int height = changedPainting.height;

        Color[] paintedPixels = changedPainting.GetPixels();
        Color[] originalPixels = safePainting.GetPixels();
        int currentPixel = StartY * width + StartX;

        // i am trying to prevent the freezing
        Stopwatch timer = new Stopwatch();
        timer.Start();


        // logically there must be finite or countably infinte number of pixells exist
        Queue<int> queue = new Queue<int>();
        // to mark the nodes visited
        bool[] visited = new bool[paintedPixels.Length];

        queue.Enqueue(currentPixel);
        visited[currentPixel] = true;

        while (queue.Count > 0) { 
            currentPixel = queue.Dequeue();
            // do ypur job: now it is colouring
            Color originalColor = originalPixels[currentPixel];
            Color currentColor = paintedPixels[currentPixel];

            if (currentColor == originalColor && selectedColour != originalColor)
            {
                levelData.changedPixels++;
            }

            /*
             * there is no erase now
            else if (currentColor != originalColor && selectedColour == originalColor)
            {
                levelData.changedPixels--;
            } */

            paintedPixels[currentPixel] = selectedColour;
            int currentX = currentPixel % width;

            int temp = currentPixel + 1;
            if ((currentX < width - 1) && getValidty(paintedPixels, originalPixels, temp) && !visited[temp]) {
                // first mark
                visited[temp] = true;
                // then add to the queue so at the next turn program will fill it.
                queue.Enqueue(temp);
            }

            temp = currentPixel + width;
            if (getValidty(paintedPixels, originalPixels, temp) && !visited[temp])
            {
                visited[temp] = true;
                queue.Enqueue(temp);
            }

            temp = currentPixel - 1;
            if ((currentX > 0) && getValidty(paintedPixels, originalPixels, temp) && !visited[temp])
            {
                visited[temp] = true;
                queue.Enqueue(temp);
            }

            temp = currentPixel - width;
            if (getValidty(paintedPixels, originalPixels, temp) && !visited[temp])
            {
                visited[temp] = true;
                queue.Enqueue(temp);
            }

            if (timer.ElapsedMilliseconds > maxFrameTimeMs)
            {
                // show the changes
                changedPainting.SetPixels(paintedPixels);
                changedPainting.Apply();
                yield return null;
                timer.Restart();
            }
        }

        changedPainting.SetPixels(paintedPixels);
        changedPainting.Apply();

        // ADD COMPLETETÝON RATE CHECK
        // locigally user can complete the %90 percent of the picture if s/he is painting.
        if (!levelData.isCompleted())
        {
            levelData.UpdateCompleted();
            // triger annimation
            if (!isLevelWon && levelData.isCompleted())
            {
                isLevelWon = true;
                coloringCompleted();
            }
        }


        isPainting = false;
    }

    /*
    // this method assumes there is a sellected colour exists
    private bool getValidty(int width, int height, Vector2Int location) {
        if (location.x < 0 || location.x >= width) {
            return false;
        }

        if (location.y < 0 ||location.y >= height) {
            return false;
        }

        // look for the similarity
        Color currentAreaColour = safePainting.GetPixel(location.x, location.y);

        // thþs þs for transperant pixels
        if (currentAreaColour.a < 0.1f)
        {
            return true;
        }

        float colourDiff = Mathf.Abs(currentAreaColour.r - boundaryColour.r)
                + Mathf.Abs(currentAreaColour.g - boundaryColour.g)
                + Mathf.Abs(currentAreaColour.b - boundaryColour.b);

        // obviously if you get close to black line your value should colse to value of the black. So the difference goes to zero!
        if (colourDiff <= Tolerance.getSimilarityTolerance()) {
            return false;
        }

        return true;
    }
    */

    private bool getValidty(Color[] painterdArr, Color[] safeArr, int index) {
        if (index < 0 || index >= painterdArr.Length) return false;

        // look for the similarity
        Color currentAreaColour = safeArr[index];
        // thþs þs for transperant pixels
        if (currentAreaColour.a < 0.1f)
        {
            return true;
        }

        float colourDiff = Mathf.Abs(currentAreaColour.r - boundaryColour.r)
                + Mathf.Abs(currentAreaColour.g - boundaryColour.g)
                + Mathf.Abs(currentAreaColour.b - boundaryColour.b);

        // obviously if you get close to black line your value should colse to value of the black. So the difference goes to zero!
        if (colourDiff <= Tolerance.getSimilarityTolerance())
        {
            return false;
        }

        return true;
    }


    // TODO: ADD SOME AUDIO AND ANIMATION
    private void coloringCompleted() {

        isPainting = false;

        byte[] imageBytes = changedPainting.EncodeToPNG();
        File.WriteAllBytes(savePath, imageBytes);

        ClickBlocker.gameObject.SetActive(true);

        celebrationPanel.gameObject.SetActive(true);
        celebrationPanel.alpha = 0f;
        celebrationPanel.blocksRaycasts = true;
        popupBox.localScale = Vector3.zero;

        celebrationPanel.DOFade(1f, 0.5f);
        popupBox.DOScale(Vector3.one, 0.7f).SetEase(Ease.OutBack).SetDelay(0.2f);
    }
}

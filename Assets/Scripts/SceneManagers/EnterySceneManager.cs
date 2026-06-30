/**
 * Engineer: Cagla Gunes Ocakli
 * Date: 23.04.2026
 * Project: Colouring Game
 * */

using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnterySceneManager : MonoBehaviour
{
    [SerializeField] public Button StartButton;
    void Start()
    {
        StartButton.GetComponent<Button>().onClick.RemoveAllListeners();

        // for bouncing affect
        StartButton.transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.8f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutQuad);

        StartButton.GetComponent<Button>().onClick.AddListener(() => {
            // transection to the main scene
            SceneManager.LoadScene(1);
        });
    }
}

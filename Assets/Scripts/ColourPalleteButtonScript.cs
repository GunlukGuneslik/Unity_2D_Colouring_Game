/**
 * Engineer: Cagla Gunes Ocakli
 * Date: 23.04.2026
 * Project: Colouring Game
 * */

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ColourPalleteButtonScript : MonoBehaviour
{
    [Header("Colors to indicate choosen slot")]
    [SerializeField] public Color InitalBackGroundColor;
    [SerializeField] public Color choosenBackGroundColour;


    [Header("UI Elements")]
    [SerializeField] public Button ColourButton;
    [SerializeField] public Image BackGround;
    [SerializeField] private RectTransform visual;

    ColourPalleteData data;

    public void initalizeSlot(ColourPalleteData data) {
        this.data = data;
        ColourButton.image.color = data.color;
    }

    public void chooseSlot() {
        BackGround.color = choosenBackGroundColour;
        PlayBounce();
        
    }

    public void unchooseSlot() {
        BackGround.color = InitalBackGroundColor;
        visual.DOScale(Vector3.one, 0.25f * 0.5f).SetEase(Ease.OutSine);
    }

    private void PlayBounce()
    {
        visual.DOKill();
        visual.localScale = Vector3.one;
        visual.DOPunchScale(Vector3.one * (1.15f - 1f), 0.25f, 2, 0.5f);
    }
}

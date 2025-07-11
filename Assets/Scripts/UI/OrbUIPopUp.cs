using TMPro;
using UnityEngine;

public class OrbUIPopUp : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI giveText;
    [SerializeField] private TextMeshProUGUI takeText;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float popupDuration = 2f;
    [SerializeField] private float animTime = 0.4f;

    private RectTransform rectTransform;
    private bool isAnimating = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup.alpha = 0;
    }

    public void Show(string giveMsg, string takeMsg)
    {
        LeanTween.cancel(gameObject);

        if (isAnimating)
        {
            LeanTween.alphaCanvas(canvasGroup, 0f, animTime * 0.5f).setOnComplete(() =>
            {
                AnimatePopup(giveMsg, takeMsg);
            });
        }
        else
        {
            AnimatePopup(giveMsg, takeMsg);
        }
    }

    private void AnimatePopup(string giveMsg, string takeMsg)
    {
        isAnimating = true;

        giveText.text = giveMsg;
        takeText.text = takeMsg;

        canvasGroup.alpha = 0f;

        LeanTween.alphaCanvas(canvasGroup, 1f, animTime).setEaseOutExpo();
        LeanTween.moveY(rectTransform, -120f, animTime).setEaseOutBack();

        LeanTween.delayedCall(gameObject, popupDuration, () =>
        {
            LeanTween.alphaCanvas(canvasGroup, 0f, animTime).setEaseInExpo();
            LeanTween.moveY(rectTransform, 90f, animTime).setEaseInBack()
                .setOnComplete(() => isAnimating = false);
        });
    }
}

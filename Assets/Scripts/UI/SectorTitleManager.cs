using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SectorTitleManager : MonoBehaviour
{
    [SerializeField] private GameObject m_sectorNumber;
    [SerializeField] private GameObject m_sectorName;
    [SerializeField] private Image m_background;

    [SerializeField] private float animationDuration = 1f;
    [SerializeField] private float delayBetweenAnimations = 0.5f;
    [SerializeField] private float displayDuration = 5f;

    private TextMeshProUGUI m_sectorNumberText;
    private TextMeshProUGUI m_sectorNameText;

    private AudioSource m_sectorNameClip;

    private void Start()
    {
        m_sectorNumberText = m_sectorNumber.GetComponent<TextMeshProUGUI>();
        m_sectorNameText = m_sectorName.GetComponent<TextMeshProUGUI>();
        m_sectorNameClip = GetComponent<AudioSource>();

        if (m_background != null)
        {
            Color bgColor = m_background.color;
            bgColor.a = 0;
            m_background.color = bgColor;
        }

        ContextManager.instance.SetCurrentSectorTitleManager(this);
    }

    public void AnimateSectorTitle()
    {
        if (m_sectorNumberText != null) m_sectorNumberText.color = new Color(m_sectorNumberText.color.r, m_sectorNumberText.color.g, m_sectorNumberText.color.b, 0);
        if (m_sectorNameText != null) m_sectorNameText.color = new Color(m_sectorNameText.color.r, m_sectorNameText.color.g, m_sectorNameText.color.b, 0);

        AnimateSectorNumber();
    }

    private void AnimateSectorNumber()
    {
        m_sectorNumber.SetActive(true);

        if (m_sectorNumberText != null)
        {
            LeanTween.value(m_sectorNumber, 0, 1, animationDuration)
                .setOnUpdate((float alpha) =>
                {
                    m_sectorNumberText.color = new Color(m_sectorNumberText.color.r, m_sectorNumberText.color.g, m_sectorNumberText.color.b, alpha);
                })
                .setOnComplete(AnimateSectorName);
        }

        if (m_background != null)
        {
            LeanTween.value(gameObject, 0, 0.4f, animationDuration)
                .setOnUpdate((float alpha) =>
                {
                    Color bgColor = m_background.color;
                    bgColor.a = alpha;
                    m_background.color = bgColor;
                })
                .setEase(LeanTweenType.linear);
        }
    }

    private void AnimateSectorName()
    {
        m_sectorName.SetActive(true);

        m_sectorNameClip.Play();

        if (m_sectorNameText != null)
        {
            LeanTween.value(m_sectorName, 0, 1, animationDuration)
                .setDelay(delayBetweenAnimations)
                .setOnUpdate((float alpha) =>
                {
                    m_sectorNameText.color = new Color(m_sectorNameText.color.r, m_sectorNameText.color.g, m_sectorNameText.color.b, alpha);
                })
                .setOnComplete(OnAnimationComplete);
        }
    }

    private void OnAnimationComplete()
    {
        LeanTween.delayedCall(displayDuration, FadeOut);
    }

    private void FadeOut()
    {
        if (m_sectorNumberText != null)
        {
            LeanTween.value(m_sectorNumber, 1, 0, animationDuration)
                .setOnUpdate((float alpha) =>
                {
                    m_sectorNumberText.color = new Color(m_sectorNumberText.color.r, m_sectorNumberText.color.g, m_sectorNumberText.color.b, alpha);
                })
                .setOnComplete(() => m_sectorNumber.SetActive(false));
        }

        if (m_sectorNameText != null)
        {
            LeanTween.value(m_sectorName, 1, 0, animationDuration)
                .setOnUpdate((float alpha) =>
                {
                    m_sectorNameText.color = new Color(m_sectorNameText.color.r, m_sectorNameText.color.g, m_sectorNameText.color.b, alpha);
                })
                .setOnComplete(() => m_sectorName.SetActive(false));
        }

        if (m_background != null)
        {
            LeanTween.value(gameObject, 0.4f, 0, animationDuration)
                .setOnUpdate((float alpha) =>
                {
                    Color bgColor = m_background.color;
                    bgColor.a = alpha;
                    m_background.color = bgColor;
                })
                .setEase(LeanTweenType.linear).setOnComplete(() =>
                {
                    if (SceneManager.GetActiveScene().name.Equals("Level_1"))
                    {
                        TutorialManager.Instance.ShowTip("Press <color=#00FFF6>WASD</color> to move and <color=#00FFF6>E</color> to interact");
                    }
                });
        }
    }
}

using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("Tip Popup (small)")]
    [SerializeField] private GameObject tipPanel;
    [SerializeField] private TextMeshProUGUI tipText;
    [SerializeField] private float tipDuration = 4f;
    [SerializeField] private float tipAnimDuration = 0.5f;
    [SerializeField] private LeanTweenType tipEaseIn = LeanTweenType.easeOutBack;
    [SerializeField] private LeanTweenType tipEaseOut = LeanTweenType.easeInBack;
    [SerializeField] private Vector2 tipHiddenPosition = new Vector2(0, -100);
    [SerializeField] private Vector2 tipShownPosition = new Vector2(0, 50);
    [SerializeField] private AudioClip m_tipopUpSFX;

    [Header("FullScreen Popup (large)")]
    [SerializeField] private GameObject fullScreenPanel;
    [SerializeField] private CanvasGroup fullScreenCanvasGroup;
    [SerializeField] private TextMeshProUGUI fullScreenText;
    [SerializeField] private TextMeshProUGUI m_fullScreenTextTitle;
    [SerializeField] private VideoPlayer fullScreenVideo;
    [SerializeField] private UnityEngine.UI.Image countdownFillImage;
    [SerializeField] private float fullScreenAnimDuration = 0.7f;
    [SerializeField] private LeanTweenType fullScreenEaseIn = LeanTweenType.easeOutExpo;
    [SerializeField] private LeanTweenType fullScreenEaseOut = LeanTweenType.easeInExpo;
    [SerializeField] private Transform fullScreenContentTransform;

    [Header("Interaction Prompt")]
    [SerializeField] private GameObject m_interactionPrompt;

    private bool m_isFullScreenActive = false;
    private bool m_isTipActive = false;
    private Coroutine m_autoCloseRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        InitializePanels();
    }

    private void InitializePanels()
    {
        if (tipPanel != null)
        {
            tipPanel.GetComponent<RectTransform>().anchoredPosition = tipHiddenPosition;
            tipPanel.SetActive(false);
        }

        if (fullScreenPanel != null)
        {
            if (fullScreenCanvasGroup == null)
                fullScreenCanvasGroup = fullScreenPanel.GetComponent<CanvasGroup>() ?? fullScreenPanel.AddComponent<CanvasGroup>();

            fullScreenCanvasGroup.alpha = 0f;
            fullScreenPanel.SetActive(false);

            if (fullScreenContentTransform != null)
            {
                fullScreenContentTransform.localScale = Vector3.zero;
            }
        }
    }

    public void ShowTip(string message, float duration = -1f)
    {
        if (m_isTipActive) return;
        m_isTipActive = true;

        tipText.text = message;
        tipPanel.SetActive(true);
        AudioManager.Instance.PlaySFX(m_tipopUpSFX , transform.position);

        LeanTween.cancel(tipPanel);

        LeanTween.move(tipPanel.GetComponent<RectTransform>(), tipShownPosition, tipAnimDuration)
            .setEase(tipEaseIn)
            .setIgnoreTimeScale(true);

        LeanTween.scale(tipPanel, Vector3.one * 1.05f, tipAnimDuration * 0.5f)
            .setEase(LeanTweenType.easeOutSine)
            .setIgnoreTimeScale(true)
            .setOnComplete(() => {
                LeanTween.scale(tipPanel, Vector3.one, tipAnimDuration * 0.5f)
                    .setEase(LeanTweenType.easeInSine)
                    .setIgnoreTimeScale(true);
            });

        LeanTween.delayedCall(tipPanel, duration > 0 ? duration : tipDuration, () =>
        {
            HideTip();
        }).setIgnoreTimeScale(true);
    }

    public void HideTip()
    {
        if (!m_isTipActive) return;

        LeanTween.move(tipPanel.GetComponent<RectTransform>(), tipHiddenPosition, tipAnimDuration)
            .setEase(tipEaseOut)
            .setIgnoreTimeScale(true)
            .setOnComplete(() => {
                tipPanel.SetActive(false);
                m_isTipActive = false;
                AudioManager.Instance.PlaySFX(m_tipopUpSFX);
            });
    }

    public void ShowFullScreen(string message, string title, VideoClip video = null, float autoCloseTime = 20f, Action onComplete = null)
    {
        if (m_isFullScreenActive) return;
        m_isFullScreenActive = true;

        if (m_isTipActive)
        {
            HideTip();
        }

        m_interactionPrompt.SetActive(false);

        fullScreenText.text = message;
        m_fullScreenTextTitle.text = title;
        fullScreenPanel.SetActive(true);

        if (video != null)
        {
            fullScreenVideo.clip = video;
            fullScreenVideo.gameObject.SetActive(true);
            fullScreenVideo.Play();
        }
        else
        {
            fullScreenVideo.Stop();
            fullScreenVideo.gameObject.SetActive(false);
        }

        if (countdownFillImage != null)
        {
            countdownFillImage.fillAmount = 1f;
        }

        Time.timeScale = 0f;

        LeanTween.alphaCanvas(fullScreenCanvasGroup, 1f, fullScreenAnimDuration * 0.5f)
            .setEase(fullScreenEaseIn)
            .setIgnoreTimeScale(true);

        if (fullScreenContentTransform != null)
        {
            fullScreenContentTransform.localScale = Vector3.zero;
            LeanTween.scale(fullScreenContentTransform.gameObject, Vector3.one, fullScreenAnimDuration)
                .setEase(fullScreenEaseIn)
                .setIgnoreTimeScale(true)
                .setOnStart(() => {
                    fullScreenContentTransform.localEulerAngles = new Vector3(0, 0, 5f);
                })
                .setOnUpdate((float val) => {
                    float rotation = Mathf.Sin(val * Mathf.PI * 2) * 2f;
                    fullScreenContentTransform.localEulerAngles = new Vector3(0, 0, rotation);
                })
                .setOnComplete(() => {
                    fullScreenContentTransform.localEulerAngles = Vector3.zero;
                });
        }

        if (m_autoCloseRoutine != null)
        {
            StopCoroutine(m_autoCloseRoutine);
        }
        m_autoCloseRoutine = StartCoroutine(AutoCloseTutorial(autoCloseTime, onComplete));

    }

    private IEnumerator AutoCloseTutorial(float duration, Action onComplete = null)
    {
        float timePassed = 0f;

        while (timePassed < duration)
        {
            timePassed += Time.unscaledDeltaTime;

            if (countdownFillImage != null)
            {
                countdownFillImage.fillAmount = 1f - (timePassed / duration);
            }

            yield return null;
        }

        HideFullScreen();
        onComplete?.Invoke();
    }


    public void HideFullScreen()
    {
        if (!m_isFullScreenActive) return;

        LeanTween.alphaCanvas(fullScreenCanvasGroup, 0f, fullScreenAnimDuration * 0.5f)
            .setEase(fullScreenEaseOut)
            .setIgnoreTimeScale(true);

        if (fullScreenContentTransform != null)
        {
            LeanTween.scale(fullScreenContentTransform.gameObject, Vector3.zero, fullScreenAnimDuration * 0.7f)
                .setEase(fullScreenEaseOut)
                .setIgnoreTimeScale(true)
                .setOnComplete(() => {
                    CompleteHideFullScreen();
                });
        }
        else
        {
            LeanTween.delayedCall(fullScreenAnimDuration * 0.5f, CompleteHideFullScreen)
                .setIgnoreTimeScale(true);
        }

        if (m_autoCloseRoutine != null)
        {
            StopCoroutine(m_autoCloseRoutine);
            m_autoCloseRoutine = null;
        }
    }

    private void CompleteHideFullScreen()
    {
        fullScreenPanel.SetActive(false);
        fullScreenVideo.Stop();
        fullScreenVideo.gameObject.SetActive(false);

        Time.timeScale = 1f;
        m_isFullScreenActive = false;
    }

    private void HideAll()
    {
        if (tipPanel != null)
        {
            tipPanel.SetActive(false);
            m_isTipActive = false;
        }

        if (fullScreenPanel != null)
        {
            fullScreenPanel.SetActive(false);
            m_isFullScreenActive = false;
            fullScreenVideo.Stop();
        }
    }
}
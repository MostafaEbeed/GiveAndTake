using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContextManager : MonoBehaviour
{
    public static ContextManager instance;

    [SerializeField] private OrbUIPopUp orbPopUI;
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private TextMeshProUGUI interactionText;
    [SerializeField] private OrbManager orbManager;
    private Image m_currentSectorBlocker = null;
    private SectorTitleManager m_sectorTitleManager;


    public OrbUIPopUp OrbUIPopUp => orbPopUI;
    public GameObject InteractionUI => interactionUI;
    public TextMeshProUGUI InteractionText => interactionText;
    public OrbManager OrbManager { get => orbManager; set => orbManager = value; }

    public Action<bool> OnMetalObjectsEnabledForInteraction;
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            if (instance != this)
                Destroy(gameObject);
        }
    }


    public void FadeInBlockerAndSetupSector(Image screenBlocker)
    {
        m_currentSectorBlocker = screenBlocker;

        LeanTween.value(gameObject, 1f, 0f, 1)
            .setOnUpdate((float val) =>
            {
                Color color = screenBlocker.color;
                color.a = val;
                screenBlocker.color = color;
            })
            .setEase(LeanTweenType.linear)
            .setOnComplete(() =>
            {
                screenBlocker.gameObject.SetActive(false);
                m_sectorTitleManager.AnimateSectorTitle();
            });
    }

    public void SetCurrentSectorTitleManager(SectorTitleManager manager) => m_sectorTitleManager = manager;

    public void EnableMetalObjectInteractablility(bool b)
    {
        OnMetalObjectsEnabledForInteraction?.Invoke(b);
    }
}

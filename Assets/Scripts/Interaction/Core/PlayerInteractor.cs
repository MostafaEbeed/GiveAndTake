using TMPro;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private Transform rayOrigin;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private AudioSource m_interactionSFXSource;

    [Header("UI")]
    private RectTransform interactionRect;
    private CanvasGroup interactionCanvasGroup;

    private IInteractable currentInteractable;
    private bool uiVisible = false;

    private void Start()
    {
        interactionRect = ContextManager.instance.InteractionUI.GetComponent<RectTransform>();
        interactionCanvasGroup = ContextManager.instance.InteractionUI.GetComponent<CanvasGroup>();

        if (interactionCanvasGroup == null)
        {
            interactionCanvasGroup = ContextManager.instance.InteractionUI.AddComponent<CanvasGroup>();
        }

        ContextManager.instance.InteractionUI.SetActive(false);
    }

    private void Update()
    {
        DetectInteractable();

        if (currentInteractable != null && Input.GetKeyDown(interactKey))
        {
            currentInteractable.Interact(GetComponent<PlayerController>());
        }
    }

    private void DetectInteractable()
    {
        Debug.DrawRay(rayOrigin.position, rayOrigin.forward * interactionRange, Color.green);

        if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out RaycastHit hit, interactionRange, interactableMask))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                currentInteractable = interactable;
                ContextManager.instance.InteractionText.text = interactable.GetInteractionMessage();
                if (!uiVisible)
                    ShowInteractionUI();

                return;
            }
        }

        currentInteractable = null;

        if (uiVisible)
            HideInteractionUI();
    }

    private void ShowInteractionUI()
    {
        uiVisible = true;
        m_interactionSFXSource.Play();
        ContextManager.instance.InteractionUI.SetActive(true);
        interactionCanvasGroup.alpha = 0f;
        interactionRect.localScale = Vector3.one * 0.8f;

        LeanTween.alphaCanvas(interactionCanvasGroup, 1f, 0.2f).setEase(LeanTweenType.easeOutExpo);
        LeanTween.scale(interactionRect, Vector3.one, 0.25f).setEase(LeanTweenType.easeOutBack);
    }

    private void HideInteractionUI()
    {
        uiVisible = false;
        LeanTween.alphaCanvas(interactionCanvasGroup, 0f, 0.2f).setEase(LeanTweenType.easeInExpo);
        LeanTween.scale(interactionRect, Vector3.one * 0.8f, 0.2f).setEase(LeanTweenType.easeInBack).setOnComplete(() =>
        {
            ContextManager.instance.InteractionUI.SetActive(false);
            m_interactionSFXSource.Play();
        });
    }
}

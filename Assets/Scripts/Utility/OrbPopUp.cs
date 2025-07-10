using UnityEngine;

public class OrbPopUp : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float m_popupDuration = 0.3f;
    [SerializeField] private float m_destroyDuration = 0.2f;
    [SerializeField] private LeanTweenType m_easeType = LeanTweenType.easeOutBack;

    private Vector3 m_initialScale;

    private void Awake()
    {
        m_initialScale = transform.localScale;
        transform.localScale = Vector3.zero;

        LeanTween.scale(gameObject, m_initialScale, m_popupDuration).setEase(m_easeType);
    }

    public void DestroyWithShrink()
    {
        LeanTween.scale(gameObject, Vector3.zero, m_destroyDuration)
                 .setEase(LeanTweenType.easeInBack)
                 .setOnComplete(() => Destroy(gameObject));
    }
}

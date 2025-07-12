using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    private Image m_screenBlockerImage;

    private void Start()
    {
        m_screenBlockerImage = GetComponent<Image>();
        ContextManager.instance.FadeInBlockerAndSetupSector(m_screenBlockerImage);
    }
}

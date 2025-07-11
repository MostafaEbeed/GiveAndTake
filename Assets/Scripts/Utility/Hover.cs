using UnityEngine;

public class Hover : MonoBehaviour
{
    [SerializeField] private float m_hoverMinY = 0.7f;
    [SerializeField] private float m_hoverMaxY = 1.0f;
    [SerializeField] private float m_hoverFrequency = 1f;

    private Vector3 m_originalPosition;
    private float m_timeOffset;

    private void Start()
    {
        m_originalPosition = transform.localPosition;
        m_timeOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    private void Update()
    {
        // Create a smooth oscillation from 0 to 1 using sine wave
        float t = (Mathf.Sin(Time.time * m_hoverFrequency + m_timeOffset) + 1f) * 0.5f;

        // Lerp between min and max hover height
        float hoverY = Mathf.Lerp(m_hoverMinY, m_hoverMaxY, t);

        // Apply new Y position while keeping original X and Z
        transform.localPosition = new Vector3(m_originalPosition.x, hoverY, m_originalPosition.z);
    }
}

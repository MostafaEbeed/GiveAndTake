using UnityEngine;

public class Hover : MonoBehaviour
{
    [SerializeField] private float m_hoverAmplitude = 0.1f;
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
        float hoverOffset = Mathf.Sin(Time.time * m_hoverFrequency + m_timeOffset) * m_hoverAmplitude;
        transform.localPosition = m_originalPosition + new Vector3(0f, hoverOffset, 0f);
    }
}


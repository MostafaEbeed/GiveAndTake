using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Vent : MonoBehaviour
{
    [Header("Mode Settings")]
    public VentMode mode = VentMode.Timed;

    [Header("Air Stream Settings")]
    public Transform airOrigin;
    public float airForce = 10f;
    public float airDuration = 5f;
    public LayerMask affectedLayers;
    private Dictionary<PlayerController, float> m_windForceProgress = new Dictionary<PlayerController, float>();
    [SerializeField] private float m_forceBuildUpSpeed = 1f;


    [Header("Air Stream Box Settings")]
    public Vector3 boxSize = new Vector3(1f, 1f, 5f);
    public float boxOffset = 2.5f;

    [Header("Timed Settings")]
    public float waitTime = 3f;

    [Header("Vent Flaps")]
    public List<Transform> flaps;
    public float openAngle = 40f;
    public float openCloseDuration = 0.3f;

    [Header("Vent UI")]
    [SerializeField] private TextMeshProUGUI m_timerUI;
    [SerializeField] private GameObject m_airIconUI;

    [Header("Vent VFX")]
    [SerializeField] private GameObject windVFXPrefab;
    private GameObject currentVFXInstance;

    private bool m_isOpen = false;

    private void Start()
    {
        if (mode == VentMode.AlwaysOn)
        {
            StartCoroutine(StreamAirForever());
        }
        else
        {
            StartCoroutine(TimedVentRoutine());
        }
    }

    private IEnumerator TimedVentRoutine()
    {
        while (true)
        {
            float timer = waitTime;
            m_timerUI.gameObject.SetActive(true);
            m_airIconUI.SetActive(false);

            while (timer > 0f)
            {
                m_timerUI.text = timer.ToString("0.0") + "s";
                timer -= Time.deltaTime;
                yield return null;
            }

            m_timerUI.gameObject.SetActive(false);
            m_airIconUI.SetActive(true);

            if (windVFXPrefab != null && airOrigin != null)
            {
                currentVFXInstance = Instantiate(windVFXPrefab, airOrigin);
                currentVFXInstance.transform.localPosition = Vector3.zero;
                currentVFXInstance.transform.localRotation = Quaternion.identity;
                currentVFXInstance.transform.localScale = Vector3.one * 0.1f;
            }

            yield return StartCoroutine(OpenFlaps());
            yield return StartCoroutine(StreamAir(airDuration));
            yield return StartCoroutine(CloseFlaps());

            m_airIconUI.SetActive(false);
            if (currentVFXInstance != null)
            {
                Destroy(currentVFXInstance);
            }
        }
    }


    private IEnumerator StreamAirForever()
    {
        m_airIconUI.SetActive(true);
        m_timerUI.gameObject.SetActive(false);

        yield return StartCoroutine(OpenFlaps());

        if (windVFXPrefab != null && airOrigin != null)
        {
            currentVFXInstance = Instantiate(windVFXPrefab, airOrigin);
            currentVFXInstance.transform.localPosition = Vector3.zero;
            currentVFXInstance.transform.localRotation = Quaternion.identity;
            currentVFXInstance.transform.localScale = Vector3.one * 0.1f;
        }

        while (true)
        {
            ApplyAirForce();
            yield return null;
        }
    }


    private IEnumerator StreamAir(float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            ApplyAirForce();
            timer += Time.deltaTime;
            yield return null;
        }
    }

    private void ApplyAirForce()
    {
        Vector3 boxCenter = airOrigin.position + airOrigin.forward * boxOffset;
        Quaternion boxRotation = airOrigin.rotation;

        Collider[] hits = Physics.OverlapBox(boxCenter, boxSize * 0.5f, boxRotation, affectedLayers);

        foreach (var hit in hits)
        {
            var rb = hit.attachedRigidbody;
            var controller = hit.GetComponent<CharacterController>();
            var player = hit.GetComponent<PlayerController>();

            if (rb != null)
            {
                rb.AddForce(airOrigin.forward * airForce, ForceMode.Acceleration);
            }

            else if (controller != null && player != null)
            {
                if (player.CurrentOrb is WeightOrbBehavior)
                    continue;

                if (!m_windForceProgress.ContainsKey(player))
                    m_windForceProgress[player] = 0f;

                m_windForceProgress[player] = Mathf.Clamp01(m_windForceProgress[player] + Time.deltaTime * m_forceBuildUpSpeed);
                float effectiveForce = airForce * m_windForceProgress[player];

                player.ApplyExternalForce(airOrigin.forward * effectiveForce);
            }
        }

        CleanupForceProgress(hits);
    }


    private void CleanupForceProgress(Collider[] currentHits)
    {
        HashSet<PlayerController> hitThisFrame = new HashSet<PlayerController>();
        foreach (var hit in currentHits)
        {
            var player = hit.GetComponent<PlayerController>();
            if (player != null)
                hitThisFrame.Add(player);
        }

        var keys = new List<PlayerController>(m_windForceProgress.Keys);
        foreach (var player in keys)
        {
            if (!hitThisFrame.Contains(player))
            {
                m_windForceProgress[player] = 0f;
            }
        }
    }


    private IEnumerator OpenFlaps()
    {
        m_isOpen = true;
        yield return AnimateFlaps(0f, openAngle);
    }

    private IEnumerator CloseFlaps()
    {
        m_isOpen = false;
        yield return AnimateFlaps(openAngle, 0f);
    }

    private IEnumerator AnimateFlaps(float from, float to)
    {
        float time = 0f;
        while (time < openCloseDuration)
        {
            float angle = Mathf.Lerp(from, to, time / openCloseDuration);
            foreach (var flap in flaps)
            {
                Vector3 rot = flap.localEulerAngles;
                rot.y = angle;
                flap.localEulerAngles = rot;
            }
            time += Time.deltaTime;
            yield return null;
        }

        foreach (var flap in flaps)
        {
            Vector3 rot = flap.localEulerAngles;
            rot.y = to;
            flap.localEulerAngles = rot;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (airOrigin != null)
        {
            Gizmos.color = Color.cyan;
            Vector3 boxCenter = airOrigin.position + airOrigin.forward * boxOffset;
            Gizmos.matrix = Matrix4x4.TRS(boxCenter, airOrigin.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, boxSize);
        }
    }


}

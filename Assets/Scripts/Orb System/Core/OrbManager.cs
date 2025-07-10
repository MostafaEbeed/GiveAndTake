using System.Collections.Generic;
using UnityEngine;

public class OrbManager : MonoBehaviour
{
    [SerializeField] private List<OrbData> m_orbDatas;
    [SerializeField] private Transform m_orbSlot;

    private GameObject m_currentVisual;
    private OrbBehavior m_currentBehavior;
    private int m_currentIndex = 0;
    private PlayerController m_player;

    private void Start()
    {
        m_player = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            EquipOrb(0);
            //int nextIndex = (m_currentIndex + 1) % m_orbDatas.Count;
            //EquipOrb(nextIndex);
        }

        m_currentBehavior?.OnUpdate();
    }

    private void EquipOrb(int index)
    {
        if (m_currentBehavior != null)
            m_currentBehavior.OnUnequip();

        if (m_currentVisual)
            m_currentVisual.GetComponent<OrbPopUp>()?.DestroyWithShrink();

        m_currentIndex = index;
        OrbData data = m_orbDatas[m_currentIndex];

        if (data.visualPrefab)
        {
            m_currentVisual = Instantiate(data.visualPrefab, m_orbSlot);
            m_currentBehavior = m_currentVisual.GetComponent<OrbBehavior>();

            if (m_currentBehavior == null)
            {
                Debug.LogWarning($"Orb prefab '{data.visualPrefab.name}' is missing an OrbBehavior component!");
                return;
            }

            m_currentBehavior.Initialize(m_player, data);
            m_currentBehavior.OnEquip();
        }
        else
        {
            Debug.LogWarning($"OrbData '{data.name}' has no visual prefab assigned.");
        }
    }
}

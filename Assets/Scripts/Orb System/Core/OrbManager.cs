using System.Collections.Generic;
using UnityEngine;

public class OrbManager : MonoBehaviour
{
    [SerializeField] private OrbData m_defaultOrbBehavior;
    [SerializeField] private List<OrbData> m_orbDatas;
    [SerializeField] private Transform m_orbSlot;

    private GameObject m_currentVisual;
    private OrbBehavior m_currentBehavior;
    private int m_currentIndex = -1;
    private PlayerController m_player;

    [SerializeField] private OrbUIPopUp orbUIPopup;
    [SerializeField] private AudioSource m_equipOrbSFXSource;

    private void Start()
    {
        m_player = GetComponent<PlayerController>();
        EquipDefaultOrb();

        orbUIPopup = ContextManager.instance.OrbUIPopUp;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipOrb(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipOrb(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) EquipOrb(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) EquipOrb(3);

        if (Input.GetKeyDown(KeyCode.X)) EquipDefaultOrb();

        m_currentBehavior?.OnUpdate();
    }

    private void UnequipCurrentOrb()
    {
        if (m_currentBehavior != null)
        {
            m_currentBehavior.OnUnequip();
            m_currentBehavior = null;
        }

        if (m_currentVisual)
        {
            m_currentVisual.GetComponent<OrbPopUp>()?.DestroyWithShrink();
            Destroy(m_currentVisual);
            m_currentVisual = null;
        }

        m_currentIndex = -1;
    }

    private void EquipOrb(int index)
    {
        if (index < 0 || index >= m_orbDatas.Count)
        {
            Debug.LogWarning("Tried to equip orb with invalid index: " + index);
            return;
        }

        if (index == m_currentIndex)
        {
            return;
        }

        UnequipCurrentOrb();

        m_currentIndex = index;
        OrbData data = m_orbDatas[m_currentIndex];

        SpawnOrb(data);
    }


    private void EquipDefaultOrb()
    {
        UnequipCurrentOrb();

        if (m_defaultOrbBehavior == null)
        {
            Debug.LogWarning("No default orb assigned.");
            return;
        }

        if (orbUIPopup != null)
        {
            orbUIPopup.HideInstantly();
        }

        SpawnOrb(m_defaultOrbBehavior);
    }


    private void SpawnOrb(OrbData data)
    {
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

            if(data != m_defaultOrbBehavior)
            {
                if (orbUIPopup != null)
                {
                    orbUIPopup.Show(data.uiGiveMessage, data.uiTakeMessage);
                    m_equipOrbSFXSource.Play();
                }
            }
        }
        else
        {
            Debug.LogWarning($"OrbData '{data.name}' has no visual prefab assigned.");
        }
    }

    public void PickUpAndEquip(OrbData newOrb)
    {
        if (newOrb == null)
        {
            Debug.LogWarning("Tried to pick up a null OrbData.");
            return;
        }

        if (!m_orbDatas.Contains(newOrb))
        {
            m_orbDatas.Add(newOrb);
        }

        EquipOrb(m_orbDatas.IndexOf(newOrb));
    }


}

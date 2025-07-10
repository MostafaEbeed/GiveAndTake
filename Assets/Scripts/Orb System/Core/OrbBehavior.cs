using UnityEngine;

public abstract class OrbBehavior : MonoBehaviour
{
    protected PlayerController m_player;
    public OrbData Data;

    public void Initialize(PlayerController player, OrbData data)
    {
        m_player = player;
        Data = data;
    }

    public virtual void OnEquip() { }
    public virtual void OnUpdate() { }
    public virtual void OnUnequip() { }
}

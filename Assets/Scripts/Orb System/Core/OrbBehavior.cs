using UnityEngine;

public abstract class OrbBehavior : MonoBehaviour
{
    protected PlayerController m_player;
    public OrbData Data;

    protected float moveInput;
    protected float turnInput;
    
    public void Initialize(PlayerController player, OrbData data)
    {
        m_player = player;
        Data = data;
    }

    public virtual void OnEquip()
    {
        m_player.IsJumpLocked = Data.disableJump;
        m_player.IsMovementLocked = Data.disableMovement;
        m_player.IsLookLocked = Data.disableLook;
        m_player.CurrentOrb = this;

        m_player.currentSpeedMultiplier = Data.moveSpeedMultiplier;
        m_player.JumpHeight = m_player.IsJumpLocked ? 0f : m_player.JumpHeight * Data.jumpHeightMultiplier;
        m_player.ExtraGravityMultiplier = Data.extraGravityMultiplier;

        if (Data.equipSound)
            AudioSource.PlayClipAtPoint(Data.equipSound, m_player.transform.position);
    }
    public virtual void OnUpdate() { }
    public virtual void OnUnequip() { }
    
    protected void InputManagement()
    {
        moveInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");
    }
}

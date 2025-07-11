using UnityEngine;

public class WeightOrbBehavior : OrbBehavior
{
    GroundCheck playerGroundCheck;
    
    public override void OnEquip()
    {
        base.OnEquip();
        
        playerGroundCheck = m_player.GetComponent<GroundCheck>();
        
        m_player.IsMovementLocked = Data.disableMovement;
        m_player.IsJumpLocked = Data.disableJump;
        m_player.IsLookLocked = Data.disableLook;

        m_player.currentSpeedMultiplier = Data.moveSpeedMultiplier;
        m_player.JumpHeight = m_player.IsJumpLocked ? 0f : m_player.JumpHeight * Data.jumpHeightMultiplier;
        m_player.ExtraGravityMultiplier = Data.extraGravityMultiplier;
        m_player.CurrentOrb = this;

        if (Data.equipSound)
            AudioSource.PlayClipAtPoint(Data.equipSound, m_player.transform.position);

        m_player.ShakeCamera(0.5f,5,0.3f);
    }

    public override void OnUpdate()
    {
        if (!m_player.controller.isGrounded)
        {
            m_player.ExtraGravityMultiplier = ((WeightOrbData)Data).slamGravityMultiplier;
        }
        
        InputManagement();
        
        m_player.InputManagement(moveInput, turnInput);

        if (playerGroundCheck.IsGlassGrounded())
        {
            playerGroundCheck.TryDestroyCurrentFloor();
        }
    }

    public override void OnUnequip()
    {
        if (m_player.CurrentOrb == this)
            m_player.CurrentOrb = null;

        m_player.IsMovementLocked = false;
        m_player.IsJumpLocked = false;
        m_player.IsLookLocked = false;

        m_player.currentSpeedMultiplier = 1f;
        m_player.JumpHeight = 2f;
        m_player.ExtraGravityMultiplier = 1f;
    }

}

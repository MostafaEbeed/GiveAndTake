using UnityEngine;

public class MagentOrbBehavior : OrbBehavior
{
    GroundCheck playerGroundCheck;
    
    public override void OnEquip()
    {
        base.OnEquip();

        playerGroundCheck = m_player.GetComponent<GroundCheck>();
        
        if (playerGroundCheck.IsMetalGrounded())
        {
            m_player.IsMovementLocked = Data.disableMovement;
            m_player.IsJumpLocked = Data.disableJump;
            m_player.IsLookLocked = Data.disableLook;

            m_player.currentSpeedMultiplier = Data.moveSpeedMultiplier;
            m_player.JumpHeight = m_player.IsJumpLocked ? 0f : m_player.JumpHeight * Data.jumpHeightMultiplier;
            m_player.ExtraGravityMultiplier = Data.extraGravityMultiplier;
            m_player.CurrentOrb = this; 
        }
        else
        {
            m_player.IsMovementLocked = false;
            m_player.IsJumpLocked = false;
            m_player.IsLookLocked = false;

            m_player.currentSpeedMultiplier = Data.moveSpeedMultiplier;
            m_player.JumpHeight = m_player.IsJumpLocked ? 0f : m_player.JumpHeight * Data.jumpHeightMultiplier;
            m_player.ExtraGravityMultiplier = Data.extraGravityMultiplier;
            m_player.CurrentOrb = this; 
        }

        if (Data.equipSound)
            AudioSource.PlayClipAtPoint(Data.equipSound, m_player.transform.position);

        m_player.ShakeCamera(0.5f,5,0.3f);
    }   
    
    public override void OnUpdate()
    {
        if (playerGroundCheck.IsMetalGrounded())
        {
            moveInput = 0;
            turnInput = 0;
            m_player.IsJumpLocked = true;
        }
        else
        {
            InputManagement();
        }
        
        m_player.InputManagement(moveInput, turnInput);
    }
}
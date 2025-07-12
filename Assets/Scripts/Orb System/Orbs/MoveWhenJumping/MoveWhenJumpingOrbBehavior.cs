public class MoveWhenJumpingOrbBehavior : OrbBehavior
{
    private bool hasLeftGroundSinceLastBounce = true;

    private float m_localMoveInput;
    private float m_localTurnInput;

    public override void OnUpdate()
    {
        InputManagement();

        /*m_localMoveInput = moveInput;
        m_localTurnInput = turnInput;

        bool isGrounded = m_player.controller.isGrounded;

        if (isGrounded && hasLeftGroundSinceLastBounce)
        {
            m_player.ForceBounce(Data.jumpHeightMultiplier);
            hasLeftGroundSinceLastBounce = false;
        }

        if (!isGrounded)
        {
            hasLeftGroundSinceLastBounce = true;
        }

        m_player.InputManagement(m_localMoveInput, m_localTurnInput);*/
        
        
        if (m_player.controller.isGrounded)
        {
            m_localMoveInput = 0;
            m_localTurnInput = 0;
        }
        else
        {
            m_localMoveInput = moveInput;
            m_localTurnInput = turnInput;
        }
        
        m_player.InputManagement(m_localMoveInput, m_localTurnInput);
    }

    public override void OnEquip()
    {
        base.OnEquip();

        /*if (m_player.controller.isGrounded)
        {
            m_player.ForceBounce(Data.jumpHeightMultiplier);
            hasLeftGroundSinceLastBounce = false;
        }
        else
        {
            hasLeftGroundSinceLastBounce = true;
        }*/
        
        m_player.IsMovementLocked = Data.disableMovement;
        m_player.IsJumpLocked = Data.disableJump;
        m_player.IsLookLocked = Data.disableLook;

        m_player.currentSpeedMultiplier = Data.moveSpeedMultiplier;
        m_player.JumpHeight = m_player.IsJumpLocked ? 0f : m_player.JumpHeight * Data.jumpHeightMultiplier;
        m_player.ExtraGravityMultiplier = Data.extraGravityMultiplier;
        m_player.CurrentOrb = this;

        if (Data.equipSound)
            AudioManager.Instance.PlaySFX(Data.equipSound, m_player.transform.position);

        m_player.ShakeCamera(0.5f,5,0.3f);
    }

    public override void OnUnequip()
    {
        if (m_player.CurrentOrb == this)
            m_player.CurrentOrb = null;

        m_player.IsJumpLocked = false;
        m_player.IsMovementLocked = false;
    }
}

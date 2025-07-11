public class MoveWhenJumpingOrbBehavior : OrbBehavior
{
    private bool hasLeftGroundSinceLastBounce = true;

    private float m_localMoveInput;
    private float m_localTurnInput;

    public override void OnUpdate()
    {
        InputManagement();

        m_localMoveInput = moveInput;
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

        m_player.InputManagement(m_localMoveInput, m_localTurnInput);
    }

    public override void OnEquip()
    {
        base.OnEquip();

        if (m_player.controller.isGrounded)
        {
            m_player.ForceBounce(Data.jumpHeightMultiplier);
            hasLeftGroundSinceLastBounce = false;
        }
        else
        {
            hasLeftGroundSinceLastBounce = true;
        }
    }

    public override void OnUnequip()
    {
        if (m_player.CurrentOrb == this)
            m_player.CurrentOrb = null;

        m_player.IsJumpLocked = false;
        m_player.IsMovementLocked = false;
    }
}

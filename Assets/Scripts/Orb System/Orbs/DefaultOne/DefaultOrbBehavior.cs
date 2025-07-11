
public class DefaultOrbBehavior : OrbBehavior
{
    public override void OnEquip()
    {
        base.OnEquip();
    }

    public override void OnUpdate()
    {
        InputManagement();
        
        m_player.InputManagement(moveInput, turnInput);
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

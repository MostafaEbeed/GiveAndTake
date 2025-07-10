using UnityEngine;

public class WeightOrbBehavior : OrbBehavior
{
    private bool m_wasGrounded;

    public override void OnEquip()
    {
        m_player.IsMovementLocked = Data.disableMovement;
        m_player.IsJumpLocked = Data.disableJump;
        m_player.IsLookLocked = Data.disableLook;

        m_player.currentSpeedMultiplier = Data.moveSpeedMultiplier;
        m_player.JumpHeight = m_player.IsJumpLocked ? 0f : m_player.JumpHeight * Data.jumpHeightMultiplier;
        m_player.ExtraGravityMultiplier = Data.extraGravityMultiplier;

        if (Data.equipSound)
            AudioSource.PlayClipAtPoint(Data.equipSound, m_player.transform.position);
    }

    public override void OnUpdate()
    {
        if (!m_player.controller.isGrounded)
        {
            m_player.ExtraGravityMultiplier = ((WeightOrbData)Data).slamGravityMultiplier;
        }

        m_wasGrounded = m_player.controller.isGrounded;
    }

    public override void OnUnequip()
    {
        m_player.IsMovementLocked = false;
        m_player.IsJumpLocked = false;
        m_player.IsLookLocked = false;

        m_player.currentSpeedMultiplier = 1f;
        m_player.JumpHeight = 2f;
        m_player.ExtraGravityMultiplier = 1f;
    }

}

using UnityEngine;

public class DashOrbBehavior : OrbBehavior
{
    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float dashCooldown = 0.5f;

    [Header("FOV Settings")]
    [SerializeField] private float normalFOV = 60f;
    [SerializeField] private float dashFOV = 75f;
    [SerializeField] private float fovReturnSpeed = 5f;
    private float currentTargetFOV;
    private float currentFOVVelocity = 0f;


    private float dashTimer = 0f;

    public override void OnUpdate()
    {
        m_player.InputManagement(0f, 0f);
        m_player.IsMovementLocked = true;
        m_player.IsJumpLocked = true;
        m_player.IsLookLocked = true;

        dashTimer += Time.deltaTime;
        if (dashTimer >= dashCooldown)
        {
            DashForward();
            dashTimer = 0f;
        }

        var lens = m_player.virtualCamera.Lens;
        lens.FieldOfView = Mathf.SmoothDamp(lens.FieldOfView, currentTargetFOV, ref currentFOVVelocity, 0.15f);
        m_player.virtualCamera.Lens = lens;
    }


    private void DashForward()
    {
        Vector3 forward = m_player.virtualCamera.transform.forward;
        forward.y = 0;
        forward.Normalize();

        m_player.ApplyExternalForce(forward * dashForce);

        currentTargetFOV = dashFOV;
        Invoke(nameof(ResetTargetFOV), 0.1f);
    }

    private void ResetTargetFOV()
    {
        currentTargetFOV = normalFOV;
    }


    public override void OnEquip()
    {
        base.OnEquip();
        currentTargetFOV = normalFOV;
        dashTimer = dashCooldown;
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

        m_player.ApplyExternalForce(-m_player.controller.velocity);

        var lens = m_player.virtualCamera.Lens;
        lens.FieldOfView = normalFOV;
        m_player.virtualCamera.Lens = lens;
    }
}

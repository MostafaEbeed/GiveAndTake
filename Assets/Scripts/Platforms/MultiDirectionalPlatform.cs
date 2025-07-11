using UnityEngine;

public class MultiDirectionalPlatform : MovingPlatform
{
    [Header("Multi-Direction Settings")]
    public Vector3 customDirection = Vector3.forward;
    public bool normalizeDirection = true;
    
    protected override void CalculateTargetPosition()
    {
        Vector3 direction = normalizeDirection ? customDirection.normalized : customDirection;
        targetPosition = startPosition + direction * moveDistance;
    }
    
    public override Vector3 GetMoveDirection()
    {
        return normalizeDirection ? customDirection.normalized : customDirection;
    }

    public Vector3 GetMoveDir()
    {
        return GetMoveDirection();
    }
}
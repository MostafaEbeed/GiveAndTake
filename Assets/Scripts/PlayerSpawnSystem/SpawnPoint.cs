using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Header("Spawn Point Settings")]
    public Color gizmoColor = Color.green;
    public float gizmoSize = 1f;
    
    private void OnDrawGizmos()
    {
        // Draw a visual indicator in the scene view
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position, gizmoSize);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * gizmoSize);
    }
}
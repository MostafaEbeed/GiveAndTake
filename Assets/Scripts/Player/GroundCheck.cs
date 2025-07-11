using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [Header("Ground Check")]
    public float groundDistance = 0.2f;
    public LayerMask groundMask;
    
    private bool isGrounded;
    
    void Update()
    {
        // Cast a ray downward from the character's position
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundDistance, groundMask);
    }
    
    public bool IsGrounded()
    {
        return isGrounded;
    }
    
    // Optional: Draw the ray in the scene view for debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * groundDistance);
    }
}

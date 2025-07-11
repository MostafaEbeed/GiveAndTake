using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [Header("Ground Check")]
    public float groundDistance = 0.2f;
    public LayerMask groundMask;
    public LayerMask metalGroundMask;
    
    private bool isGrounded;
    private bool isMetalGrounded;
    
    void Update()
    {
        // Cast a ray downward from the character's position
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundDistance, groundMask);
        isMetalGrounded = Physics.Raycast(transform.position, Vector3.down, groundDistance, metalGroundMask);
    }
    
    public bool IsGrounded()
    {
        return isGrounded;
    }
    
    public bool IsMetalGrounded()
    {
        return isMetalGrounded;
    }
    
    // Optional: Draw the ray in the scene view for debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * groundDistance);
    }
}

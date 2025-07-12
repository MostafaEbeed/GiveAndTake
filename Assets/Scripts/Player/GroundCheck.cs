using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [Header("Ground Check")]
    public float groundDistance = 0.2f;
    public LayerMask groundMask;
    public LayerMask metalGroundMask;
    public LayerMask glassGroundMask;
    
    private bool isGrounded;
    private bool isMetalGrounded;
    private bool isGlassGrounded;
    
    void Update()
    {
        // Cast a ray downward from the character's position
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundDistance, groundMask);
        isMetalGrounded = Physics.Raycast(transform.position, Vector3.down, groundDistance, metalGroundMask);
        isGlassGrounded = Physics.Raycast(transform.position, Vector3.down, groundDistance, glassGroundMask);
    }
    
    public bool IsGrounded()
    {
        return isGrounded;
    }
    
    public bool IsMetalGrounded()
    {
        return isMetalGrounded;
    }
    
    public bool IsGlassGrounded()
    {
        return isGlassGrounded;
    }
    
    // Optional: Draw the ray in the scene view for debugging
    void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * groundDistance);
    }

    public void TryDestroyCurrentFloor()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundDistance, glassGroundMask))
        {
            if (hit.collider.gameObject != null)
            {
                GlassFloor glassFloor = hit.collider.gameObject.GetComponent<GlassFloor>();
                
                if(glassFloor != null)
                    glassFloor.StartClassFractureAnim();
                
                //Destroy(hit.collider.gameObject);
            }
        }
    }
}

using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damageAmount = 50f;
    public bool killInstantly = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                if (killInstantly)
                {
                    playerHealth.KillPlayer();
                }
                else
                {
                    playerHealth.TakeDamage(damageAmount);
                }
            }
        }
    }
}
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    
    // Event that gets called when player dies
    public System.Action onPlayerDeath;
    
    public UnityEvent OnPlayerDeath;
    
    private void Start()
    {
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }
    
    private void Die()
    {
        Debug.Log("Player died!");
        
        OnPlayerDeath?.Invoke();
        
        // Trigger the death event
        onPlayerDeath?.Invoke();
    }
    
    // Optional: Method to kill player instantly (for testing)
    [ContextMenu("Kill Player")]
    public void KillPlayer()
    {
        Die();
    }
}
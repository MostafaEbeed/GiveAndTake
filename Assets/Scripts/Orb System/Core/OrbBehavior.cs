using UnityEngine;

public abstract class OrbBehavior : MonoBehaviour
{
    protected PlayerController m_player;
    public OrbData Data;

    protected float moveInput;
    protected float turnInput;
    protected float mouseX;
    protected float mouseY;
    
    public void Initialize(PlayerController player, OrbData data)
    {
        m_player = player;
        Data = data;
    }

    public virtual void OnEquip()
    {
        Debug.Log($"Entering {GetType().Name}");
    }
    public virtual void OnUpdate() { }
    public virtual void OnUnequip() { }
    
    protected void InputManagement()
    {
        moveInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
    }
}

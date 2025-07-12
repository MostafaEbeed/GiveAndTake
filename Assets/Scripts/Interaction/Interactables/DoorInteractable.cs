using UnityEngine;
using UnityEngine.Events;

public class DoorInteractable : InteractableBase
{
    public UnityEvent OnDoorOpen;
    
    public override void Interact(PlayerController interactor)
    {
        Debug.Log("Interacting with " + interactor.name);
        
        OnDoorOpen?.Invoke();
    }
}

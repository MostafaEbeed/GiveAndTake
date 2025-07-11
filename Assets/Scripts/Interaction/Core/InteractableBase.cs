using UnityEngine;

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    [SerializeField] private string interactionMessage = "Interact";

    public string GetInteractionMessage() => interactionMessage;

    public abstract void Interact(PlayerController interactor);
}

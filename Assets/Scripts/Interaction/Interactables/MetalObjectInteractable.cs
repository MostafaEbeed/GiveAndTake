using System;
using UnityEngine;

public class MetalObjectInteractable : InteractableBase
{
    Collider currentCollider;
    private SlidingDoorPlatform slidingDoor;
    
    
    private void Awake()
    {
        currentCollider = GetComponent<Collider>();
        slidingDoor = GetComponent<SlidingDoorPlatform>();
    }

    private void Start()
    {
        ContextManager.instance.OnMetalObjectsEnabledForInteraction += ControlInteraction;
    }

    private void OnDestroy()
    {
        ContextManager.instance.OnMetalObjectsEnabledForInteraction -= ControlInteraction;
    }

    public override void Interact(PlayerController interactor)
    {
        slidingDoor.OpenDoor();
    }

    private void ControlInteraction(bool b)
    {
        currentCollider.enabled = b;
    }
    
}
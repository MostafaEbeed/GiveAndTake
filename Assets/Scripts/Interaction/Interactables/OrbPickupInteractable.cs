using UnityEngine;

public class OrbPickupInteractable : InteractableBase
{
    [SerializeField] private OrbManager m_orbManager;
    [SerializeField] private OrbData m_orbToPickUp;

    public override void Interact(PlayerController interactor)
    {
        m_orbManager.PickUpAndEquip(m_orbToPickUp);
    }
}

using UnityEngine;
using UnityEngine.Video;

public class OrbPickupInteractable : InteractableBase
{
    [SerializeField] private OrbManager m_orbManager;
    [SerializeField] private OrbData m_orbToPickUp;
    [SerializeField] private string m_pickedOrbtutorialTextMesh;
    [SerializeField] private VideoClip m_orbTutorialClip;

    public override void Interact(PlayerController interactor)
    {
        m_orbManager.PickUpAndEquip(m_orbToPickUp);
        TutorialManager.Instance.ShowFullScreen(m_pickedOrbtutorialTextMesh, m_orbToPickUp.orbName, m_orbTutorialClip);
        LeanTween.scale(gameObject , Vector3.zero , 0.5f).setOnComplete( () => { Destroy(gameObject); });
    }
}

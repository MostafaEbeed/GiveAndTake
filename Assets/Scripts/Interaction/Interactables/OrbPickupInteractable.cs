using Unity.VisualScripting;
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
        if (ContextManager.instance.OrbManager == null)
        {
            ContextManager.instance.OrbManager = FindAnyObjectByType<OrbManager>();
        }

        ContextManager.instance.OrbManager.PickUpAndEquip(m_orbToPickUp);
        TutorialManager.Instance.ShowFullScreen(m_pickedOrbtutorialTextMesh, m_orbToPickUp.orbName, m_orbTutorialClip ,20,ShowAcquiredOrbTip);
        LeanTween.scale(gameObject , Vector3.zero , 0.5f).setOnComplete( () => { Destroy(gameObject); });
    }

    private void ShowAcquiredOrbTip()
    {
        TutorialManager.Instance.ShowTip(m_orbToPickUp.orbAcquireTip);
    }
}

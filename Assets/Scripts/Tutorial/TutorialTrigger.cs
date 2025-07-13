using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] string TipToDisplay;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TutorialManager.Instance.ShowTip(TipToDisplay);
            gameObject.SetActive(false);
        }
    }
}

using System;
using TMPro;
using UnityEngine;

public class ContextManager : MonoBehaviour
{
    public static ContextManager instance;

    [SerializeField] private OrbUIPopUp orbPopUI;
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private TextMeshProUGUI interactionText;
    
    public OrbUIPopUp OrbUIPopUp => orbPopUI;
    public GameObject InteractionUI => interactionUI;
    public TextMeshProUGUI InteractionText => interactionText;
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            if (instance != this)
                Destroy(gameObject);
        }
        
        DontDestroyOnLoad(gameObject);
    }
}

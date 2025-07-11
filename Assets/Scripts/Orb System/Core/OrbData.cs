using UnityEngine;

public class OrbData : ScriptableObject
{
    public string orbName;
    public GameObject visualPrefab;
    public AudioClip equipSound;

    public string uiTakeMessage;
    public string uiGiveMessage;

    public float moveSpeedMultiplier = 1f;
    public float jumpHeightMultiplier = 1f;
    public float extraGravityMultiplier = 1f;

    public bool disableMovement = false;
    public bool disableJump = false;
    public bool disableLook = false;
}

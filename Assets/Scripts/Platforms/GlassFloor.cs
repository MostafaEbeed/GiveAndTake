using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassFloor : MonoBehaviour
{
    [SerializeField] private GameObject glassWhole;
    [SerializeField] private GameObject glass;
    [SerializeField] private ParticleSystem glassPS;

    public void StartClassFractureAnim()
    {
        glassWhole.SetActive(false);
        GetComponent<Collider>().enabled = false;
        glass.SetActive(true);
        glassPS.Play();

        StartCoroutine(DestroyGlassObject());
    }

    IEnumerator DestroyGlassObject()
    {
        yield return new WaitForSeconds(3f);
        
        Destroy(gameObject);
    }
}

using UnityEngine;
using System.Collections;

public class PortalEntryEffect : MonoBehaviour
{
    public float lifetime = 1f; // tempo da animação de entrada

    private void Start()
    {
        StartCoroutine(DestroyAfterTime());
    }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}

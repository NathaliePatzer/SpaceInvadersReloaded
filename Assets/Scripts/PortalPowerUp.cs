using UnityEngine;
using System.Collections;

public class PortalPowerUp : MonoBehaviour
{
    public GameObject portalPrefab;
    public float portalDuration = 10f;
    public GameObject entryEffectPrefab;
    public float entryEffectDuration = 0.8f;

    private bool collected = false;
    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected || !other.CompareTag("Player")) return;

        collected = true;

        Vector3 portalPosition = new Vector3(0f, -1.63f, 0f);
        GameObject existingPortal = GameObject.FindWithTag("Portal");

        if (existingPortal == null)
        {
            StartCoroutine(SpawnPortalSequence(portalPosition));
        }
        else
        {
            existingPortal.GetComponent<PortalController>().StartPortal(portalDuration);
        }

        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        GetComponent<Collider2D>().enabled = false;

        if (audioSource != null)
        {
            audioSource.Play();
            StartCoroutine(StopSoundAfter(2f));
        }

        if (animator != null)
        {
            animator.SetTrigger("Colided");
        }

        StartCoroutine(DestroyAfterFade());
    }

    private IEnumerator SpawnPortalSequence(Vector3 portalPosition)
    {
        Instantiate(entryEffectPrefab, portalPosition, Quaternion.identity);
        yield return new WaitForSeconds(entryEffectDuration);

        GameObject portal = Instantiate(portalPrefab, portalPosition, Quaternion.identity);
        portal.GetComponent<PortalController>().StartPortal(portalDuration);
    }

    private IEnumerator StopSoundAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.Stop();
    }

    private IEnumerator DestroyAfterFade()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}

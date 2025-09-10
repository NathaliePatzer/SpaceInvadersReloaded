using System.Collections;
using UnityEngine;

public class ExtraLifePowerUp : MonoBehaviour
{
    public GameObject specialHeartPrefab;
    public GameObject heartSpawnEffectPrefab;
    public float effectDuration = 15f;
    public Vector3 enlargedScale = new Vector3(1.3f, 1.3f, 1f);
    public Vector3 heartSpawnPosition = new Vector3(0, 4, 0);

    private Animator animator;
    private Rigidbody2D rb;
    private bool collected = false;
    private AudioSource audioSource;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;

        if (other.CompareTag("Player"))
        {
            collected = true;
            GetComponent<Collider2D>().enabled = false;

            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ActivateSpecialLife(effectDuration, enlargedScale);
                player.StartCoroutine(player.NoCollider());

                GameObject existingHeart = GameObject.FindWithTag("SpecialHeart");

                if (specialHeartPrefab != null)
                {
                    if (existingHeart == null)
                    {
                        StartCoroutine(SpawnHeartWithEffect());
                    }
                    else
                    {
                        HeartStatus heartStatus = existingHeart.GetComponent<HeartStatus>();
                        if (heartStatus != null && heartStatus.IsDisappearing)
                        {
                            Destroy(existingHeart);
                            StartCoroutine(SpawnHeartWithEffect());
                        }
                    }
                }

                if (rb != null)
                {
                    rb.velocity = Vector2.zero;
                    rb.isKinematic = true;
                }

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
        }
    }

    private IEnumerator SpawnHeartWithEffect()
    {
        if (heartSpawnEffectPrefab != null)
        {
            GameObject effect = Instantiate(heartSpawnEffectPrefab, heartSpawnPosition, Quaternion.identity);
            Destroy(effect, 1f);
        }

        yield return new WaitForSeconds(0.9f);

        Instantiate(specialHeartPrefab, heartSpawnPosition, Quaternion.identity);
    }

    private IEnumerator DestroyAfterFade()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    private IEnumerator StopSoundAfter(float time)
    {
        yield return new WaitForSeconds(time);
        audioSource.Stop();
    }

    
}

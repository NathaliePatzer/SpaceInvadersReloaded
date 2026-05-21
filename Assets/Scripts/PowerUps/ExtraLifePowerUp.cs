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
                        // 1. Passamos o "player" como parâmetro aqui!
                        StartCoroutine(SpawnHeartWithEffect(player)); 
                    }
                    else
                    {
                        HeartStatus heartStatus = existingHeart.GetComponent<HeartStatus>();
                        if (heartStatus != null && heartStatus.IsDisappearing)
                        {
                            Destroy(existingHeart);
                            // Passamos o "player" como parâmetro aqui também!
                            StartCoroutine(SpawnHeartWithEffect(player));
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

    // 2. Mudamos a assinatura para exigir o Player
    private IEnumerator SpawnHeartWithEffect(PlayerController player)
    {
        if (heartSpawnEffectPrefab != null)
        {
            GameObject effect = Instantiate(heartSpawnEffectPrefab, heartSpawnPosition, Quaternion.identity);
            Destroy(effect, 1f);
        }

        // Espera o tempo dramático...
        yield return new WaitForSeconds(0.9f);

        // --- A BARREIRA DE QA ---
        // Verifica se o player ainda existe na memória E se ele AINDA tem o buff!
        if (player != null && player.PossuiVidaEspecial())
        {
            Instantiate(specialHeartPrefab, heartSpawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.Log("Cogumelo abortou o spawn do coração porque o jogador tomou dano muito rápido!");
        }
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
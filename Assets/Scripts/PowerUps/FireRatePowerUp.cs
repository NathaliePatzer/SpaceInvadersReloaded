using System.Collections;
using UnityEngine;

public class FireRatePowerUp : MonoBehaviour
{
    public float cooldownReduction = 0.8f;
    public float duration = 5f;

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
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ReduceWeaponCooldown(cooldownReduction, duration);
            }

            collected = true;

            // Para o movimento
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.isKinematic = true; // desativa física
            }

            // Desativa colisão
            GetComponent<Collider2D>().enabled = false;

            if (audioSource != null)
            {
                audioSource.Play();
                StartCoroutine(StopSoundAfter(2f)); 
            }

            // Toca a animação
            if (animator != null)
            {
                animator.SetTrigger("Colided");
            }

            // Espera a animação terminar antes de destruir
            StartCoroutine(DestroyAfterFade());
        }
    }

    private IEnumerator DestroyAfterFade()
    {
        yield return new WaitForSeconds(0.8f); // ajuste conforme a duração da animação
        Destroy(gameObject);
    }


    private IEnumerator StopSoundAfter(float time)
    {
        yield return new WaitForSeconds(time);
        audioSource.Stop();
    }

}

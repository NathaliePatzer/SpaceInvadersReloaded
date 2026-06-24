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

        // --- A TRAVA DE TRANSICAO DE FASE AQUI ---
        // Procuramos o Player na cena inteira. Se ele for nulo (desativado na transição)
        // OU se ele estiver com os buffs travados, barramos o Olho do Ender na hora!
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player == null || !player.podeReceberBuffs)
        {
            return;
        }

        Vector3 portalPosition = new Vector3(0f, -1.63f, 0f);
        GameObject existingPortal = GameObject.FindWithTag("Portal");

        bool shouldSpawnNewPortal = true;

        if (existingPortal != null)
        {
            PortalController controller = existingPortal.GetComponent<PortalController>();
            if (controller != null && !controller.IsDisappearing())
            {
                controller.StartPortal(portalDuration);
                shouldSpawnNewPortal = false;
            }
        }

        if (shouldSpawnNewPortal)
        {
            StartCoroutine(SpawnPortalSequence(portalPosition));
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

        // --- DOUBLE CHECK DE SEGURANÇA DE TRANSICAO DE FASE---
        // Se após os 0.8 segundos do efeito o player sumiu ou travou, cancela o nascimento do portal!
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player == null || !player.podeReceberBuffs)
        {
            yield break; // Cancela a execução da Coroutine imediatamente
        }

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

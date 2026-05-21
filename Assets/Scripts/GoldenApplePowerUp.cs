using System.Collections;
using UnityEngine;

public class GoldenApplePowerUp : MonoBehaviour
{

    [Header("Efeitos Visuais")]
    public GameObject textinhoFlutuantePrefab;
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
        // 1. Trava de segurança (QA): Evita dupla coleta
        if (collected) return;

        // 2. Otimização: Só tenta rodar a lógica se quem encostou foi o Player
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // 2. A BIFURCAÇÃO DA RECOMPENSA
                if (player.TentarRecuperarVida())
                {
                    // Deu true! O jogador curou a vida. Não damos pontos, apenas o juice de cura.
                    Debug.Log("Maçã curou a vida do jogador!");
                }
                else
                {
                    // Deu false! A vida já estava cheia. 
                    Debug.Log("Vida cheia! Convertendo maçã em 500 pontos!");

                    // Adiciona os pontos (Ajuste o nome do seu método AddScore se for diferente!)
                    if (ScoreSystem.Instance != null)
                    {
                        ScoreSystem.Instance.AddScore(500);
                    }

                    // Instancia o textinho flutuante exatamente na posição da maçã
                    if (textinhoFlutuantePrefab != null)
                    {
                        Instantiate(textinhoFlutuantePrefab, transform.position, Quaternion.identity);
                    }
                }
            }

            // 3. Marca como coletado para travar o script
            collected = true;

            // 4. Congela o movimento no ar
            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.isKinematic = true; // desativa física
            }

            // 5. Desativa a colisão para não tomar hit de tiros perdidos
            Collider2D coll = GetComponent<Collider2D>();
            if (coll != null) coll.enabled = false;

            // 6. Toca o som de coleta
            if (audioSource != null)
            {
                audioSource.Play();
            }

            // 7. Toca a animação de coleta (aquele brilho ou fade out)
            if (animator != null)
            {
                animator.SetTrigger("Colided");
            }

            // 8. Espera a animação terminar antes de remover da memória
            StartCoroutine(DestroyAfterFade());
        }
    }

    private IEnumerator DestroyAfterFade()
    {
        yield return new WaitForSeconds(1.2f); // ajuste conforme a duração da animação lá no Animator
        Destroy(gameObject);
    }

}
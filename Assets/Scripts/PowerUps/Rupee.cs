using UnityEngine;

public class Rupee : MonoBehaviour
{
    [Header("Pontuação e Efeitos")]
    public int valorPontuacao = 10;
    public float tempoAnimacao = 5f; // Ajuste para o tempo exato da sua animação!
    public AudioClip somColeta;        // O efeitinho sonoro

    private Animator anim;
    private AudioSource audioSource;
    private Collider2D col;
    private Rigidbody2D rb;

    // Flag de segurança super importante para o player não pontuar 2x na mesma moeda!
    private bool jaColetada = false;

    private void Start()
    {
        // "Decora" os componentes quando a rupia nasce
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Se ela já foi pega milissegundos atrás, ignora colisões extras
        if (jaColetada) return;

        if (other.CompareTag("Player"))
        {
            jaColetada = true; // Trava a rupia!

            // --- Congela a rupia no ar! ---
            if (rb != null)
            {
                rb.velocity = Vector2.zero; // Zera a velocidade da queda
                rb.simulated = false;       // Desliga a gravidade e o motor físico
            }

            // 1. Dá a pontuação
            if (ScoreSystem.Instance != null)
            {
                ScoreSystem.Instance.AddScore(valorPontuacao);
            }

            // 2. Toca o sonzinho Arcade
            if (audioSource != null && somColeta != null)
            {
                audioSource.PlayOneShot(somColeta);
            }

            // 3. Puxa o gatilho da sua animação (ex: um brilho sumindo)
            if (anim != null)
            {
                anim.SetTrigger("Collected");
            }

            // 4. Desliga o colisor para a física ignorar ela a partir de agora
            if (col != null)
            {
                col.enabled = false;
            }

            // 5. Destrói o objeto com ATRASO, dando tempo pro som e pra arte brilharem!
            Destroy(gameObject, tempoAnimacao);
        }
    }

    private void Update()
    {
        // A nossa velha e boa faxina de memória
        if (transform.position.y < -7f && !jaColetada)
        {
            Destroy(gameObject);
        }
    }
}
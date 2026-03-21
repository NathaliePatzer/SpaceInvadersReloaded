using UnityEngine;

public class DemobatController : MonoBehaviour
{
    [Header("Status do Morcego")]
    public float speed = 3f; // Velocidade de queda
    public float frequency = 5f; // Quão rápido ele faz o zigue-zague
    public float magnitude = 1.5f; // Quão largo é o zigue-zague

    private Vector3 startPosition;
    private float aliveTime;

    // --- AS NOVIDADES DA ANIMAÇÃO ---
    private Animator anim;
    private bool isDead = false; // Flag para saber se ele já tomou o tiro

    void Start()
    {
        // Salva o eixo X original de onde ele nasceu para usar como base da onda
        startPosition = transform.position;
        anim = GetComponent<Animator>(); // "Decora" o Animator
    }

    void Update()
    {
        // Se ele morreu, aborta o Update! Ele para de cair e de fazer zigue-zague.
        if (isDead) return;

        aliveTime += Time.deltaTime;

        // 1. A gravidade empurrando ele pra baixo
        float newY = transform.position.y - (speed * Time.deltaTime);
        // 2. A função Seno criando o zigue-zague no eixo X
        float newX = startPosition.x + Mathf.Sin(aliveTime * frequency) * magnitude;
        // Aplica a nova posição calculada
        transform.position = new Vector3(newX, newY, 0f);
        // Sistema de faxina: Se o morcego sair da tela lá embaixo, destrói para liberar memória!
        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return; // Se já tá morto, ignora outras colisões

        if (other.CompareTag("Player"))
        {
            PlayerController nave = other.GetComponent<PlayerController>();
            if (nave != null)
            {
                nave.TakeDamage(); // Usa o seu sistema perfeitinho de dano!
            }
            Die(); // O morcego morre no impacto
        }
        else if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject); // Destrói o laser da nave
            Die(); // Chama a rotina de morte do morcego
        }
    }

    // Rotina isolada para organizar a morte
    void Die()
    {
        isDead = true; // Avisa o Update para parar o movimento

        // Desliga a física dele para ele virar um "fantasma" na tela
        GetComponent<Collider2D>().enabled = false;

        // Puxa o gatilho da animação que você criou ali no Animator!
        anim.SetTrigger("shoot");

        // Destrói o morcego com um ATRASO. 
        // 0.5f significa meio segundo. Ajuste esse número para o tempo exato da sua animação!
        Destroy(gameObject, 0.5f);
    }
}
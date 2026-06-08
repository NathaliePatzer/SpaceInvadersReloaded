using UnityEngine;

public class DemobatController : MonoBehaviour
{
    [Header("Status do Morcego")]
    public float speed = 3f; // Velocidade de queda
    public float frequency = 3f; // Quão rápido ele faz o zigue-zague
    public float magnitude = 3.5f; // Quão largo é o zigue-zague
    // Valor da recompensa 
    [Header("Pontuação")]
    public int scoreValue = 25;
    public GameObject floatingTextPrefab;

    [Header("Drops")]
    public GameObject triforcePrefab; // Onde você vai arrastar o prefab na Unity
    [Range(0, 100)]
    public float chanceDropTriforce = 10f; // 10% de chance de dropar ao morrer

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
            // Deixa a bala se reciclar sozinha, sem destruir! 
            // (Removemos o Destroy(other.gameObject))

            // Dá os pontos para o jogador! 
            if (ScoreSystem.Instance != null)
            {
                ScoreSystem.Instance.AddScore(scoreValue);
            }

            // --- NOVO: Instancia o texto flutuante na posição do morcego! ---
            if (floatingTextPrefab != null)
            {
                Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
            }

            Die(); // Chama a rotina de morte do morcego
        }
    }

    // Rotina isolada para organizar a morte
    void Die()
    {
        isDead = true; // Avisa o Update para parar o movimento

        // Sorteia um número de 0 a 100
        float sorteio = Random.Range(0f, 100f);

        // Se o número sorteado for menor que a chance estipulada, solta o item!
        if (sorteio <= chanceDropTriforce)
        {
            // Verifica se você lembrou de colocar o prefab no Inspector para não dar erro
            if (triforcePrefab != null)
            {
                Instantiate(triforcePrefab, transform.position, Quaternion.identity);
            }
        }

        // Desliga a física dele para ele virar um "fantasma" na tela
        GetComponent<Collider2D>().enabled = false;

        // Puxa o gatilho da animação que você criou ali no Animator!
        anim.SetTrigger("shoot");

        // Destrói o morcego com um ATRASO. 
        // 0.5f significa meio segundo. Ajuste esse número para o tempo exato da sua animação!
        Destroy(gameObject, 0.5f);
    }
}
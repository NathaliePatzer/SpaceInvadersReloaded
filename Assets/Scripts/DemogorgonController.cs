using System.Collections;
using UnityEngine;

public class DemogorgonController : MonoBehaviour
{
    [Header("Status do Boss")]
    public int maxHealth = 50;
    private int currentHealth;

    [Header("Ataques e Summons")]
    public GameObject demobatPrefab; // Preparado para os morcegos
    public GameObject wafflePowerUp; // Preparado para o drop de cura

    [Header("Movimentação (Mundo Invertido)")]
    public float teleportInterval = 6f; // Tempo que ele fica parado antes de sumir
    public float minX = -8f; // Ajuste conforme o limite esquerdo da sua câmera
    public float maxX = 8f;  // Ajuste conforme o limite direito da sua câmera
    public float fixedY = 2f; // A altura que ele vai ficar flutuando no topo

    // Otimização: guardar os componentes para não procurar toda hora
    private SpriteRenderer spriteRenderer;
    private Collider2D bossCollider;

    void Awake()
    {
        // "Decora" os componentes assim que o objeto nasce
        spriteRenderer = GetComponent<SpriteRenderer>();
        bossCollider = GetComponent<Collider2D>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        Debug.Log("O Demogorgon rasgou a realidade e entrou na tela!");

        // Inicia a rotina do Mundo Invertido
        StartCoroutine(TeleportRoutine());
    }

    IEnumerator TeleportRoutine()
    {
        while (currentHealth > 0)
        {
            // Fica parado encarando o jogador pela metade do tempo...
            yield return new WaitForSeconds(teleportInterval / 2f);

            // ATACA! (Cospe a nuvem de morcegos)
            ShootDemobats();

            // Espera a outra metade do tempo pra o jogador ver o ataque sair...
            yield return new WaitForSeconds(teleportInterval / 2f);

            // 3. Calcula a nova posição aleatória
            float randomX = Random.Range(minX, maxX);
            transform.position = new Vector3(randomX, fixedY, 0f);

            // 4. Rasga a realidade e volta pra tela!
            if (spriteRenderer != null) spriteRenderer.enabled = true;
            if (bossCollider != null) bossCollider.enabled = true;
        }
    }

    // A bala da nave precisa chamar essa função quando bater nele (OnTriggerEnter2D)
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Vida do Boss: " + currentHealth);

        // Se a vida zerar, ele morre
        if (currentHealth <= 0)
        {
            Defeated();
        }
    }

    void Defeated()
    {
        Debug.Log("Demogorgon Derrotado! A cidade está salva!");

        // Avisa o WaveManager que o Chefão caiu e o jogo foi vencido
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.OnWaveCompleted();
        }

        // Destrói o corpo do Boss
        Destroy(gameObject);
    }

    void ShootDemobats()
    {
        if (demobatPrefab != null)
        {
            // Instancia 3 morcegos de uma vez: um no centro, um na esquerda e um na direita!
            Instantiate(demobatPrefab, transform.position, Quaternion.identity);
            Instantiate(demobatPrefab, transform.position + new Vector3(-1f, -0.5f, 0f), Quaternion.identity);
            Instantiate(demobatPrefab, transform.position + new Vector3(1f, -0.5f, 0f), Quaternion.identity);
        }
    }
}
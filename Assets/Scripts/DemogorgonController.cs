using System.Collections;
using UnityEngine;

public class DemogorgonController : MonoBehaviour
{
    [Header("Status do Boss")]
    public int maxHealth = 112;
    private int currentHealth;

    [Header("Ataques e Summons")]
    public GameObject demobatPrefab; // Preparado para os morcegos
    public GameObject wafflePowerUp; // Preparado para o drop de cura

    [Header("Movimentação (Mundo Invertido)")]
    public float teleportInterval = 8f; // Tempo que ele fica parado antes de sumir
    public float minX = -8f; // Ajuste conforme o limite esquerdo da sua câmera
    public float maxX = 8f;  // Ajuste conforme o limite direito da sua câmera
    public float fixedY = 2f; // A altura que ele vai ficar flutuando no topo

    [Header("Feedback Visual (Hit Flash)")] // --- NOVA SEÇÃO ---
    public Color hitColor = Color.white; // A cor que ele vai piscar (Branco puro ou Vermelho são bons)
    public float flashDuration = 0.1f;    // Quão curtinho é o piscar (0.1 ou 0.05 são ótimos tempos)
    private Color originalColor;          // Para lembrar a cor original do sprite
    private Coroutine hitFlashCoroutine;  // Referência para controlar o piscar

    [Header("Efeitos do Teleporte")]
    public GameObject portalPrefab; // Arraste o Prefab do portal para cá na Unity
    public float portalAnimationTime = 1f; // Tempo exato da animação do portal

    [Header("Aviso de Teleporte")]
    public int teleportBlinkCount = 3;       // Quantas vezes ele vai piscar
    public float teleportBlinkDuration = 0.1f; // O tempo que ele fica "apagado" e "aceso" na piscada

    [Header("Barra de Vida (UI Visual)")]
    public SpriteRenderer lifebarRenderer; // O componente que vai mostrar a barra na tela
    public Sprite[] lifebarSprites; // A lista com os seus 15 sprites

    // Otimização: guardar os componentes para não procurar toda hora
    private SpriteRenderer spriteRenderer;
    private Collider2D bossCollider;

    void Awake()
    {
        // "Decora" os componentes assim que o objeto nasce
        spriteRenderer = GetComponent<SpriteRenderer>();
        bossCollider = GetComponent<Collider2D>();

        // Salva a cor original (geralmente branco) para ele poder voltar ao normal depois de piscar
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    void Start()
    {
        currentHealth = maxHealth;

        UpdateLifebar();

        Debug.Log("O Demogorgon rasgou a realidade e entrou na tela!");

        // 1. Esconde o boss logo no primeiro frame para ele não piscar na posição errada do Prefab
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (bossCollider != null) bossCollider.enabled = false;

        // 2. Inicia a rotina de entrada com o portal em vez de ir direto pro ataque
        StartCoroutine(IntroRoutine());
    }

    IEnumerator IntroRoutine()
    {
        // 1. Calcula a posição inicial correta
        float randomX = Random.Range(minX, maxX);
        Vector3 posicaoInicial = new Vector3(randomX, fixedY, 0f);
        transform.position = posicaoInicial;

        // 2. Cria o portal de entrada nessa nova posição
        GameObject portal = Instantiate(portalPrefab, posicaoInicial, Quaternion.identity);

        // 3. Espera o portal abrir até a metade
        yield return new WaitForSeconds(portalAnimationTime / 2f);

        // --- NOVO: PISCAR "GLITCH" ANTES DE APARECER DEFINITIVAMENTE ---
        for (int i = 0; i < teleportBlinkCount; i++)
        {
            // Acende
            if (spriteRenderer != null) spriteRenderer.enabled = true;
            yield return new WaitForSeconds(teleportBlinkDuration);

            // Apaga
            if (spriteRenderer != null) spriteRenderer.enabled = false;
            yield return new WaitForSeconds(teleportBlinkDuration);
        }

        // 4. O boss aparece definitivamente (e liga o colisor)!
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (bossCollider != null) bossCollider.enabled = true;

        // 5. Espera a outra metade da animação do portal terminar
        yield return new WaitForSeconds(portalAnimationTime / 2f);
        Destroy(portal);

        // 6. Começa o ciclo normal de atacar e teleportar
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

            // --- NOVO: O AVISO DE TELEPORTE (PISCAR) ---
            for (int i = 0; i < teleportBlinkCount; i++)
            {
                // Apaga
                if (spriteRenderer != null) spriteRenderer.enabled = false;
                yield return new WaitForSeconds(teleportBlinkDuration);

                // Acende
                if (spriteRenderer != null) spriteRenderer.enabled = true;
                yield return new WaitForSeconds(teleportBlinkDuration);
            }

            // --- CONTINUA A LÓGICA DO PORTAL ---

            // 1. Agora sim, desliga o boss "de verdade" para ele sumir da posição antiga
            if (spriteRenderer != null) spriteRenderer.enabled = false;
            if (bossCollider != null) bossCollider.enabled = false;

            // 2. Calcula a nova posição para onde ele vai
            float randomX = Random.Range(minX, maxX);
            Vector3 novaPosicao = new Vector3(randomX, fixedY, 0f);
            transform.position = novaPosicao;

            // 3. Abre o portal na nova posição!
            GameObject portal = Instantiate(portalPrefab, novaPosicao, Quaternion.identity);

            // 4. Espera METADE da animação do portal tocar
            yield return new WaitForSeconds(portalAnimationTime / 2f);

            // --- PISCAR ANTES DE REAPARECER ---
            for (int i = 0; i < teleportBlinkCount; i++)
            {
                // Acende
                if (spriteRenderer != null) spriteRenderer.enabled = true;
                yield return new WaitForSeconds(teleportBlinkDuration);

                // Apaga
                if (spriteRenderer != null) spriteRenderer.enabled = false;
                yield return new WaitForSeconds(teleportBlinkDuration);
            }

            // 5. Liga o boss de novo (ele está saindo bem no meio do portal!)
            if (spriteRenderer != null) spriteRenderer.enabled = true;
            if (bossCollider != null) bossCollider.enabled = true;

            // 6. Espera a outra metade da animação do portal terminar de rodar
            yield return new WaitForSeconds(portalAnimationTime / 2f);

            // 7. Destrói o portal para ele sumir da tela
            Destroy(portal);
        }
    }

    // A bala da nave precisa chamar essa função quando bater nele (OnTriggerEnter2D)
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Vida do Boss: " + currentHealth);

        // --- Toca o efeito visual de dano! ---
        StartHitFlash();

        // ---  Atualiza o sprite da barra de vida após tomar o tiro! ---
        UpdateLifebar();

        // Se a vida zerar, ele morre
        if (currentHealth <= 0)
        {
            Defeated();
        }
    }

    // Função auxiliar para iniciar a coroutine com segurança
    void StartHitFlash()
    {
        if (spriteRenderer == null) return;

        // Se o boss tomar múltiplos tiros rápidos, a gente para o piscar anterior 
        // e começa um novo do zero (para o feedback ser instantâneo)
        if (hitFlashCoroutine != null)
        {
            StopCoroutine(hitFlashCoroutine);
        }

        hitFlashCoroutine = StartCoroutine(HitFlashRoutine());
    }

    // A coroutine que realmente faz o piscar
    IEnumerator HitFlashRoutine()
    {
        // 1. Muda para a cor de dano (ex: Branco puro)
        spriteRenderer.color = hitColor;

        // 2. Espera o tempo curtinho
        yield return new WaitForSeconds(flashDuration);

        // 3. Volta para a cor normal
        spriteRenderer.color = originalColor;

        // Limpa a referência
        hitFlashCoroutine = null;
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

    // A física das colisões! O Boss sente o tiro batendo nele.
    void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se quem bateu nele foi o laser da sua nave
        if (other.CompareTag("Bullet"))
        {
            // 1. Destrói o tiro da nave para não atravessar o Boss
            Destroy(other.gameObject);

            // 2. Chama a nossa função maravilhosa que tira vida e faz piscar!
            // (Coloquei 1 de dano, mas se o seu tiro for mais forte, pode mudar esse número)
            TakeDamage(1);
        }
    }

    void UpdateLifebar()
    {
        // Se você esquecer de colocar o renderer ou os sprites na Unity, ele não faz nada (evita erros)
        if (lifebarRenderer == null || lifebarSprites.Length == 0) return;

        // Evita que a vida fique negativa e quebre a matemática
        int vidaSegura = Mathf.Max(0, currentHealth);

        // A mágica: divide a vida por 8 e arredonda para cima. 
        // Ex: 112/8 = 14. Vida 105/8 = 13.1 (arredonda pra 14). Vida 0 = 0.
        int indiceSprite = Mathf.CeilToInt(vidaSegura / 8f);

        // Por segurança, garante que o índice não tente buscar um sprite que não existe na lista
        indiceSprite = Mathf.Clamp(indiceSprite, 0, lifebarSprites.Length - 1);

        // Troca a imagem da barra!
        lifebarRenderer.sprite = lifebarSprites[indiceSprite];
    }
}
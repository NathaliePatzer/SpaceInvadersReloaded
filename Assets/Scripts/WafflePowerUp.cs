using UnityEngine;

public class WafflePowerUp : MonoBehaviour
{
    [Header("Waffle Settings")]
    // Velocidade com que o Waffle cai pela tela
    public float fallSpeed = 5f;
    // Posição Y (limite inferior da câmera) onde o Waffle é destruído caso o jogador não o pegue
    public float lowerBoundary = -10f;

    [Header("Feedback Settings")]
    // Tempo de espera (em segundos) para a animação de coleta tocar inteira antes de apagar o objeto
    public float destroyDelay = 0.5f;

    [Header("Buff Power")]
    // Quantidade de tempo que será subtraída do cooldown original do tiro da nave
    public float cooldownReductionAmount = 0.3f;
    // Quantos segundos o efeito do tiro rápido vai durar antes de voltar ao normal
    public float buffDuration = 5f;
    // Referência para o sistema de partículas de poeira
    private ParticleSystem dustParticles;
    // Referência para o componente de rastro
    private TrailRenderer trail;

    void Update()
    {
        // Faz o Waffle se mover para baixo constantemente, multiplicando pela velocidade e pelo tempo do frame
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

        // Verifica se o Waffle passou do limite inferior da tela
        if (transform.position.y < lowerBoundary)
        {
            // Destrói o objeto para não pesar a memória do jogo com coisas fora da tela
            Destroy(gameObject);
        }
    }

    // Função chamada automaticamente pela Unity quando algo entra na área de colisão (Trigger) do Waffle
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto que encostou tem a tag "Player" (a sua nave)
        if (other.CompareTag("Player"))
        {
            // Tenta pegar o script 'PlayerController' que está anexado à nave que colidiu
            PlayerController playerShip = other.GetComponent<PlayerController>();

            // Se encontrou o script na nave (ou seja, é realmente a nave correta), executa o efeito
            if (playerShip != null)
            {
                // Chama a função da nave passando o quanto reduzir do tiro e por quanto tempo
                playerShip.ReduceWeaponCooldown(cooldownReductionAmount, buffDuration);
                
                // Aviso no console só para termos certeza de que a matemática funcionou durante os testes
                //Debug.Log("Waffle coletado! Cooldown da arma reduzido.");

                // --- LÓGICA DE FEEDBACK VISUAL (ANIMAÇÃO E DESTRUIÇÃO) ---

                // Tenta achar o componente Animator no próprio objeto do Waffle
                Animator animator = GetComponent<Animator>();
                
                // Se não achar no objeto principal, procura caso o Animator esteja em um objeto "filho"
                if (animator == null)
                {
                    animator = GetComponentInChildren<Animator>();
                }

                // Se achou o Animator com sucesso...
                if (animator != null)
                {
                    // Dispara o gatilho exato para trocar de estado e tocar a animação de sumir/fade
                    animator.SetTrigger("Colided");
                }

                // Zera a velocidade para o Waffle parar de cair exatamente no ponto onde foi pego
                fallSpeed = 0f;

                if (dustParticles != null)
                {
                    // Dizemos para o sistema de partículas parar de criar novas poeiras.
                    // As poeiras que já foram criadas continuam lá fadando suavemente.
                    dustParticles.Stop();
                }

                // ---  Para de gerar o rastro! ---
                if (trail != null)
                {
                    // Dizemos para o rastro parar de criar novos pontinhos, mas o rastro que já foi criado continua ali fadando suavemente.
                    trail.emitting = false;
                }

                // Desativa o colisor para garantir que a nave não acione esse mesmo Waffle duas vezes num único frame
                Collider2D col = GetComponent<Collider2D>();
                if (col != null) col.enabled = false;

                // Destrói o Waffle da cena, mas aguarda o tempo definido para a animação dar tempo de tocar
                Destroy(gameObject, destroyDelay);
            }
        }
    }
}
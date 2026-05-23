using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IShootable
{
    Coroutine cooldownRoutine;
    float originalCooldown;
    public Team team;
    public float speed = 10;
    public int lives = 3;
    float _speed;
    Vector2 movement;
    Rigidbody2D rb;
    Weapon weapon;
    Animator animator;
    Collider2D coll;
    AudioSource audioSource;
    private SpriteRenderer spriteRenderer;

    private bool hasSpecialLife = false;
    private Coroutine specialLifeRoutine;
    private Vector3 originalScale;
    private bool isInvincibleFlag = false;

    [Header("Efeitos Visuais")]
    public ParticleSystem particulasFogoRapido; // Arraste o Particle System recém-criado para cá no Inspector

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        weapon = GetComponentInChildren<Weapon>();
        animator = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        _speed = speed;
        originalCooldown = weapon.cooldown;
        originalScale = transform.localScale;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime * speed);
    }

    public void Movement(InputAction.CallbackContext context)
    {
        if (context.performed)
            movement = context.ReadValue<Vector2>();
        else if (context.canceled)
            movement = Vector2.zero;
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.performed)
            weapon.ShootBullet();
    }

    public void OnShot(Bullet bullet)
    {
        if (bullet != null)
            bullet.speed = 0;

        TakeDamage(); // Chama a nossa nova função central de dano!
    }

    public bool PossuiVidaEspecial()
    {
        return hasSpecialLife;
    }

    // NOVA FUNÇÃO: Faz exatamente tudo o que você já tinha programado!
    public void TakeDamage()
    {
        // 1. O CADEADO: Se já estiver invencível, ignora o dano do segundo morcego!
        if (isInvincibleFlag) return;

        // 2. Tranca o cadeado na hora que toma o primeiro dano
        isInvincibleFlag = true;

        if (hasSpecialLife)
        {
            hasSpecialLife = false;
            audioSource.Play();
            StartCoroutine(Invencible());
            StartCoroutine(BlinkBeforeResetScale());

            if (specialLifeRoutine != null)
            {
                StopCoroutine(specialLifeRoutine);
                specialLifeRoutine = null;
            }

            GameObject heart = GameObject.FindWithTag("SpecialHeart");
            if (heart != null)
            {
                Animator heartAnimator = heart.GetComponent<Animator>();
                if (heartAnimator != null)
                {
                    HeartStatus heartStatus = heart.GetComponent<HeartStatus>();
                    if (heartStatus != null)
                    {
                        heartStatus.TriggerDisappear();
                        StartCoroutine(DestroyAfterAnimation(heart.GetComponent<Animator>(), 1f));
                    }
                }
            }
            return;
        }

        lives--;
        LivesHearts.Instance.UpdateHearts(lives);
        animator.SetTrigger("Death");
        animator.SetInteger("Lives", lives);
        audioSource.Play();
        StartCoroutine(Invencible());

        if (lives <= 0)
            GameOver.Instance.OnGameOver(2000);
    }

    public bool TentarRecuperarVida()
    {
        // Só recupera se o jogador tiver menos de 3 vidas
        if (lives < 3)
        {
            lives++;

            // 1. Atualiza os corações na tela
            if (LivesHearts.Instance != null)
            {
                LivesHearts.Instance.UpdateHearts(lives);
            }

            Debug.Log("Vida recuperada! Vidas atuais: " + lives);
            return true; //Avisa a maçã "Eu precisei da cura"

        }
        else
        {
            // Opcional: só para você ver no console que o limite funcionou, 
            // mesmo a maçã sumindo da tela
            Debug.Log("Vida já estava cheia. Maçã consumida sem efeito.");
            return false; // Avisa a maçã: "Tô cheio de vida, me dá os pontos!"
        }
    }

    private IEnumerator DestroyAfterAnimation(Animator animator, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (animator != null && animator.gameObject != null)
            Destroy(animator.gameObject);
    }

    public Team GetTeam() => team;

    IEnumerator Invencible()
    {
        coll.enabled = false;
        speed = 0;
        yield return new WaitForSeconds(1.5f);
        coll.enabled = true;
        speed = _speed;

        // 3. ABRE O CADEADO: O jogador pode tomar dano de novo!
        isInvincibleFlag = false;
    }
    public IEnumerator NoCollider()
    {
        //Debug.Log("No collider ON");
        coll.enabled = false;
        speed = 0;
        yield return new WaitForSeconds(1f);
        coll.enabled = true;
        speed = _speed;
        //Debug.Log("No collider OFF");
    }

    public void ReduceWeaponCooldown(float amount, float duration)
    {
        if (cooldownRoutine != null)
        {
            StopCoroutine(cooldownRoutine);
            weapon.cooldown = originalCooldown;

            // Corta as partículas se o jogador pegar outro power-up em cima da hora
            if (particulasFogoRapido != null) particulasFogoRapido.Stop();
        }

        originalCooldown = weapon.cooldown;
        cooldownRoutine = StartCoroutine(ApplyTemporaryCooldown(amount, duration));
    }

    IEnumerator ApplyTemporaryCooldown(float amount, float duration)
    {
        weapon.cooldown = Mathf.Max(0.1f, weapon.cooldown - amount);

        // Acende os motores! (Liga as partículas)
        if (particulasFogoRapido != null) particulasFogoRapido.Play();

        yield return new WaitForSeconds(duration);

        weapon.cooldown = originalCooldown;

        // Desliga a emissão quando o tempo acabar
        if (particulasFogoRapido != null) particulasFogoRapido.Stop();

        cooldownRoutine = null;
    }

    public void ActivateSpecialLife(float duration, Vector3 newScale)
    {
        if (hasSpecialLife)
        {
            if (specialLifeRoutine != null)
                StopCoroutine(specialLifeRoutine);
        }
        else
        {
            hasSpecialLife = true;
            StartCoroutine(BlinkBeforeScale(newScale));
        }

        specialLifeRoutine = StartCoroutine(SpecialLifeTimer(duration));
    }

    private IEnumerator BlinkBeforeScale(Vector3 newScale)
    {
        int blinkCount = 4;
        float blinkInterval = 0.1f;

        for (int i = 0; i < blinkCount; i++)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(blinkInterval);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(blinkInterval);
        }

        transform.localScale = newScale;
    }

    private IEnumerator BlinkBeforeResetScale()
    {
        int blinkCount = 4;
        float blinkInterval = 0.1f;

        for (int i = 0; i < blinkCount; i++)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(blinkInterval);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(blinkInterval);
        }

        transform.localScale = originalScale;
    }


    private IEnumerator SpecialLifeTimer(float duration)
    {
        yield return new WaitForSeconds(duration);
        hasSpecialLife = false;

        // Faz o coração desaparecer
        GameObject heart = GameObject.FindWithTag("SpecialHeart");
        if (heart != null)
        {
            Animator heartAnimator = heart.GetComponent<Animator>();
            if (heartAnimator != null)
            {

                HeartStatus heartStatus = heart.GetComponent<HeartStatus>();
                if (heartStatus != null)
                {
                    heartStatus.TriggerDisappear();
                    StartCoroutine(DestroyAfterAnimation(heart.GetComponent<Animator>(), 1f));
                }

            }
        }

        StartCoroutine(BlinkBeforeResetScale());
    }

    // Essa é a função que a Triforce chama!
    public void AtivarTiroTriplo()
    {
        // Se a nave tem uma arma equipada, manda ela ligar o poder
        if (weapon != null)
        {
            weapon.LigarTiroTriplo(8f); // Passa o tempo (5 segundos) como aviso
        }

        // Opcional: Dica visual mudando a cor da nave pra amarelinho
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(1f, 0.3f, 0.3f);
        }
    }

    // Como o Player mudou de cor, ele mesmo se encarrega de voltar ao normal depois
    public void DesativarEfeitoVisualTiroTriplo()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
    }

    // --- O BOTÃO DE PÂNICO: PREPARAÇÃO PARA O BOSS ---
    public void LimparTodosOsBuffs()
    {
        // 1. FAXINA DA ARMA (Tiro Triplo)
        if (weapon != null)
        {
            weapon.ResetarTodosOsPowerUps();
        }

        // 1.1 Limpa o Fire Rate (Waffle e StarFruit)
        if (cooldownRoutine != null)
        {
            StopCoroutine(cooldownRoutine);
            if (weapon != null) weapon.cooldown = originalCooldown;
            cooldownRoutine = null;
        }

        // <-- A FAXINA DAS PARTÍCULAS -->
        if (particulasFogoRapido != null)
        {
            particulasFogoRapido.Stop(); // Para de emitir novas faíscas

            // DICA: Se você quiser que as faíscas que já estão na tela sumam instantaneamente 
            // (já que é uma transição de tela para o Boss), você pode usar o comando Clear() também:
            particulasFogoRapido.Clear();
        }

        // 1.2 Limpa o visual do Tiro Triplo
        DesativarEfeitoVisualTiroTriplo();

        // 2. FAXINA DA VIDA ESPECIAL
        if (hasSpecialLife)
        {
            hasSpecialLife = false;

            if (specialLifeRoutine != null)
            {
                StopCoroutine(specialLifeRoutine);
                specialLifeRoutine = null;
            }

            GameObject heart = GameObject.FindWithTag("SpecialHeart");
            if (heart != null)
            {
                Animator heartAnimator = heart.GetComponent<Animator>();
                if (heartAnimator != null)
                {
                    HeartStatus heartStatus = heart.GetComponent<HeartStatus>();
                    if (heartStatus != null)
                    {
                        heartStatus.TriggerDisappear();
                        StartCoroutine(DestroyAfterAnimation(heartAnimator, 1f));
                    }
                }
            }
            transform.localScale = originalScale;
        }

        // 3. FAXINA DO PORTAL DO END
        GameObject portal = GameObject.FindWithTag("Portal");
        if (portal != null)
        {
            PortalController portalController = portal.GetComponent<PortalController>();
            // Se o portal existe e ainda não está sumindo...
            if (portalController != null && !portalController.IsDisappearing())
            {
                // Zeramos o tempo dele! Isso faz ele tocar a animação de saída sozinho.
                portalController.StartPortal(0f);
            }
        }

    }

}

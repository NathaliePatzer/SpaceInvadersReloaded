using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    //public float fireCooldown = 0.5f;
    public Bullet bullet;
    public Vector2 bulletDirection;
    public float bulletSpeed;
    public float cooldown;
    float lastShot;
    IShootable shootable;
    AudioSource audioSource;

    [Header("Power-Ups")]
    public bool temTiroTriplo = false;
    private Coroutine rotinaTiroTriplo;

    private void Awake()
    {
        shootable = GetComponentInParent<IShootable>();
        audioSource = GetComponent<AudioSource>();
    }
    public void ShootBullet()
    {
        if (Time.time - lastShot < cooldown)
            return;

        lastShot = Time.time;

        if (temTiroTriplo)
        {
            float angulo = 15f; // Aquele ângulo perfeito que vimos na simulação

            // Dispara as 3 balas usando o seu sistema de Pooling!
            SpawnPooledBullet(Quaternion.identity); // Reta
            SpawnPooledBullet(Quaternion.Euler(0, 0, angulo)); // Esquerda
            SpawnPooledBullet(Quaternion.Euler(0, 0, -angulo)); // Direita
        }
        else
        {
            // Disparo Simples Original
            SpawnPooledBullet(Quaternion.identity);
        }

        if (audioSource != null)
            audioSource.Play();
    }

    // --- NOVA FUNÇÃO: Organiza o seu Pooling para a gente não repetir código 3 vezes! ---
    private void SpawnPooledBullet(Quaternion rotacao)
    {
        // Faxina de QA: Verificação de segurança para evitar NullReference no jogo
        if (PoolingSystem.Instance == null || bullet == null) return;

        // 1. Pegamos a bala do pool igualzinho a antes
        Bullet instantiated = PoolingSystem.Instance.GetBullet(bullet);

        // Faxina de QA 2: Garante que pegamos um objeto válido
        if (instantiated == null) return;

        // 2. Posicionamos a bala no local de tiro
        instantiated.transform.position = transform.position;

        // --- AJUSTE CRÍTICO DE QA AQUI ---
        // Pegamos o Rigidbody2D da bala e zeramos qualquer velocidade/rotação residual
        Rigidbody2D rbBala = instantiated.GetComponent<Rigidbody2D>();
        if (rbBala != null)
        {
            rbBala.velocity = Vector2.zero; // Zera a velocidade antiga
            rbBala.angularVelocity = 0f; // Zera a rotação antiga
            rbBala.simulated = true; // Garante que a física está ligada
        }

        // --- MATEMÁTICA ESTÁVEL (O PULO DO GATO!) ---
        // Em vez de lermos otransform.up visual da Unity (que é instável), nós resolvemos a matemática!
        // Pegamos a direção padrão (que no Inspetor é (0,1)) e multiplicamos pelo ângulo (rotacao)!
        Vector3 defaultDirection = bulletDirection; // Transforma Vector2 para Vector3 implicitamente
        Vector3 finalVector = rotacao * defaultDirection; // Aplica o ângulo Z no vetor de direção (Y)

        // Agora avisamos a bala para onde ir, matematicamente resolvido e estável!
        instantiated.direction = finalVector; // Transforma Vector3 de volta para Vector2 implicitamente

        // Também rotacionamos visualmente o sprite pra ficar bonitinho e diagonal
        instantiated.transform.rotation = rotacao;
        // -----------------------------------------------------------------------------

        // 3. Configura o resto dos seus status originais (speed e team)
        instantiated.speed = bulletSpeed;
        instantiated.team = shootable.GetTeam();

        // 4. Liga a bala - Agora a física sabe exatamente para onde ir e tem os dados limpos!
        instantiated.gameObject.SetActive(true);
    }


    public void ReduceCooldown(float amount)
    {
        cooldown = Mathf.Max(0.1f, cooldown - amount);
    }

    // --- FUNÇÕES DE CONTROLE DO POWER UP ---
    public void LigarTiroTriplo(float duracao)
    {
        // 1. Verifica se já existe um cronômetro rodando (ou seja, se o poder já está ativo)
        if (rotinaTiroTriplo != null)
        {
            // 2. Se existir, ele "mata" o cronômetro antigo antes que o tempo acabe
            StopCoroutine(rotinaTiroTriplo);
        }
        // 3. E então, cria um cronômetro novinho em folha com os segundos completos!
        rotinaTiroTriplo = StartCoroutine(RotinaTiroTriploTimer(duracao));
    }

    private IEnumerator RotinaTiroTriploTimer(float duracao)
    {
        temTiroTriplo = true;

        yield return new WaitForSeconds(duracao);

        temTiroTriplo = false;

        // Avisa o Player para tirar o efeito visual amarelinho (opcional)
        PlayerController player = GetComponentInParent<PlayerController>();
        if (player != null) player.DesativarEfeitoVisualTiroTriplo();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    Player,
    Aliens,
}

public class Bullet : MonoBehaviour
{
    [HideInInspector]
    public Team team;

    public Vector2 direction;
    public float speed;

    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D coll;

    private bool isBeingAttracted = false;
    private Vector2 targetPortalPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
    }

    void OnEnable()
    {
        coll.enabled = true;
        isBeingAttracted = false; // Reset ao ativar
    }

    private void FixedUpdate()
    {
        if (isBeingAttracted)
        {
            Vector2 directionToPortal = (targetPortalPosition - rb.position).normalized;
            rb.MovePosition(rb.position + directionToPortal * speed * Time.fixedDeltaTime);
        }
        else
        {
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
        }
    }

    public void SetAttractionTarget(Vector2 portalPosition)
    {
        isBeingAttracted = true;
        targetPortalPosition = portalPosition;
    }

    public void ClearAttractionTarget()
    {
        isBeingAttracted = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Ignora áreas de invasão, itens e outras balas
        if (collision.GetComponent<InvadedTrigger>()) return;
        if (collision.CompareTag("PowerUp")) return;
        if (collision.GetComponent<Bullet>()) return;

        // 2. Lida com personagens que levam dano (Player e Aliens)
        IShootable shootable = collision.GetComponent<IShootable>();

        if (shootable != null)
        {
            // Se for do time inimigo, acerta!
            if (shootable.GetTeam() != team)
            {
                shootable.OnShot(this);
            }
            // Se for fogo amigo, ignora e a bala segue voando.
            else
            {
                return;
            }
        }
        else
        {
            // --- A INSERÇÃO DE BLINDAGEM DE QA ---
            // Se o objeto não é um IShootable (ou seja, não é o Player nem os Aliens),
            // a bala só pode explodir se for uma estrutura física (parede/barreira).
            // Lembre-se de garantir que as suas barreiras de defesa tenham a tag "Barrier" (ou mude a string abaixo pro nome que usar)!
            if (!collision.CompareTag("Barrier"))
            {
                // Se não for uma barreira, é um colisor secundário (como radar de inimigo). Ignora!
                return;
            }
        }

        // 3. O Fim da Bala! 
        // Se o código chegou até aqui, é porque ela acertou um alvo inimigo OU uma Barreira.
        speed = 0;

        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        coll.enabled = false;
    }
}

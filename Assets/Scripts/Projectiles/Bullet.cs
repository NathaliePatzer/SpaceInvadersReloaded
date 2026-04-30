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
        if (collision.GetComponent<InvadedTrigger>())
            return;

        // ---  A bala ignora objetos com a Tag "PowerUp" ---
        if (collision.CompareTag("PowerUp"))
            return;

        // --- O CONSERTO ESTÁ AQUI: A bala ignora outras balas! ---
        if (collision.GetComponent<Bullet>())
            return;

        IShootable shootable = collision.GetComponent<IShootable>();
        if (shootable != null)
        {
            if (shootable.GetTeam() != team)
            {
                shootable.OnShot(this);
            }
            else
            {
                return;
            }
        }

        speed = 0;
        animator.SetTrigger("Hit");
        coll.enabled = false;
    }
}

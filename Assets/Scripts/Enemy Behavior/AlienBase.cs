using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienBase : MonoBehaviour
{
    public int scoreValue;
    protected Animator animator;
    protected Rigidbody2D rb;

    // TRAVA DE SEGURANÇA (QA): Impede que o alien morra duas vezes no mesmo frame
    protected bool isDead = false;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    public Team GetTeam()
    {
        return Team.Aliens;
    }

    public virtual void OnShot(Bullet bullet)
    {
        // Se a bala bater mas ele já estiver morto, aborta a missão!
        if (isDead) return;
        isDead = true; // Tranca o cadeado da morte

        if (bullet != null) bullet.speed = 0;

        if (animator != null) animator.SetTrigger("Death");

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (ScoreSystem.Instance != null) ScoreSystem.Instance.AddScore(scoreValue);
    }
}

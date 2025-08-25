using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        weapon = GetComponentInChildren<Weapon>();
        animator = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();
        _speed = speed;
        originalCooldown = weapon.cooldown;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * Time.fixedDeltaTime * speed);
    }

    public void Movement(InputAction.CallbackContext context)
    {
        //down
        if (context.performed)
        {
            movement = context.ReadValue<Vector2>();
        }
        //up
        else if (context.canceled)
        {
            movement = Vector2.zero;
        }
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            weapon.ShootBullet();
        }
    }

    public void OnShot(Bullet bullet)
    {
        bullet.speed = 0;
        lives--;
        LivesHearts.Instance.UpdateHearts(lives);
        animator.SetTrigger("Death");
        animator.SetInteger("Lives", lives);
        audioSource.Play();
        StartCoroutine(Invencible());
        if (lives <= 0)
        {
            GameOver.Instance.OnGameOver(2000);
        }
    }

    public Team GetTeam()
    {
        return team;
    }

    IEnumerator Invencible()
    {
        coll.enabled = false;
        speed = 0;
        yield return new WaitForSeconds(1.5f);
        coll.enabled = true;
        speed = _speed;
    }

    public void ReduceWeaponCooldown(float amount, float duration)
    {
        if (cooldownRoutine != null)
        {
            StopCoroutine(cooldownRoutine);
            weapon.cooldown = originalCooldown; // restaura antes de aplicar novo
        }

        originalCooldown = weapon.cooldown;
        cooldownRoutine = StartCoroutine(ApplyTemporaryCooldown(amount, duration));
    }

    IEnumerator ApplyTemporaryCooldown(float amount, float duration)
    {
        weapon.cooldown = Mathf.Max(0.1f, weapon.cooldown - amount);

        yield return new WaitForSeconds(duration);

        weapon.cooldown = originalCooldown;
        cooldownRoutine = null;
    }

}

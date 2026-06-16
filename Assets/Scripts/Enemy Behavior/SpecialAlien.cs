using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAlien : AlienBase, IShootable
{
    Transform wall;
    public float speed;
    Vector2 direction;
    public GameObject extraLifePrefab;
    [Range(0f, 1f)] public float dropChance = 1f;

    [Header("Efeitos Visuais e Sonoros")]
    public GameObject textinhoFlutuantePrefab; 
    private AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake(); 
        audioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
       if (!isDead)
        {
            rb.MovePosition(rb.position + Time.fixedDeltaTime * speed * direction);
        }
    }
    public void StartMove(Transform rcWall)
    {
        wall = rcWall;
        if (transform.position.x > wall.position.x)
        {
            direction = Vector2.left;
        }
        else
        {
            direction = Vector2.right;
            GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    public override void OnShot(Bullet bullet)
    {
        if (isDead) return; // A nossa trava antimorte dupla

        // 1. Chama a base para somar o score
        base.OnShot(bullet);
        speed = 0;

        // --- A NOSSA ARMADILHA DE DETETIVE ---
        // Isso vai imprimir no console EXATAMENTE quantos pontos ele mandou somar!
        //Debug.Log("<color=yellow>[SPECIAL ALIEN]</color> Fui atingido! Enviando " + scoreValue + " pontos para o ScoreSystem.");

        // 2. O Juice (Texto e Som)
        if (audioSource != null)
        {
            audioSource.Play();
        }

        if (textinhoFlutuantePrefab != null)
        {
            Instantiate(textinhoFlutuantePrefab, transform.position, Quaternion.identity);
        }

        // 3. Drop do Power Up
        TryDropExtraLife();
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (isDead) return;

        if (wall == coll.transform)
        {
            Destroy(this.gameObject);
        }
    }

    void TryDropExtraLife()
    {
        if (Random.value <= dropChance)
        {
            Instantiate(extraLifePrefab, transform.position, Quaternion.identity);
        }

    }
}
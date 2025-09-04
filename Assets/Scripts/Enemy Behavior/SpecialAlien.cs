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

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + Time.fixedDeltaTime * speed * direction);
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
        base.OnShot(bullet);
        speed = 0;
        TryDropExtraLife();
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
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
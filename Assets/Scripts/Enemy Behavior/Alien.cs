using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alien : AlienBase, IShootable
{
    [HideInInspector]
    public Weapon weapon;
    public Vector2Int matrixPos;

    protected override void Awake()
    {
        base.Awake();
        weapon = GetComponentInChildren<Weapon>();
    }

    public override void OnShot(Bullet bullet)
    {
        base.OnShot(bullet);
        AlienController.Instance.OnAlienDeath(matrixPos);
    }

    public void StartShooting()
    {
        StartCoroutine(AlienShooting());
    }

    public void MoveTo(Vector2 direction, float speed)
    {
        rb.MovePosition(rb.position + direction * speed);
        animator.SetTrigger("Move");
    }

    IEnumerator AlienShooting()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3, 15));
            // MODO METRALHADORA ON
            // yield return new WaitForSeconds(Random.Range(1, 8));
            Shoot();
        }
    }

    void Shoot()
    {
        weapon.ShootBullet();
    }

}

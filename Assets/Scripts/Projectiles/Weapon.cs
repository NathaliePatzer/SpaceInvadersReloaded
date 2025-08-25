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
        Bullet instantiated = PoolingSystem.Instance.GetBullet(bullet);
        instantiated.direction = bulletDirection;
        instantiated.speed = bulletSpeed;
        instantiated.team = shootable.GetTeam();
        instantiated.transform.position = transform.position;
        instantiated.gameObject.SetActive(true);

        if (audioSource != null)
            audioSource.Play();
    }


    public void ReduceCooldown(float amount)
    {
        cooldown = Mathf.Max(0.1f, cooldown - amount);
    }

}

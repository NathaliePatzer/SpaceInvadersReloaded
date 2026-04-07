using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public float attractionRadius = 15f;

    private void Update()
    {
        foreach (var bullet in PoolingSystem.Instance.activeBullets)
        {
            // --- Se a bala for nula (destruída por algum motivo), pula ela e segue o jogo! ---
            if (bullet == null) continue;
            
            if (bullet.gameObject.activeInHierarchy && bullet.CompareTag("AlienProjectile"))
            {
                float distance = Vector2.Distance(bullet.transform.position, transform.position);
                if (distance <= attractionRadius)
                {
                    bullet.SetAttractionTarget(transform.position);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("AlienProjectile"))
        {
            Bullet bullet = other.GetComponent<Bullet>();
            if (bullet != null)
            {
                PoolingSystem.Instance.StoreBullet(bullet);
            }
        }
    }

    private void OnDestroy()
    {
        foreach (var bullet in PoolingSystem.Instance.activeBullets)
        {
            if (bullet != null && bullet.gameObject.activeInHierarchy && bullet.CompareTag("AlienProjectile"))
            {
                bullet.ClearAttractionTarget();
            }
        }
    }

    

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attractionRadius);
    }
}

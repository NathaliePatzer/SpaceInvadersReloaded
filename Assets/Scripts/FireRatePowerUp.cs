using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRatePowerUp : MonoBehaviour
{
    public float cooldownReduction = 0.8f;
    public float duration = 5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ReduceWeaponCooldown(cooldownReduction, duration);
            }
            
            Destroy(gameObject);
        }
    }

}

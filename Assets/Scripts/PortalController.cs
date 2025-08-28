using UnityEngine;
using System.Collections;

public class PortalController : MonoBehaviour
{
    private float remainingTime;
    private bool isActive = false;
    private Animator animator;
    private bool isDisappearing = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void StartPortal(float duration)
    {
        remainingTime = duration;
        isActive = true;
        isDisappearing = false;
    }

    private void Update()
    {
        if (!isActive || isDisappearing) return;

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            StartCoroutine(PlayExitAnimationAndDestroy());
        }
    }

    private IEnumerator PlayExitAnimationAndDestroy()
    {
        isDisappearing = true;

        if (animator != null)
        {
            animator.SetTrigger("Exit");
            yield return new WaitForSeconds(1f); // tempo da animação de saída
        }

        Destroy(gameObject);
    }
}

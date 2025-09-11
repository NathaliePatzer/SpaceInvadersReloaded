using UnityEngine;

public class HeartStatus : MonoBehaviour
{
    private Animator animator;
    public bool IsDisappearing { get; private set; } = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void TriggerDisappear()
    {
        Debug.Log("TriggerDisappear chamado");

        if (animator != null)
        {
            animator.SetTrigger("Disappear");
            IsDisappearing = true;
        }
    }
}

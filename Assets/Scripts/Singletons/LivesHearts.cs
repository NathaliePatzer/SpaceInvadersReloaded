using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivesHearts : MonoBehaviour
{
    public static LivesHearts Instance;

    public Animator[] heartAnimators;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void UpdateHearts(int livesLeft)
    {
        livesLeft = Mathf.Clamp(livesLeft, 0, heartAnimators.Length);

        for (int i = 0; i < heartAnimators.Length; i++)
        {
            if (i >= livesLeft)
            {
                // DANO: Manda apagar. Resetar o oposto previne bugs visuais.
                heartAnimators[i].SetTrigger("Disappear");
                heartAnimators[i].ResetTrigger("Appear"); // <-- DICA DE QUALIDADE QA!
            }
            else
            {
                // CURA: Manda reaparecer.
                heartAnimators[i].SetTrigger("Appear");
                heartAnimators[i].ResetTrigger("Disappear"); // <-- DICA DE QUALIDADE QA!
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriforcePowerUp : MonoBehaviour
{
    // Variáveis para guardar os componentes locais
    private Animator anim;
    private Collider2D col;
    private Rigidbody2D rb;
    private bool jaColetado = false; // Flag de segurança

    [Header("Configurações de Coleta")]
    public float tempoParaDestruir = 1f; // Tempo da sua animação 'Colided'
    
    private AudioSource audioSource;

    private void Awake()
    {
        // Pega as referências dos componentes no mesmo GameObject
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se é o player e se já não foi coletado (segurança de QA!)
        if (other.CompareTag("Player") && !jaColetado)
        {
            jaColetado = true; // Tranca a porta

            // 1. Lógica do Power-Up na Nave
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.AtivarTiroTriplo();
            }

            // 3. Iniciar o feedback visual (Animação)
            InicializarFeedbackColeta();
        }
    }

    private void InicializarFeedbackColeta()
    {
        // Desativa o colisor imediatamente para não interagir com mais nada
        if (col != null) col.enabled = false;

        // Congela o Rigidbody para ele parar de cair e tocar a animação no lugar
        if (rb != null)
        {
            rb.velocity = Vector2.zero; // Zera a velocidade atual
            rb.simulated = false;       // Desliga a simulação física
        }

        // Toca a animação setando o trigger 'Colided'
        if (anim != null)
        {
            anim.SetTrigger("Colided"); // <--- SEU TRIGGER AQUI
        }

        if (audioSource != null)
        {
            audioSource.Play();
        }

        // Agenda a destruição do objeto para depois que a animação acabar
        Destroy(gameObject, tempoParaDestruir);
    }

    private void Update()
    {
        // Faxina de QA: Se não foi coletado e saiu da tela, deleta.
        // Se já foi coletado, ignora essa lógica (jaColetado impede o Destroy prematuro).
        if (!jaColetado && transform.position.y < -7f)
        {
            Destroy(gameObject);
        }
    }
}

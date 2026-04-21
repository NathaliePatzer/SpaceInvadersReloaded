using UnityEngine;

public class Rupee : MonoBehaviour
{
    [Header("Pontuação")]
    public int valorPontuacao = 10; // Você pode mudar isso no Inspector de cada Prefab!

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Se bateu no jogador...
        if (other.CompareTag("Player"))
        {
            // Adiciona a pontuação
            if (ScoreSystem.Instance != null)
            {
                ScoreSystem.Instance.AddScore(valorPontuacao);
            }

            // Opcional: Se quiser, pode tocar um sonzinho de "Moeda" aqui depois!

            // O jogador pegou, então destrói a rupia
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Faxina: Se a rupia não for pega e cair além do fundo da tela, a gente destrói ela
        // para não acumular lixo na memória do jogo.
        if (transform.position.y < -7f)
        {
            Destroy(gameObject);
        }
    }
}
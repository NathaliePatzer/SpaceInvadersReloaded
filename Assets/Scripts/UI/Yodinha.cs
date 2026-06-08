using UnityEngine;

public class Yodinha : MonoBehaviour
{
    public float velocidade = 3f;
    public float limiteX = 8f; // Até onde ele vai antes de virar
    private int direcao = 1;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Move o Yoda
        transform.Translate(Vector3.right * direcao * velocidade * Time.deltaTime);

        // Checa se bateu no limite da direita
        if (transform.position.x > limiteX)
        {
            direcao = -1; // Volta pra esquerda
            if (sr != null) sr.flipX = true; // Vira o rostinho dele
        }
        // Checa se bateu no limite da esquerda
        else if (transform.position.x < -limiteX)
        {
            direcao = 1; // Vai pra direita
            if (sr != null) sr.flipX = false; // Desvira o rostinho
        }
    }
}
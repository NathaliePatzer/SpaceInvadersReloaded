using UnityEngine;
using TMPro; // Biblioteca obrigatória para mexer com TextMeshPro!

public class FloatingText : MonoBehaviour
{
    public float destroyTime = 1f; // Quanto tempo ele dura na tela
    public float moveSpeed = 1.5f; // Velocidade que ele sobe
    public Vector3 offset = new Vector3(0, 0.5f, 0); // Faz ele nascer um pouquinho acima do morcego

    private TextMeshPro textMesh;
    private Color originalColor;
    private float timer;

    void Start()
    {
        // Pega o componente de texto e salva a cor original dele
        textMesh = GetComponent<TextMeshPro>();
        originalColor = textMesh.color;

        // Empurra ele um pouquinho pra cima pra não nascer exatamente no meio da explosão
        transform.position += offset;
    }

    void Update()
    {
        // 1. Move o texto para cima
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // 2. Calcula a transparência (Alpha) gradativa
        timer += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, timer / destroyTime);

        // Aplica a nova cor com a transparência alterada
        textMesh.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);

        // 3. Destrói o objeto quando o tempo acabar para não pesar o jogo
        if (timer >= destroyTime)
        {
            Destroy(gameObject);
        }
    }
}
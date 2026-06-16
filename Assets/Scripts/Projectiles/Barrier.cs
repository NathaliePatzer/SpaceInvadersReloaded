using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    [Header("Configurações da Barreira")]
    // Coloquei public para você poder testar valores diferentes direto no Inspector da Unity!
    public int maxLives = 40;

    // Essa variável guarda a vida do momento
    private int currentLives;
    public Sprite[] damageSprites;
    private SpriteRenderer spriteRenderer;
    public GameObject faiscaImpactoPrefab;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentLives = maxLives; // A barreira nasce com a vida cheia
        UpdateSprite();
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.GetComponent<Bullet>())
        {
            // <-- 2. A MÁGICA DO IMPACTO ACONTECE AQUI! -->
            if (faiscaImpactoPrefab != null)
            {
                // Cria a faísca exatamente na posição (X, Y) do tiro que bateu
                GameObject faisca = Instantiate(faiscaImpactoPrefab, coll.transform.position, Quaternion.identity);
                
                // Dica de Otimização de QA: Destrói o objeto da faísca após 0.5 segundos para não lotar a memória!
                Destroy(faisca, 0.5f); 
            }

            currentLives--;
            
            if (currentLives <= 0)
            {
                Destroy(this.gameObject); // Otimização: Só manda atualizar o sprite se não tiver destruído
            }
            else
            {
                UpdateSprite(); 
            }
        }
    }
     
    void UpdateSprite()
    {
        int index = 0;

        // A MÁGICA MATEMÁTICA: Descobre quanto vale 1/3 e 2/3 da vida máxima atual
        float umTerco = maxLives / 3f;
        float doisTercos = (maxLives * 2f) / 3f;

        // Verifica a vida atual contra as fatias matemáticas
        if (currentLives <= umTerco)
        {
            index = 2; // Vida no último terço (sprite mais destruído)
        }
        else if (currentLives <= doisTercos)
        {
            index = 1; // Vida no segundo terço (sprite levemente trincado)
        }
        else
        {
            index = 0; // Vida no primeiro terço (sprite intacto)
        }

        // Aplica o sprite com segurança
        if (damageSprites != null && damageSprites.Length > index)
        {
            spriteRenderer.sprite = damageSprites[index];
        }
    }

}

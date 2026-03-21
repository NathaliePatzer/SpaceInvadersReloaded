using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AlienController : MonoBehaviour
{
    [SerializeField] private GameObject fireRatePowerUpPrefab;
    [SerializeField] private GameObject portalPowerUpPrefab;
    [SerializeField] private float dropChance = 0.1f;
    [SerializeField] private float portalDropChance = 0.5f;

    public static AlienController Instance;
    public float alienSpeed = 0.1f;
    public float movementDelay = 0.1f;

    private float originalMovementDelay; // Para resetar a cada fase

    public Queue<Vector2> direction;
    public Alien[,] aliens = new Alien[15, 5];
    public SpecialAlien specialAlienPrefab;
    public List<Transform> walls;
    int remainingAliens;

    GameObject currentWaveInstance; // Guarda a wave atual para poder deletar depois

    [Header("Barreiras de Defesa")]
    public GameObject barrierGroupPrefab; // A "planta" das barreiras
    private GameObject currentBarrierGroup; // A barreira que está viva na tela no momento

    void Awake()
    {
        Instance = this;
        direction = new Queue<Vector2>();
        originalMovementDelay = movementDelay;
        // Removemos o SetMatrix e as coroutines daqui! O WaveManager que vai mandar começar.
    }

    // NOVA FUNÇÃO: O WaveManager chama essa função e passa o Prefab
    public void InitializeWave(GameObject wavePrefab)
    {
        StopAllCoroutines();
        direction.Clear();
        direction.Enqueue(Vector2.right);
        Array.Clear(aliens, 0, aliens.Length);
        movementDelay = originalMovementDelay;

        if (currentWaveInstance != null)
            Destroy(currentWaveInstance);

        currentWaveInstance = Instantiate(wavePrefab, transform.position, Quaternion.identity, transform);


        // --- A MÁGICA DA DETECÇÃO ---
        // Ele procura se essa Wave tem aliens comuns dentro dela
        Alien[] alienGOs = currentWaveInstance.GetComponentsInChildren<Alien>();

        if (alienGOs.Length > 0)
        {
            // É UMA WAVE NORMAL! Faz o trabalho de sempre:

            // Se as barreiras não existirem na tela, a gente cria elas novinhas em folha!
            if (currentBarrierGroup == null && barrierGroupPrefab != null)
            {
                currentBarrierGroup = Instantiate(barrierGroupPrefab);
            }

            SetMatrix();
            SetInitialShooting();
            StartCoroutine(Movement());
            StartCoroutine(SpecialAlienRoutine());
        }
        else
        {
            // É UM CHEFÃO!
            // Não iniciamos o movimento em matriz.
            // O próprio script do Demogorgon vai assumir o controle a partir daqui!
            // Destrói as barreiras para deixar a arena limpa e perigosa!
            if (currentBarrierGroup != null)
            {
                Destroy(currentBarrierGroup);
            }
            Debug.Log("Wave de Boss detectada! AlienController em modo de espera.");
        }
    }

    public void TryDropPowerUp(Vector3 position)
    {
        float roll = Random.value;
        if (roll < dropChance)
            Instantiate(fireRatePowerUpPrefab, position, Quaternion.identity);
        else if (roll < dropChance + portalDropChance)
            Instantiate(portalPowerUpPrefab, position, Quaternion.identity);
    }

    public void OnAlienDeath(Vector2Int matrixPos)
    {
        Alien alien = aliens[matrixPos.x, matrixPos.y];
        if (alien != null)
        {
            TryDropPowerUp(alien.transform.position);
        }

        remainingAliens--;
        movementDelay -= 0.0025f;
        BGMController.Instance.IncreaseSpeed();

        // AQUÍ É A MÁGICA: Em vez de Game Over, chamamos a próxima Wave!
        if (remainingAliens <= 0)
        {
            StopAllCoroutines(); // Para tudo dessa wave
            WaveManager.Instance.OnWaveCompleted(); // Avisa o chefe!
            return; // Importante para ele não tentar atirar depois de morto
        }

        Alien nextAlien = null;
        for (int i = matrixPos.y + 1; i < aliens.GetLength(1) && nextAlien == null; i++)
        {
            nextAlien = aliens[matrixPos.x, i];
        }

        if (nextAlien != null)
        {
            nextAlien.StartShooting();
        }
    }

    void SetMatrix()
    {
        // Agora ele lê apenas os aliens da Wave instanciada, não da cena inteira
        Alien[] alienGOs = currentWaveInstance.GetComponentsInChildren<Alien>();
        remainingAliens = alienGOs.Length; // Muito mais seguro! Permite waves com "buracos" no desenho.

        float offsetX = 8;
        float offsetY = -1.44f;
        foreach (Alien a in alienGOs)
        {
            int xPos = Mathf.FloorToInt(a.transform.localPosition.x + offsetX);
            int yPos = Mathf.FloorToInt((a.transform.localPosition.y + offsetY) / 0.75f);
            aliens[xPos, yPos] = a;
            a.matrixPos = new Vector2Int(xPos, yPos);
        }
    }

    void SetInitialShooting()
    {
        for (int i = 0; i < aliens.GetLength(0); i++)
        {
            if (aliens[i, 0] != null) // Prevenção de erro caso a wave tenha espaços vazios
                aliens[i, 0].StartShooting();
        }
    }

    // ... (Os métodos Movement(), SpecialAlienRoutine() e GetGroupBounds() continuam EXATAMENTE IGUAIS ao seu original)
    IEnumerator Movement()
    {
        while (true)
        {
            Vector2 currentDirection = direction.Dequeue();
            if (direction.Count == 0) direction.Enqueue(currentDirection);

            Vector2 bounds = GetGroupBounds();
            float leftWallX = walls[0].position.x;
            float rightWallX = walls[1].position.x;
            float margin = 0.8f;

            if ((currentDirection == Vector2.right && bounds.y >= rightWallX - margin) ||
                (currentDirection == Vector2.left && bounds.x <= leftWallX + margin))
            {
                direction.Clear();
                direction.Enqueue(Vector2.down);
                direction.Enqueue(currentDirection == Vector2.right ? Vector2.left : Vector2.right);
            }

            for (int i = 0; i < aliens.GetLength(1); i++)
            {
                for (int j = 0; j < aliens.GetLength(0); j++)
                {
                    if (aliens[j, i] != null && aliens[j, i].gameObject.activeSelf)
                    {
                        aliens[j, i].MoveTo(currentDirection, alienSpeed);
                    }
                }
                yield return new WaitForSeconds(movementDelay);
            }
        }
    }

    IEnumerator SpecialAlienRoutine()
    {
        while (true)
        {
            int rand = Random.Range(-3, 7);
            yield return new WaitForSeconds(10 + rand);
            int side = Random.Range(0, 2);
            int otherside = 1 - side;
            Vector3 position = new Vector3(walls[side].position.x, 4.0f, 0);
            SpecialAlien specialAlien = Instantiate(specialAlienPrefab, position, Quaternion.identity, this.transform);
            specialAlien.StartMove(walls[otherside]);
        }
    }

    Vector2 GetGroupBounds()
    {
        float minX = float.MaxValue;
        float maxX = float.MinValue;

        foreach (Alien a in aliens)
        {
            if (a != null && a.gameObject.activeSelf)
            {
                float x = a.transform.position.x;
                if (x < minX) minX = x;
                if (x > maxX) maxX = x;
            }
        }
        return new Vector2(minX, maxX);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [Header("Configuração das Waves")]
    public GameObject[] wavePrefabs; // Arraste os prefabs Wave_01, Wave_02 aqui
    public AudioClip[] waveMusics;
    public int currentWaveIndex = 0;

    [Header("Interface (UI)")]
    public Text waveText; // Arraste um texto da UI para mostrar "WAVE 1"

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(StartWaveRoutine());
    }

    public IEnumerator StartWaveRoutine()
    {
        // 1. Tranca o pause na transição
        PauseMenu.PodePausar = false;

        // --- A GRANDE SACADA ---
        // Verifica se o índice atual é exatamente o do último prefab da lista
        bool isBossWave = (currentWaveIndex == wavePrefabs.Length - 1);

        // Se for a wave do Boss (o Demogorgon chegou!), apertamos o botão de pânico!
        if (isBossWave)
        {
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.LimparTodosOsBuffs(); // Faxina completa antes da luta!
            }
        }

        // 2. Mostra o texto da transição
        if (waveText != null)
        {
            if (isBossWave)
            {
                // Bônus: Muda o texto para avisar do Boss!
                waveText.text = "Final fight";
                waveText.color = Color.red; // (Opcional) Pinta de vermelho pra dar tensão
            }
            else
            {
                // Texto normal para waves normais
                waveText.text = "Wave " + (currentWaveIndex + 1);
            }

            waveText.gameObject.SetActive(true);
        }

        // Pausa dramática
        yield return new WaitForSeconds(3f);

        if (waveText != null)
        {
            waveText.gameObject.SetActive(false);
            waveText.color = Color.white; // Garante que a cor volte ao normal no futuro
        }

        // 3. Instancia o prefab (seja wave normal ou a wave sem aliens do boss)
        GameObject currentWavePrefab = wavePrefabs[currentWaveIndex];

        // 4. Manda o AlienController assumir o controle!
        AlienController.Instance.InitializeWave(currentWavePrefab);

        // 5. Chamar o BGM
        if (currentWaveIndex < waveMusics.Length && waveMusics[currentWaveIndex] != null)
        {
            BGMController.Instance.PlayNewTrack(waveMusics[currentWaveIndex]);
        }

        // 6. Destranca o pause
        PauseMenu.PodePausar = true;
    }

    public void OnWaveCompleted()
    {
        currentWaveIndex++;

        if (currentWaveIndex < wavePrefabs.Length)
        {
            // Tem mais waves! (Pode ser normal ou a do Boss, o StartWaveRoutine resolve)
            StartCoroutine(StartWaveRoutine());
        }
        else
        {
            // Acabaram TODAS as waves do array (o Boss foi derrotado!)
            // Aqui entra a sua tela de Game Over / Vitória
            Debug.Log("Todas as waves concluídas! Você venceu o jogo!");
            WinGame();
        }
    }

    public async void WinGame()
    {
        Debug.Log("O Boss caiu! Vitória!");

        // 1. Tranca o pause (regra de ouro de QA!)
        PauseMenu.PodePausar = false;

        // 2. Mostra o texto de VICTORY na tela
        if (waveText != null)
        {
            waveText.text = "Victory!";
            waveText.color = Color.yellow; // Cor de ouro!
            waveText.gameObject.SetActive(true);
        }

        // 3. Espera 3 segundos para o jogador comemorar
        await System.Threading.Tasks.Task.Delay(5000);

        // 4. Salva o Score final antes de ir pros créditos
        ScoreSystem.Instance.SaveHiScore();

        // 5. Carrega a cena dos créditos
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
}

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
        // 1. Mostra o texto da Wave
        if (waveText != null)
        {
            waveText.text = "Wave " + (currentWaveIndex + 1);
            waveText.gameObject.SetActive(true);
            yield return new WaitForSeconds(3f); // Tempo que o texto fica na tela
            waveText.gameObject.SetActive(false);
        }

        // 2. Instancia o prefab da wave atual
        GameObject currentWavePrefab = wavePrefabs[currentWaveIndex];

        // 3. Manda o AlienController assumir o controle!
        AlienController.Instance.InitializeWave(currentWavePrefab);

        //4 . Chamar o BGM
        if (currentWaveIndex < waveMusics.Length && waveMusics[currentWaveIndex] != null)
        {
            BGMController.Instance.PlayNewTrack(waveMusics[currentWaveIndex]);
        }
    }

    public void OnWaveCompleted()
    {
        currentWaveIndex++;

        if (currentWaveIndex < wavePrefabs.Length)
        {
            // Tem mais waves! Chama a próxima.
            StartCoroutine(StartWaveRoutine());
        }
        else
        {
            // Acabaram as waves! Aqui você vai colocar o Boss no futuro.
            // Por enquanto, vamos dar um Game Over de vitória.
            Debug.Log("Todas as waves concluídas!");
            GameOver.Instance.OnGameOver(2000);
        }
    }
}
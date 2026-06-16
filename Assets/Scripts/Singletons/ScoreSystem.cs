using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreSystem : MonoBehaviour
{
    const string HighScoreKey = "HiScore";
    public Text score;
    public Text hiscore;
    int scorePoints;
    public static ScoreSystem Instance;
    void Awake()
    {
        // O Cadeado de Segurança contra Impostores!
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("⚠️ ATENÇÃO: Detectamos um ScoreSystem duplicado na cena! Destruindo o impostor.");
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }
    void Start()
    {
        LoadHiScore();
    }
    public void AddScore(int points)
    {
        scorePoints += points;
        
        if (score != null)
        {
            score.text = "" + scorePoints;
            Debug.Log($"<color=cyan>[SCORE SYSTEM]</color> Sucesso! Recebi +{points} pontos. Placar atualizado na tela para: {score.text}");
        }
        else
        {
            Debug.LogError("<color=red>[SCORE SYSTEM]</color> SOCORRO! Recebi os pontos, mas a variável 'score' (o Texto da UI) está vazia no Inspector!");
        }
    }
    [ContextMenu("Test save")]
    public void SaveHiScore()
    {
        int hiscoreValue = int.Parse(hiscore.text);
        if (hiscoreValue < scorePoints)
        {
            PlayerPrefs.SetInt(HighScoreKey, scorePoints);
        }

        // Salva a pontuação desta rodada no HD com uma chave diferente!
        PlayerPrefs.SetInt("LastScore", scorePoints);

    }
    void LoadHiScore()
    {
        hiscore.text = "" + PlayerPrefs.GetInt(HighScoreKey);
    }
    [ContextMenu("Test clear")]
    void ClearHiScore()
    {
        PlayerPrefs.DeleteKey(HighScoreKey);
    }
}

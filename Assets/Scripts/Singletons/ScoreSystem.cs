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
        Instance = this;
    }
    void Start()
    {
        LoadHiScore();
    }
    public void AddScore(int points)
    {
        scorePoints += points;
        score.text = "" + scorePoints;
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

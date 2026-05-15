using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public static GameOver Instance;
    public Text gameOverText;
    bool gameOvering;
    void Awake()
    {
        // 1. Se já existe uma Instância e não sou eu, me destruo e PARO de ler o código!
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return; // <-- A palavra mágica que resolve o erro do fantasma!
        }

        // 2. Se eu sou o primeiro a nascer, assumo o trono e não morro mais.
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    public async void OnGameOver(int delay)
    {
        if (gameOvering)
        {
            return;
        }
        else
        {
            gameOvering = true;
            // Assim que a nave morre, a gente desliga a permissão do pause
            PauseMenu.PodePausar = false;
        }
        ScoreSystem.Instance.SaveHiScore();
        Debug.Log("Game over");
        await Task.Delay(delay);
        gameOverText.enabled = true;
        Time.timeScale = 0;
        await Task.Delay(2000);
        StartCoroutine(Reload());
    }
    IEnumerator Reload()
    {
        Debug.Log("Recarregando");
        yield return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        Time.timeScale = 1;
        // A cena reiniciou e o jogador está vivo, então ele pode pausar de novo!
        PauseMenu.PodePausar = true;
        gameOverText = GameObject.Find("Canvas/GameOvertxt").GetComponent<Text>();
        gameOvering = false;
    }
}

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
            PauseMenu.PodePausar = false;
        }

        if (ScoreSystem.Instance != null)
        {
            ScoreSystem.Instance.SaveHiScore();
        }

        Debug.Log("Game over");

        // Espera o tempo dramático (ex: explosão da nave)
        await Task.Delay(delay);

        // --- A BLINDAGEM DE QA ---
        // Antes de tentar ligar o texto, nós verificamos se ele se perdeu na troca de cena
        if (gameOverText == null)
        {
            GameObject textObj = GameObject.Find("Canvas/GameOvertxt");
            if (textObj != null)
            {
                gameOverText = textObj.GetComponent<Text>();
            }
        }

        // Agora sim, ligamos com segurança (se ele foi encontrado)
        if (gameOverText != null)
        {
            gameOverText.enabled = true;
        }
        else
        {
            Debug.LogWarning("[GameOver] O texto de GameOvertxt não foi encontrado no Canvas!");
        }

        Time.timeScale = 0; // Pausa o mundo

        await Task.Delay(2000); // Espera 2 segundos com o texto na tela

        StartCoroutine(Reload());
    }
    IEnumerator Reload()
    {
        Debug.Log("Recarregando");
        yield return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);

        Time.timeScale = 1;
        PauseMenu.PodePausar = true;

        // Como você já fazia, recupera a referência para a próxima partida
        GameObject textObj = GameObject.Find("Canvas/GameOvertxt");
        if (textObj != null)
        {
            gameOverText = textObj.GetComponent<Text>();
        }

        gameOvering = false;
    }
}

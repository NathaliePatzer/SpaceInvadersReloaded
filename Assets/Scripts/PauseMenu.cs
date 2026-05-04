using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Necessário para trocar de cena
using UnityEngine.InputSystem; // O pacote do seu Input System!

public class PauseMenu : MonoBehaviour
{
    // Uma flag global para outros scripts saberem se o jogo está pausado
    public static bool GameIsPaused = false;

    [Header("UI do Menu")]
    public GameObject pauseMenuUI; // Onde vamos arrastar o seu Panel

    void Update()
    {
        // Checa se o ESC foi apertado no teclado
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false); // Esconde a tela escurecida
        Time.timeScale = 1f; // O tempo volta a correr!
        GameIsPaused = false;

        // Dica de QA: Despausa a música do jogo!
        AudioListener.pause = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true); // Mostra a tela escurecida
        Time.timeScale = 0f; // Congela a física e os updates!
        GameIsPaused = true;

        // Dica de QA: Pausa o som para não ficar tocando música de tensão com o jogo parado
        AudioListener.pause = true;
    }

    // Função para o botão "Menu Principal" (se tiver um)
    public void LoadMenu()
    {
        // REGRA DE OURO: Sempre descongele o tempo antes de sair da cena, 
        // senão o Menu Principal vai carregar todo travado!
        Time.timeScale = 1f;
        AudioListener.pause = false;
        GameIsPaused = false;

        SceneManager.LoadScene("Menu"); // Tela de menu
    }

    // Função para o botão "Sair do Jogo"
    public void QuitGame()
    {
        Debug.Log("Saindo do jogo...");
        Application.Quit(); // Isso só funciona na build final (no .exe), não no Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Para parar o jogo no editor
#endif
    }
}

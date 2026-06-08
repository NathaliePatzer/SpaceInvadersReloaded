using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Necessário para trocar de cena
using UnityEngine.InputSystem; // O pacote do seu Input System!
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    // Uma flag global para outros scripts saberem se o jogo está pausado
    public static bool GameIsPaused = false;
    // --- O CADEADO GLOBAL ---
    // Começa verdadeiro, mas outros scripts podem desligar isso!
    public static bool PodePausar = true;

    [Header("UI do Menu")]
    public GameObject pauseMenuUI; // Onde vamos arrastar o seu Panel

    [Header("Navegação do Controle")]
    public GameObject botaoInicial;

    void Update()
    {
        // Checa se o ESC foi apertado no teclado
        // Só tenta ler o botão ESC se o cadeado estiver aberto!
        if (PodePausar)
        {
            // 1. Checa se o ESC foi apertado no teclado
            bool pausouNoTeclado = Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame;

            // 2. Checa se o botão X (Button West) foi apertado no controle (Gamepad)
            bool pausouNoControle = Gamepad.current != null && Gamepad.current.buttonWest.wasPressedThisFrame;

            // Se apertou em QUALQUER um dos dois, roda a lógica de pause!
            if (pausouNoTeclado || pausouNoControle)
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
        if (GameIsPaused)
        {
            // Se a Unity perdeu a referência (foco = null) porque alguém mexeu o mouse...
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                // ...nós devolvemos o foco para o botão inicial na marra!
                EventSystem.current.SetSelectedGameObject(botaoInicial);
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

        // Quando pausar, já garante que o botão inicial seja selecionado pro controle funcionar de cara
        EventSystem.current.SetSelectedGameObject(null); // Limpa qualquer lixo
        EventSystem.current.SetSelectedGameObject(botaoInicial);
    }

    // Função para o botão "Menu Principal" (se tiver um)
    public void LoadMenu()
    {
        // REGRA DE OURO: Sempre descongele o tempo antes de sair da cena, 
        // senão o Menu Principal vai carregar todo travado!
        Time.timeScale = 1f;
        AudioListener.pause = false;
        GameIsPaused = false;
        // Importante: destranca o pause ao voltar pro menu, pro jogo não começar trancado na próxima vez!
        PodePausar = true;

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

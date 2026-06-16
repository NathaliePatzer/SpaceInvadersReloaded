using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuManagement : MonoBehaviour
{

    [Header("Navegação do Controle")]
    public GameObject botaoInicial;

    void Start()
    {
        // Assim que a cena do menu carregar, já garante que o controle tem um foco!
        if (botaoInicial != null && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(botaoInicial);
        }
    }

    void Update()
    {
        // --- A TRAVA DE SEGURANÇA CONTRA O MOUSE ---
        // Se a Unity perder a referência (foco = null) porque alguém esbarrou no mouse...
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null)
        {
            // ...nós devolvemos o foco para o botão "Jogar" na marra!
            EventSystem.current.SetSelectedGameObject(botaoInicial);
        }
    }

    // Este método é chamado quando o botão "Jogar" é clicado
    public void Jogar()
    {
        SceneManager.LoadScene("Preparing");
    }

    // Este método é chamado quando o botão "Sair" é clicado
    public void Sair()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Para parar o jogo no editor
#endif
    }
}

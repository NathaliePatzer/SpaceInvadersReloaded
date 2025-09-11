using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManagement : MonoBehaviour
{
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

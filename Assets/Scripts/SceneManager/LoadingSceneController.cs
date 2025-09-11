using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingSceneController : MonoBehaviour
{
    public string nomeCenaDestino = "FaseOne"; // defina no Inspector
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(CarregarCena());
        Invoke(nameof(TocarAudio), 12f); 
    }

    IEnumerator CarregarCena()
    {
        yield return new WaitForSeconds(15.8f); // tempo para mostrar algo na tela
        SceneManager.LoadScene(nomeCenaDestino);
    }

    void TocarAudio()
    {
        if (audioSource != null)
            audioSource.Play();
    }


}

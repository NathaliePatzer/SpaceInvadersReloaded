using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class VictorySceneController : MonoBehaviour
{
    [Header("Configurações de Tela")]
    public VideoPlayer vPlayer;
    public RawImage telaDoVideo;
    public GameObject menuFinal;

    [Header("Easter Egg")]
    public GameObject babyYoda; // <-- 1. Criamos a variável para receber a sua pixel art!

    [Header("Textos")]
    public TextMeshProUGUI textoHighScore;
    public TextMeshProUGUI scoreDaPartidaText;

    [Header("Fade do Menu")]
    public CanvasGroup grupoDoMenu;
    public float tempoDeFade = 2f;

    [Header("Áudio")]
    public AudioSource musicaEstrelas; 

    void Start()
    {
        Time.timeScale = 1f;

        // <-- 2. Garante que o Baby Yoda comece escondido e paradinho
        if (babyYoda != null)
        {
            babyYoda.SetActive(false);
        }

        menuFinal.SetActive(false);
        if (grupoDoMenu != null)
        {
            grupoDoMenu.alpha = 0f;
            grupoDoMenu.interactable = false;
            grupoDoMenu.blocksRaycasts = false;
        }

        telaDoVideo.gameObject.SetActive(true);
        vPlayer.loopPointReached += AoTerminarVideo;
    }

    void AoTerminarVideo(VideoPlayer vp)
    {
        telaDoVideo.gameObject.SetActive(false);
        menuFinal.SetActive(true);

        // <-- 3. O vídeo acabou! Hora de dar o play nas viagens dele!
        if (babyYoda != null)
        {
            babyYoda.SetActive(true);
        }

        if (scoreDaPartidaText != null)
        {
            scoreDaPartidaText.text = "Final score: " + PlayerPrefs.GetInt("LastScore").ToString();
        }

        if (textoHighScore != null)
        {
            textoHighScore.text = "High score: " + PlayerPrefs.GetInt("HiScore").ToString();
        }

        if (musicaEstrelas != null)
        {
            musicaEstrelas.Play();
        }

        if (grupoDoMenu != null)
        {
            StartCoroutine(AparecerMenuSuavemente());
        }
    }

    IEnumerator AparecerMenuSuavemente()
    {
        float tempoDecorrido = 0f;

        while (tempoDecorrido < tempoDeFade)
        {
            tempoDecorrido += Time.deltaTime;
            grupoDoMenu.alpha = Mathf.Lerp(0f, 1f, tempoDecorrido / tempoDeFade);
            yield return null;
        }

        grupoDoMenu.alpha = 1f;
        grupoDoMenu.interactable = true;
        grupoDoMenu.blocksRaycasts = true;
    }

    public void VoltarAoMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
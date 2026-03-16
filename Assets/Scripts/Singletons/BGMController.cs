using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMController : MonoBehaviour
{
    public static BGMController Instance;
    public const string PitchParameter = "PitchValue";
    public float increaseAmount;
    float increased = 1;
    public AudioSource audioSource;
    private float originalPitch = 1f; // Para lembrar a velocidade normal

    void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }
    [ContextMenu("Increase")]
    public void IncreaseSpeed()
    {
        increased += increaseAmount;
        audioSource.pitch = increased;
        audioSource.outputAudioMixerGroup.audioMixer.SetFloat(PitchParameter, 1 / increased);
    }

    void OnDisable()
    {
        audioSource.outputAudioMixerGroup.audioMixer.SetFloat(PitchParameter, 1);
    }

    void Start()
    {
        // Salva a velocidade inicial caso você tenha alterado no Inspector
        if (audioSource != null) originalPitch = audioSource.pitch;
    }

    // NOVA FUNÇÃO: Recebe o "CD" novo, reseta a velocidade e dá o Play
    public void PlayNewTrack(AudioClip newClip)
    {
        if (audioSource == null) return;

        // 1. Reseta a variável interna matemática
        increased = 1f; 

        audioSource.Stop();
        audioSource.clip = newClip;
        audioSource.pitch = originalPitch; // 2. Reseta o player de áudio
        
        // 3. Reseta o Mixer para tirar o efeito da wave anterior
        if (audioSource.outputAudioMixerGroup != null)
        {
            audioSource.outputAudioMixerGroup.audioMixer.SetFloat(PitchParameter, 1f);
        }

        if (newClip != null) // Só dá play se tiver música nova configurada
        {
            audioSource.Play();
        }
    }

}

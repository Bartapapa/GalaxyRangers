using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioClip[] playlist;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup soundEffectMixer;
    private GameObject[] _soundAlreadyExist;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple AudioManagers found in game! Removing latest one.");
            Destroy(this.gameObject);
        }
        audioSource.clip = playlist[0];
        audioSource.Play();
        // audioMixer.SetFloat("MusicMixer", -30);
        // audioMixer.SetFloat("MainVolume", -80);
        // audioMixer.SetFloat("SoundMixer", 0);
    }

    // utiliser dans un autre script

    public void MyFunction()
    {
        // Fade d'un son vers l'autre
    }

    private void Update()
    {
        if (audioSource.isPlaying == false) {
            audioSource.clip = playlist[0];
            audioSource.Play();
        }
    }

    public AudioSource PlayClipAt(AudioClip clip, Vector3 pos)
    {
        _soundAlreadyExist = GameObject.FindGameObjectsWithTag("TempAudioTag");
        for (int i = 0; i < _soundAlreadyExist.Length; i++)
            Destroy(_soundAlreadyExist[i]);

        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.position = pos;
        tempGO.gameObject.tag="TempAudioTag";
        AudioSource audioSource = tempGO.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.outputAudioMixerGroup = soundEffectMixer;
        audioSource.Play();
        if (tempGO == true)
            Destroy(tempGO, clip.length);
        return audioSource;
    }
}

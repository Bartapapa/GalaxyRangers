using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioClip[] playlist;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioMixerGroup musicMixer;
    [SerializeField] private AudioMixerGroup soundEffectMixer;
    private bool _isFaiding = false;
    private float _speedOfFaiding = 0.25f;

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
        DontDestroyOnLoad(this.gameObject);
        audioSource.clip = playlist[0];
        audioSource.outputAudioMixerGroup = musicMixer;
        audioSource.Play();
        // audioMixer.SetFloat("MusicMixer", -30);
        // audioMixer.SetFloat("MainVolume", -80);
        // audioMixer.SetFloat("SoundMixer", 0);
    }

    // utiliser dans un autre script

    public void MyMusicRun()
    {
        // Fade d'un son vers l'autre
        // _isFaiding = true;

        if (playlist.Length > 1)
        {
            audioSource.clip = playlist[1];
            audioSource.Play();
        }
    }

    private void Update()
    {
        /*if (audioSource.isPlaying == false) {
            audioSource.clip = playlist[0];
            audioSource.Play();
        }*/
        // if (_isFaiding)
        // {
            // musicMixer.SetFloat("MusicMixer", Mathf.Lerp(0, -80, _speedOfFaiding));
        // }
    }

    public AudioSource PlayClipAt(AudioClip clip, Vector3 pos)
    {
        // _soundAlreadyExist = GameObject.FindGameObjectsWithTag("TempAudioTag");

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

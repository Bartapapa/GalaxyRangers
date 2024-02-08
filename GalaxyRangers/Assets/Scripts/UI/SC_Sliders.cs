using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SC_Sliders : MonoBehaviour
{
    [SerializeField]
    private AudioMixer audioMixer;
    [SerializeField]
    private Slider MainSlider;
    [SerializeField]
    private Slider musicSlider;
    [SerializeField]
    private Slider soundSlider;

    private void Start()
    {
        audioMixer.GetFloat("MainVolume", out float MainValueSlider);
        MainSlider.value = MainValueSlider;

        audioMixer.GetFloat("SoundMixer", out float soundValueSlider);
        soundSlider.value = soundValueSlider;

        audioMixer.GetFloat("MusicMixer", out float musicValueSlider);
        musicSlider.value = musicValueSlider;
    }

    public void SetVolume(float Volume)
    {
        audioMixer.SetFloat("MainVolume", Volume);
    }
    public void SetVolumeMusic(float Volume)
    {
        audioMixer.SetFloat("MusicMixer", Volume);
    }
    public void SetVolumeSound(float Volume)
    {
        audioMixer.SetFloat("SoundMixer", Volume);
    }
}

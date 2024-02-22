using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Audio : MonoBehaviour
{
    [SerializeField]
    private AudioClip sound;

    public void PlaySound()
    {
        AudioManager.Instance.PlayClipAt(sound, this.transform.position);
    }

    public void PlayMusic()
    {
        AudioManager.Instance.MyMusicRun();
    }
}

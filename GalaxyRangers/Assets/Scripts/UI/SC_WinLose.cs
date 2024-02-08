using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_WinLose : MonoBehaviour
{
    // EndGame
    [SerializeField]
    private GameObject UIgameOver;
    [SerializeField]
    private GameObject victoryScreen;

    // [SerializeField]
    // private AudioClip audioClipWinning = null;


    public void GameOverScreen()
    {
        UIgameOver.SetActive(true);
    }

    public void VictoryScreen()
    {
        // GameObject.FindGameObjectWithTag("AudioManager").GetComponent<SC_AudioManager>().PlayClipAt(audioClipWinning, this.transform.position);
        victoryScreen.SetActive(true);
    }
}

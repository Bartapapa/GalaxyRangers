using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

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
    }

    // à utiliser dans un autre script
    // AudioManager.Instance.MyFunction();
    public void MyFunction()
    {
        // Fade d'un son vers l'autre
    }
}

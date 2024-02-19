using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SC_ButtonAnimSound : MonoBehaviour
{

    [SerializeField]
    private Animator animatorButton;
    [SerializeField]
    private bool _isButChangerScene = true;

    private bool _isHovered = false;
    [SerializeField]
    private AudioClip _soundClick = null;
    [SerializeField]
    private AudioClip _soundHovered = null;


    public void ClickOnBut()
    {
        if (_isButChangerScene)
            animatorButton.SetTrigger("Trigger_Click");


        // Jouer le sound du Click --> enlever le commentaire


        if (_soundClick == null)
        {
            Debug.Log("Pas de son drag and drop dans le bouton: " + gameObject.name);
            return;
        }

        //if (GameObject.FindGameObjectWithTag("AudioManager") != null) ;
        //GameObject.FindGameObjectWithTag("AudioManager").GetComponent<SC_AudioManager>().PlayClipAt(_soundClick, this.transform.position);
        //else
        //    Debug.Log("Pas de AudioManager dans la scene");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_isHovered == false)
        {
            animatorButton.SetTrigger("Trigger_Hovered");
            _isHovered = !_isHovered;


            // Jouer le sound du Hovered--> enlever le commentaire


            if (_soundHovered == null)
            {
                Debug.LogWarning("Pas de son drag and drop dans le bouton: " + gameObject.name);
                return;
            }
            else
            {
                // if (GameObject.FindGameObjectWithTag("AudioManager") != null) ;
                // GameObject.FindGameObjectWithTag("AudioManager").GetComponent<SC_AudioManager>().PlayClipAt(_soundHovered, this.transform.position);
                // else
                //    Debug.Log("Pas de AudioManager dans la scene");
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_isHovered)
        {
            animatorButton.SetTrigger("Trigger_NoHovered");
            _isHovered = !_isHovered;
        }

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossIllusion : MonoBehaviour
{
    public AIState_Boss_AttackDisplacement illusionDisplacementState;
    [Space]

    public CharacterHealth trueBossHealth;
    public AIB_Boss bossAI;
    public AIState idleState { get { return bossAI.defaultState; } }
    [Space]
    public BCC_Boss controller;
    public Transform mesh;
    public Renderer rend;
    public GameObject healthBar;
    [Space]
    public float apparateDuration = 1f;
    public float apparateIntensity = 10f;

    private Coroutine apparateCoroutine;

    private void OnEnable()
    {
        trueBossHealth.CharacterDied -= OnBossDie;
        trueBossHealth.CharacterDied += OnBossDie;
    }

    private void OnDisable()
    {
        trueBossHealth.CharacterDied -= OnBossDie;
    }

    public void Apparate()
    {
        //Make renderers appear
        rend.enabled = true;
        //Make healthbar appear
        healthBar.SetActive(true);
        //Reuse hitflash code

        if (apparateCoroutine != null)
        {
            StopCoroutine(apparateCoroutine);
        }
        apparateCoroutine = StartCoroutine(CoApparate(true));
    }

    public void Disapparate()
    {
        if (apparateCoroutine != null)
        {
            StopCoroutine(apparateCoroutine);
        }
        apparateCoroutine = StartCoroutine(CoApparate(false));
    }

    private IEnumerator CoApparate(bool appear)
    {
        float timer = 0f;
        Renderer[] rends = mesh.GetComponentsInChildren<Renderer>();

        while (timer < apparateDuration)
        {
            if (appear)
            {
                foreach(Renderer rend in rends)
                {
                    foreach (Material mat in rend.materials)
                    {
                        float flashLerp = Mathf.Lerp(apparateIntensity, 1f, timer / apparateDuration);
                        //mat.color = Color.white * flashLerp;
                        mat.SetFloat("_Diffuse_Color_Intensity", flashLerp);
                    }
                }
            }
            else
            {
                foreach(Renderer rend in rends)
                {
                    foreach (Material mat in rend.materials)
                    {
                        float flashLerp = Mathf.Lerp(1f, apparateIntensity, timer / apparateDuration);
                        //mat.color = Color.white * flashLerp;
                        mat.SetFloat("_Diffuse_Color_Intensity", flashLerp);
                    }
                }
            }


            timer += Time.deltaTime;
            yield return null;
        }

        if (appear)
        {
            foreach(Renderer rend in rends)
            {
                foreach (Material mat in rend.materials)
                {
                    //mat.color = Color.white;
                    mat.SetFloat("_Diffuse_Color_Intensity", 1f);
                }
            }

        }
        else
        {
            foreach (Renderer rend in rends)
            {
                foreach (Material mat in rend.materials)
                {
                    //mat.color = Color.white;
                    mat.SetFloat("_Diffuse_Color_Intensity", apparateIntensity);
                }
            }

        }

        if (!appear)
        {
            OnApparateFlashEnd();
        }
        apparateCoroutine = null;
    }

    private void OnApparateFlashEnd()
    {
        //Make renderers disappear
        rend.enabled = false;
        //Make healthbar disappear 
        healthBar.SetActive(false);
    }

    private void OnBossDie(CharacterHealth characterHealth)
    {
        controller.characterHealth.Hurt(999f);
        //Disapparate
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillerBox : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;

    [Header("Audio")]
    [SerializeField] private AudioSource sfxPlayer;
    [SerializeField] private AudioClip stashVictimClip;

    [Header("Transforms")]
    [SerializeField] private Transform trapPosition;
    [SerializeField] private Transform freePosition;
    [HideInInspector] public Victim trappedVictim;

    public bool TrapVictim(Victim victim)
    {
        if (trappedVictim == null){
            animator.SetBool("Full", true);
            sfxPlayer.PlayOneShot(stashVictimClip);

            trappedVictim = victim;
            trappedVictim.transform.SetParent(trapPosition);
            trappedVictim.transform.position = trapPosition.position;
            //trappedVictim.collider_Component.enabled = false;
            trappedVictim.gameObject.SetActive(false);
            trappedVictim.isTrash = true;
            return true;
        }
        else return false;
    }

    public void FreeVictim()
    {
        if(trappedVictim != null){
            animator.SetBool("Full", false);
            trappedVictim.transform.position = freePosition.position;
            trappedVictim.transform.SetParent(null);
            //trappedVictim.collider_Component.enabled = true;
            trappedVictim.gameObject.SetActive(true);
            trappedVictim.isTrash = false;

            trappedVictim = null;
        }
    }
}
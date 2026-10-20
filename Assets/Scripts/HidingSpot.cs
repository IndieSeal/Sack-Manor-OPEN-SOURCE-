using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingSpot : MonoBehaviour, IDamageable
{
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip onDestroySFX;
    private bool repeatAudio = true;

    [Header("Health & Victim")]
    [SerializeField] private Transform dropOffLocation;
    [SerializeField] private float objectHealth = 1;
    [Space]
    [SerializeField] private float dropDelay = 0;
    public Victim victim;
    private float health;

    public const float MAX_DISTANCE_FROM_KILLER = 5;
    public const float MAX_EVIL_DISTANCE_FROM_KILLER = 2;

    private void Start(){
        HidingSpotManager.instance.Add(this);
        Thunder.instance.OnThunder += OnThunderStart;
    }

    private void Update()
    {
        Transform killer = FindAnyObjectByType<Killer>().transform;
        if(victim != null && Mathf.Abs(Vector2.Distance(transform.position, killer.position)) < MAX_EVIL_DISTANCE_FROM_KILLER){
            if (victim.IsEvil()){
                FreeVictim();
            }
        }
    }

    private void OnHidingDestroyed(){
        if(animator != null && victim != null) animator.SetTrigger("Scared");
        else animator.SetTrigger("ScaredAlone");
    }

    public void HideVictim(Victim victim)
    {
        repeatAudio = true;
        animator.SetTrigger("Default");
        gameObject.SetActive(true);

        this.victim = victim;
        victim.transform.SetParent(transform);
        victim.transform.localPosition = Vector3.zero;
        victim.gameObject.SetActive(false);
        health = objectHealth;
    }

    public void FreeVictim()
    {
        victim.transform.position = dropOffLocation.position;
        victim.gameObject.SetActive(true);
        victim.transform.SetParent(null);

        victim = null;

        OnHidingDestroyed();
    }

    public void OnHit(){
        health--;
        if(health <= 0){
            if(health < 0){
                StartCoroutine(DropVictim(victim));
                victim = null;
            }

            OnHidingDestroyed();
        }
    }

    private IEnumerator DropVictim(Victim prevVictim)
    {
        if (prevVictim != null)
        {
            if (onDestroySFX != null && source != null && repeatAudio) { source.PlayOneShot(onDestroySFX); }
            repeatAudio = false;

            if (dropDelay != 0) { yield return new WaitForSeconds(dropDelay); }
            if (prevVictim != null)
            {
                prevVictim.transform.position = dropOffLocation.position;
                prevVictim.gameObject.SetActive(true);
                prevVictim.transform.SetParent(null);
            }

            HidingSpotManager.instance.Remove();
        }
    }

    private void OnThunderStart()
    {
        if (victim == null) return;

        Transform killerTransform = FindAnyObjectByType<Killer>().transform;
        float distanceFromKiller = Vector2.Distance(transform.position, killerTransform.position);

        if (distanceFromKiller < MAX_DISTANCE_FROM_KILLER){
            animator.SetTrigger("ScaredAlone");

            victim.transform.position = dropOffLocation.position;
            victim.gameObject.SetActive(true);
            victim.transform.SetParent(null);

            victim.Escape();
            victim = null;
        }
    }
}
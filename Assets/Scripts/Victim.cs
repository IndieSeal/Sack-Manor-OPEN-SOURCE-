using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class Victim : MonoBehaviour, IDamageable
{
    [Header("Components")]
    public Collider2D collider_Component;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spr;
    [SerializeField] private AIDestinationSetter destinationSetter;
    [SerializeField] private AIPath aiPath;
    [SerializeField] private CollisionDetector detector;

    [Header("Audio")]
    [SerializeField] private AudioSource sfxPlayer;
    [SerializeField] private AudioClip hurtClip;

    [Header("Health")]
    [SerializeField] private float maximumHealth = 1;
    private float health;

    [Header("Weak")]
    [SerializeField] private float weakDelay = 20;
    [HideInInspector] public bool isWeak;
    private float weakTimer;

    [Header("Container")]
    [SerializeField] private float deathDelay = 20;
    [HideInInspector] public float deathTimer;

    [Header("Evil Child")]
    [SerializeField] private bool isEvil;
    [Space, Header("the following variables will only work if [isEvil] is set to true")]
    [SerializeField] private float evilMovementSpeed = 1;
    [SerializeField] private int evilHealth = 2;
    [SerializeField] private float evilAttackDelay = 4;
    private float attackTimer;

    private void Reset()
    {
        collider_Component = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        detector.OnCollisionStart += Detector_OnCollisionStart;
    }

    private void OnDisable()
    {
        detector.OnCollisionStart -= Detector_OnCollisionStart;
    }

    private void Detector_OnCollisionStart(GameObject obj)
    {
        if (obj.TryGetComponent(out Killer killer))
        {
            killer.Damage();
            detector.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        deathTimer = deathDelay;
        health = maximumHealth;

        attackTimer = Random.Range(0, evilAttackDelay);
    }

    private void Update()
    {
        if (isWeak) { destinationSetter.target = null; }
        if (isEvil && !isBagged && !isWeak){
            anim.SetBool("IsEvil", true);

            spr.flipX = false;

            Transform killer = FindAnyObjectByType<Killer>().transform;
            aiPath.maxSpeed = evilMovementSpeed;
            destinationSetter.target = killer;

            //flip
            bool positive = killer.position.x > transform.position.x;
            Vector2 absoluteScale = new(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            if (positive) transform.localScale = new(absoluteScale.x * -1, absoluteScale.y);
            else transform.localScale = new(absoluteScale.x, absoluteScale.y);

            attackTimer -= Time.deltaTime;
            if(attackTimer <= 0)
            {
                anim.SetTrigger("Attack");
                attackTimer = evilAttackDelay;
            }
        }

        if (isBagged && killer != null){
            Vector2 absoluteScale = new(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            transform.localScale = absoluteScale;
        }
    }

    public void OnHit()
    {
        health--;
        if(health <= 0){
            destinationSetter.target = null;
            spr.material = Killer.instance.paletteMaterial;
            collider_Component.enabled = true;

            weakTimer = weakDelay;

            sfxPlayer.PlayOneShot(hurtClip);
            anim.SetTrigger("Weak");
            isWeak = true;
        }
    }

    public void Evil()
    {
        health = evilHealth;
        isEvil = true;
    }

    public bool IsEvil()
    {
        return isEvil;
    }

    bool isBagged;
    Transform killer;
    public void OnBagged(Transform killer)
    {
        this.killer = killer;
        isBagged = true;
        anim.SetTrigger("Bag");
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x),
            Mathf.Abs(transform.localScale.y), Mathf.Abs(transform.localScale.z));
    }
    public void OnUnBagged()
    {
        killer = null;
        isBagged = false;
        spr.flipX = false;
        anim.SetTrigger("Weak");
    }

    public void Escape()
    {
        if (!isEvil)
        {
            health = maximumHealth;
            isWeak = false;
            StartCoroutine(Run());
        }
    }

    private IEnumerator Run()
    {
        while (Killer.instance.paletteMaterial.GetFloat("_HitEffectBlend") != 1) yield return null;

        Material copyOfMat = new(Killer.instance.paletteMaterial);
        copyOfMat.SetFloat("_HitEffectBlend", 0);
        spr.material = copyOfMat;

        //Run to another room
        List<Door> doors = FindObjectsByType<Door>(FindObjectsSortMode.None).ToList();
        Transform closestObject = null;
        for (int i = 0; i < doors.Count; i++)
        {
            if(closestObject == null) closestObject = doors[i].transform;
            else{
                float thisObjectDistance = Mathf.Abs(Vector2.Distance(transform.position, doors[i].transform.position));
                float closestObjectDistance = Mathf.Abs(Vector2.Distance(transform.position, closestObject.position));
                if (thisObjectDistance < closestObjectDistance) closestObject = doors[i].transform;
            }
        }

        destinationSetter.target = closestObject;

        while (Killer.instance.paletteMaterial.GetFloat("_HitEffectBlend") == 1) yield return null;

        spr.material = Killer.instance.paletteMaterial;
    }

    public void OnDoorUsage(Door door)
    {
        if (isWeak) return;

        if (!isEvil)
        {
            health = maximumHealth;
            destinationSetter.target = null;
            transform.position = door.nextDoor.mySpawnPoint.position;

            //Find further hiding spot
            List<HidingSpot> spots = FindObjectsOfType<HidingSpot>(true).ToList();
            HidingSpot longestDistance = null;
            for (int i = 0; i < spots.Count; i++)
            {
                HidingSpot currentSpot = spots[i];
                if (currentSpot.victim != null) continue;

                if (longestDistance == null) { longestDistance = currentSpot; }
                else
                {
                    float distanceFromSpot = Mathf.Abs(Vector2.Distance(transform.position, currentSpot.transform.position));
                    float distanceFromLongestSpot = Mathf.Abs(Vector2.Distance(transform.position, longestDistance.transform.position));
                    if (distanceFromSpot < distanceFromLongestSpot) { longestDistance = currentSpot; }
                }
            }

            longestDistance.HideVictim(this);
        }
        else{
            transform.position = door.nextDoor.mySpawnPoint.position;
        }
    }

    public bool isTrash;
}
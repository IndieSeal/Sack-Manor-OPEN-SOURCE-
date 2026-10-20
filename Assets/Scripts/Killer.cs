using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Killer : MonoBehaviour
{
    [Header("Components")]
    public Material paletteMaterial;
    public float defaultBrightness = 1;
    [Space]
    public AudioSource sfxPlayer;
    [SerializeField] private Animator anim;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 5;
    private float currentSpeed;
    [SerializeField] private Rigidbody2D rb;
    private Vector2 playerInput;

    [Header("Attacking")]
    [SerializeField] private KeyCode attackKey = KeyCode.Z;
    [SerializeField] private float attackDelay = 0.5f;
    [Space]
    [SerializeField] private AudioClip hitVictimClip;
    [Space]
    [SerializeField] private CollisionDetector attackCollisionDetector;
    private List<GameObject> damageables = new();
    private float attackTimer;

    [Header("Drag Corpse")]
    [SerializeField] private KeyCode dragKey = KeyCode.X;
    [SerializeField] private Transform dragPosition;
    private Victim victimBeingDragged;
    private List<Victim> victims = new();
    private KillerBox killerBox;

    public static Killer instance;

    private float minDistanceForThunder = 4;
    public bool canThunder;

    [Header("Health")]
    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip deathClip;
    [Space]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float invizFrames = 0.3f;
    [Space(30)]
    [SerializeField] private GameObject heartUIPrefab;
    [SerializeField] private Transform heartParent;
    private float invizTimer;
    private int currentHealth;

    private void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private List<GameObject> hearts = new();
    private void Awake()
    {
        instance = this;

        attackCollisionDetector.OnCollisionStart += OnCollisionStart;
        attackCollisionDetector.OnCollisionStop += OnCollisionStop;
        //paletteMaterial.SetFloat("_HsvBright", defaultBrightness);
        paletteMaterial.SetFloat("_HitEffectBlend", defaultBrightness);

        currentHealth = maxHealth;
        for (int i = 0; i < currentHealth; i++)
        {
            hearts.Add(Instantiate(heartUIPrefab, heartParent));
        }

        ResetSpeed();
    }

    private void Update()
    {
        if (!dead)
        {
            invizTimer -= Time.deltaTime;

            canThunder = true;
            List<Door> doors = FindObjectsByType<Door>(FindObjectsSortMode.None).ToList();
            for (int i = 0; i < doors.Count; i++)
            {
                if (Mathf.Abs(Vector2.Distance(transform.position, doors[i].transform.position)) < minDistanceForThunder)
                {
                    canThunder = false;
                }
            }

            SetInput();

            //Movement
            Move();
            Flip();

            //Attacking
            AttackManager();

            //Dragging
            DragCorpse();
        }
    }

    bool dead;
    public void Damage()
    {
        if (currentHealth <= 0 || invizTimer > 0) return;
        invizTimer = invizFrames;
        currentHealth--;
        hearts[currentHealth].SetActive(false);
        CameraShake.instance.MediumShake();
        sfxPlayer.PlayOneShot(hurtClip);
        Debug.Log("damaged");

        if (currentHealth <= 0)
        {
            sfxPlayer.PlayOneShot(deathClip);
            anim.SetBool("Death", true);

            dead = true;
            Debug.Log("Health died");

            DeathScreen();
        }
    }

    public void DeleteHearts()
    {
        for (int i = 0;i < hearts.Count;i++)
        {
            hearts[i].gameObject.SetActive(false);
        }
    }

    private void DeathScreen()
    {
        Debug.Log("Called Lose Scene");
        StartCoroutine(NightManager.instance.Lose(true));
    }

    private void DragCorpse()
    {
        if(victimBeingDragged != null) victimBeingDragged.transform.position = dragPosition.position;

        bool dropped = false;
        if (Input.GetKeyDown(dragKey) && victimBeingDragged != null)
        {
            dropped = true;
            if(killerBox != null){
                if (killerBox.TrapVictim(victimBeingDragged)){
                    victimBeingDragged.transform.SetParent(null);
                    victimBeingDragged.collider_Component.enabled = true;

                    victimBeingDragged.OnUnBagged();
                    victimBeingDragged = null;
                    ResetSpeed();
                }
                return;
            }
            else{
                victimBeingDragged.transform.SetParent(null);
                victimBeingDragged.collider_Component.enabled = true;

                victimBeingDragged.OnUnBagged();
                victimBeingDragged = null;
                ResetSpeed();
            }
        }
        if (!dropped && Input.GetKeyDown(dragKey) && victimBeingDragged == null && victims.Count > 0) {
            for (int i = 0; i < victims.Count; i++){
                if (victims[i].isWeak){
                    SpeedMultiply(0.7f);

                    victimBeingDragged = victims[i];
                    victimBeingDragged.collider_Component.enabled = false;

                    victimBeingDragged.OnBagged(transform);
                    victimBeingDragged.transform.SetParent(transform);
                    victimBeingDragged.transform.position = dragPosition.position;
                    return;
                }
            }
        }
    }

    private void SetInput(){
        playerInput = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void Move(){
        anim.SetBool("isWalking", false);
        if (playerInput == Vector2.zero) return;
        if(playerInput.x != 0) playerInput.y = 0;

        rb.MovePosition(rb.position + (currentSpeed * Time.deltaTime * playerInput.normalized));
        anim.SetBool("isWalking", true);
    }

    private void Flip(){
        if (currentSpeed != 0){
            if (playerInput.x > 0 && transform.localScale.x < 0 || playerInput.x < 0 && transform.localScale.x > 0){
                transform.localScale = new(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            }
        }
    }

    private void AttackManager(){
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0 && Input.GetKeyDown(attackKey)) Attack();
    }

    private void OnCollisionStart(GameObject collision){
        if(collision.TryGetComponent(out IDamageable damageable)) damageables.Add(collision);
        if (collision.TryGetComponent(out Victim victim)) victims.Add(victim);
        if(collision.TryGetComponent(out KillerBox killerBox) && killerBox.trappedVictim == null) this.killerBox = killerBox;
    }
    private void OnCollisionStop(GameObject collision){
        if (collision.TryGetComponent(out IDamageable damageable)) damageables.Remove(collision);
        if (collision.TryGetComponent(out Victim victim)) victims.Remove(victim);
        if (collision.TryGetComponent(out KillerBox killerBox) && killerBox.trappedVictim == null) this.killerBox = null;
    }

    private void Attack(){
        anim.SetTrigger("Attack");
        attackTimer = attackDelay;
        CameraShake.instance.SmallestShake();

        if (damageables.Count == 0){
            return;
        }

        sfxPlayer.PlayOneShot(hitVictimClip);
        CameraShake.instance.SmallShake();

        for (int i = 0; i < damageables.Count; i++){
            damageables[i].GetComponent<IDamageable>().OnHit();
        }
    }

    public void ResetSpeed()
    {
        currentSpeed = movementSpeed;
    }

    public void SpeedMultiply(float toMultiply)
    {
        currentSpeed *= toMultiply;
    }

    public void OnDoorUsage(Door door)
    {
        currentSpeed = 0;
        StartCoroutine(DoorTeleport(door));
    }

    public IEnumerator DoorTeleport(Door door)
    {
        paletteMaterial.SetFloat("_HitEffectBlend", 1);

        door.OnInteract(transform);
        yield return new WaitForSeconds(0.25f);

        paletteMaterial.SetFloat("_HitEffectBlend", 0);

        ResetSpeed();
    }
}
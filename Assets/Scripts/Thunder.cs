using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunder : MonoBehaviour
{
    public static Thunder instance;

    public event Action OnThunder;

    [SerializeField] private float minTimeForThunder = 10;
    [SerializeField] private float maxTimeForThunder = 20;
    private float timer;

    [Header("Visuals & Sound")]
    [SerializeField] private SpriteRenderer thunderSprite;
    [Space]
    [SerializeField] private AudioSource sfxPlayer;
    [SerializeField] private AudioClip thunderClip;
    [Space]
    [SerializeField] private float darknessDuration = 3;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        RandomizeTime();
    }

    private void LateUpdate()
    {
        timer -= Time.deltaTime;
        if(timer <= 0 && Killer.instance.canThunder)
        {
            OnThunder?.Invoke();
            RandomizeTime();

            //VISUAL ELEMENTS PLEASE
            sfxPlayer.PlayOneShot(thunderClip);
            StartCoroutine(BlackScreen(darknessDuration));
        }
    }

    private void RandomizeTime()
    {
        timer = UnityEngine.Random.Range(minTimeForThunder, maxTimeForThunder);
    }

    public IEnumerator BlackScreen(float darknessDuration)
    {
        Material paletteMaterial = Killer.instance.paletteMaterial;

        paletteMaterial.SetFloat("_HitEffectBlend", 1);

        Material copyOfMat = new(Killer.instance.paletteMaterial);
        copyOfMat.SetFloat("_HitEffectBlend", 0);
        copyOfMat.SetFloat("_ColorChangeTolerance2", 0.8f);
        thunderSprite.material = copyOfMat;

        thunderSprite.gameObject.SetActive(true);
        yield return new WaitForSeconds(darknessDuration);
        thunderSprite.gameObject.SetActive(false);
        thunderSprite.material = Killer.instance.paletteMaterial;

        paletteMaterial.SetFloat("_HitEffectBlend", 0);
    }
}
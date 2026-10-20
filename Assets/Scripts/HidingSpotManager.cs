using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HidingSpotManager : MonoBehaviour
{
    [SerializeField] private AudioSource sfxPlayer;
    [SerializeField] private AudioClip onDestroyClip;

    public event Action OnHidingSpotDestroyed;

    private List<HidingSpot> hidingSpots = new();
    public static HidingSpotManager instance;


    private void Awake(){
        instance = this;
    }

    public void HideChildren()
    {
        //Spawn victims at hiding spots
        List<Victim> victimList = FindObjectsByType<Victim>().ToList();
        List<HidingSpot> hidingSpots = FindObjectsByType<HidingSpot>().ToList();
        for (int i = 0; i < victimList.Count; i++)
        {
            int randomHidingSpot = UnityEngine.Random.Range(0, hidingSpots.Count);
            hidingSpots[randomHidingSpot].HideVictim(victimList[i]);
            hidingSpots.Remove(hidingSpots[randomHidingSpot]);
        }
    }

    public void Add(HidingSpot hidingSpot){
        hidingSpots.Add(hidingSpot);
    }

    public void Remove(){
        StartCoroutine(PlayDestroySound());
        OnHidingSpotDestroyed?.Invoke();
    }

    private IEnumerator PlayDestroySound()
    {
        yield return new WaitForSeconds(0.1f);
        sfxPlayer.PlayOneShot(onDestroyClip);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Sound")]
    [SerializeField] private AudioSource sfxPlayer;
    [SerializeField] private AudioClip openDoorClip;

    [Header("Door Spawn")]
    public Transform mySpawnPoint;
    public Transform myCamPoint;
    public Door nextDoor;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.TryGetComponent(out Killer killer)) { killer.OnDoorUsage(this); }
        if (collision.gameObject.TryGetComponent(out Victim victim)) { victim.OnDoorUsage(this); }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.TryGetComponent(out Killer killer)) { killer.OnDoorUsage(this); }
        if (collision.gameObject.TryGetComponent(out Victim victim)) { victim.OnDoorUsage(this); }
    }

    public void OnInteract(Transform toMove)
    {
        sfxPlayer.PlayOneShot(openDoorClip);

        toMove.position = nextDoor.mySpawnPoint.position;
        Camera.main.transform.parent.position = nextDoor.myCamPoint.position;
    }
}
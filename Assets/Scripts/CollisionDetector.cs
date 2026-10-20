using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    public event Action<GameObject> OnCollisionStart;
    public event Action<GameObject> OnCollisionStay;
    public event Action<GameObject> OnCollisionStop;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionStart?.Invoke(collision.gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        OnCollisionStay?.Invoke(collision.gameObject);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        OnCollisionStop?.Invoke(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnCollisionStart?.Invoke(collision.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        OnCollisionStay?.Invoke(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        OnCollisionStop?.Invoke(collision.gameObject);
    }
}
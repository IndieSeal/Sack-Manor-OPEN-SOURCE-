using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    private float time;
    private float frequency;

    private Vector3 lastCamPosition;

    private void Awake()
    {
        instance = this;
        lastCamPosition = transform.localPosition;
    }

    public void ShakeCamera(float time, float frequency)
    {
        this.frequency = frequency;
        this.time = time;

        StartCoroutine(Shake());
    }

    private IEnumerator Shake()
    {
        while (time > 0)
        {
            time -= Time.deltaTime;
            transform.localPosition = (Vector2)lastCamPosition + (Random.insideUnitCircle * frequency);
            transform.localPosition = new(transform.localPosition.x, transform.localPosition.y, -10);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        transform.localPosition = new(0, 0, -10);
    }

    public void SmallestShake()
    {
        CameraShake.instance.ShakeCamera(0.1f, 0.15f);
    }

    public void SmallShake()
    {
        CameraShake.instance.ShakeCamera(0.1f, 0.3f);
    }

    public void MediumShake()
    {
        CameraShake.instance.ShakeCamera(0.2f, 0.5f);
    }

    public void BigShake()
    {
        CameraShake.instance.ShakeCamera(0.3f, 0.8f);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
    private void Update()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 31;
    }
}
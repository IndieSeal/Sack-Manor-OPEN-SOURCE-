using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timeskip : MonoBehaviour
{
    public float toTo = 10;
    private void Update()
    {
        if (Input.GetKey(KeyCode.Y))
        {
            Time.timeScale = toTo;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}
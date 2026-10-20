using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class VictimsLeft : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    List<Victim> list = new();

    public static VictimsLeft instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
         text.gameObject.SetActive(false);
         list = FindObjectsOfType<Victim>(true).ToList();
    }

    public void ActiveText()
    {
        text.gameObject.SetActive(true);
    }

    private void Update()
    {
        int emptyBoxes = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (!list[i].isTrash) { emptyBoxes++; }
        }

        text.text = emptyBoxes + " Left";

        if(emptyBoxes <= 0)
        {
            NightManager.instance.StartNewNight();
        }
    }
}
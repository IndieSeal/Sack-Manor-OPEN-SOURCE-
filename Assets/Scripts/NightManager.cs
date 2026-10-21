using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using System;

public class Vector3Double
{
    public double x, y, z;

    public Vector3Double(double x, double y, double z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}

[System.Serializable]
public class Night
{
    public string nightName = "Night 1";
    public string nightDescription = "I'm gonna get those damn kids";
    [Space]
    public int nightDuration = 100;
    public int badVictimAmount = 0;
    [Space]
    [Header("Make sure this value is within 0 and 360")]
    public int hueShiftValue = 0;
    
    [Space]
    public Color textColor = new(194, 239, 192, 1);
    public Color cameraColor = new(45, 26, 0, 1);
}

public class NightManager : MonoBehaviour
{
    [SerializeField] private TMP_Text loseText;

    public GameObject nightPanel;
    [SerializeField] private TMP_Text nightText;
    [SerializeField] private TMP_Text nightDescriptionText;

    [SerializeField] private List<Night> nightList = new();
    private int nightIndex;

    public Material paletteMaterial;

    [Header("Audio")]
    [SerializeField] private AudioSource audioPlayer;
    [SerializeField] private AudioClip winClip;
    [SerializeField] private AudioClip loseClip;

    [SerializeField] private bool debugMode = false;
    [SerializeField] private KeyCode debugSkipNightKey = KeyCode.P;

    public static NightManager instance;

    private void Awake()
    {
        int isFirst = FindObjectsByType<NightManager>().ToList().Count;
        if (isFirst > 1) { Destroy(gameObject); return; }

        instance = this;
        DontDestroyOnLoad(gameObject);

        if(debugMode) Debug.LogWarning("WARNING: Debug Mode is active!");
    }

    private void Start()
    {
        StartCoroutine(ManageNight());
        paletteMaterial.SetFloat("_HsvShift", nightList[nightIndex].hueShiftValue);
    }

    private void Update()
    {
        if(debugMode && Input.GetKeyDown(debugSkipNightKey)) StartNewNight();

        if (Killer.instance == null) return;

        Camera.main.backgroundColor = nightList[nightIndex].cameraColor;
        paletteMaterial.SetFloat("_HsvShift", nightList[nightIndex].hueShiftValue);

        nightText.text = nightList[nightIndex].nightName;
        nightDescriptionText.text = nightList[nightIndex].nightDescription;

        List<TMP_Text> texts = FindObjectsByType<TMP_Text>().ToList();
        for (int i = 0; i < texts.Count; i++)
        {
            texts[i].color = nightList[nightIndex].textColor;
        }
    }

    private void LateUpdate()
    {
        if (Killer.instance == null) return;
        paletteMaterial.SetFloat("_HsvShift", nightList[nightIndex].hueShiftValue);
    }

    public IEnumerator Lose(bool death)
    {
        loseText.gameObject.SetActive(true);
        if (!death) loseText.text = "Ran out of time";
        else loseText.text = "You were killed";
        
        Killer.instance.DeleteHearts();
        Killer.instance.SpeedMultiply(0);
        yield return new WaitForSeconds(1.5f);

        loseText.gameObject.SetActive(false);

        SceneManager.LoadScene("Lose");
        audioPlayer.PlayOneShot(loseClip);

        StartCoroutine(ManageNight());
    }

    public void RepeatNight()
    {
        audioPlayer.PlayOneShot(loseClip);
        SceneManager.LoadScene("Level");
        StartCoroutine(ManageNight());
    }

    public void StartNewNight()
    {
        audioPlayer.PlayOneShot(winClip);
        nightIndex++;

        if (nightIndex > nightList.Count) {
            SceneManager.LoadScene("Win");
            Destroy(gameObject);
        }
        else
        {
            paletteMaterial.SetFloat("_HsvShift", nightList[nightIndex].hueShiftValue);
            List<TMP_Text> texts = FindObjectsByType<TMP_Text>().ToList();
            for (int i = 0; i < texts.Count; i++)
            {
                texts[i].color = nightList[nightIndex].textColor;
            }

            SceneManager.LoadScene("Level");
            StartCoroutine(ManageNight());
        }
    }

    public IEnumerator ManageNight()
    {
        yield return 0;

        while(SceneManager.GetActiveScene().name != "Level") yield return null;
        List<TMP_Text> texts = FindObjectsByType<TMP_Text>().ToList();
        for (int i = 0; i < texts.Count; i++)
        {
            texts[i].color = nightList[nightIndex].textColor;
        }
        nightPanel.SetActive(true);

        yield return new WaitForSeconds(0.25f);

        GameTimer.instance.SetEndTime(nightList[nightIndex].nightDuration);

        List<Victim> victims = FindObjectsByType<Victim>(FindObjectsInactive.Include).ToList();
        for (int i = 0; i < nightList[nightIndex].badVictimAmount; i++){
            if(victims.Count != 0)
            {
                int random = UnityEngine.Random.Range(0, victims.Count);
                victims[random].Evil();
                victims.RemoveAt(random);
            }
        }

        HidingSpotManager.instance.HideChildren();
        StartCoroutine(HidePanel());
    }

    private IEnumerator HidePanel()
    {
        Killer.instance.SpeedMultiply(0);
        yield return new WaitForSeconds(0.25f);
        Killer.instance.SpeedMultiply(0);
        yield return new WaitForSeconds(0.75f);
        Killer.instance.SpeedMultiply(0);

        while (!Input.GetKeyDown(KeyCode.Z)) yield return null;
        nightPanel.SetActive(false);
        GameTimer.instance.StartTimer();
        Killer.instance.ResetSpeed();

        VictimsLeft.instance.ActiveText();
        GameTimer.instance.ActiveText();
    }
}
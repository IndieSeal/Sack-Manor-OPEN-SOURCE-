using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;

    [SerializeField] private float endTime = 100;
    private float timer;
    private bool canStart;
    public static GameTimer instance;

    private void Awake()
    {
        instance = this;

        if (timerText == null)
        {
            Debug.LogError("There's no timer text assigned, object will be destroyed");
            Destroy(gameObject);
        }
        timer = endTime;
    }

    private void Start()
    {
        timerText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!canStart) return;
        timer -= Time.deltaTime;
        timerText.text = Mathf.RoundToInt(timer).ToString();
        if(timer <= 0)
        {
            StartCoroutine(NightManager.instance.Lose(false));
        }
    }

    public void ActiveText()
    {
        timerText.gameObject.SetActive(true);
    }

    public void SetEndTime(float time)
    {
        endTime = time;
        timer = endTime;
        timerText.text = Mathf.RoundToInt(timer).ToString();
    }

    public void StartTimer()
    {
        canStart = true;
    }
}
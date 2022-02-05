using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Timer : MonoBehaviour
{
    [SerializeField] private float gameTime = 600;
    [SerializeField] private GameObject pausePanel;
    public static float timeRemaining;
    bool timerIsRunning = false;
    public TextMeshProUGUI timeText;
    public GameObject ResultPanel;
    // private PauseMenu pauseobj;


    void Start()
    {
        timeRemaining = gameTime;
        timerIsRunning = true;
        // pauseobj = FindObjectOfType<PauseMenu>();
    }
    void Update()
    {
        if (!(Time.timeScale == 0f))
        {
            if (timerIsRunning)
            {
                if (timeRemaining > 0)
                {
                    timeRemaining -= Time.deltaTime;
                    DisplayTime(timeRemaining);
                }
                else
                {
                    timeRemaining = 0;
                    Time.timeScale = 0f;
                    timerIsRunning = false;
                    // pauseobj.Pause();
                    pausePanel.SetActive(false);
                    // pauseobj.Gameispaused(true);

                    ResultPanel.SetActive(true);

                }
            }
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        timeToDisplay += 1;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}

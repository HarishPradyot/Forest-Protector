using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI, ButtonHolder;
    
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();    
            }
            else
            {
                Pause();
                
            }

        }

    }

    public void Resume(){
        pauseMenuUI.SetActive(false);
        ButtonHolder.SetActive(true);
        Time.timeScale = 1f;
        GameIsPaused = false;
        //GameObject.Find("Main Camera").GetComponent<CameraController>().enabled = true;
        
    }

    public void Pause(){
        pauseMenuUI.SetActive(true);
        ButtonHolder.SetActive(false);
        Time.timeScale = 0f;
        GameIsPaused = true;
        //GameObject.Find("Main Camera").GetComponent<CameraController>().enabled = false;
    }

    public void LoadMenu(){
        //GameObject.Find("Main Camera").GetComponent<CameraController>().enabled = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

}

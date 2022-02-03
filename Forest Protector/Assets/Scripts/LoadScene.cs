using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void PlayAgain () {
       SceneManager.LoadScene("Main Game");
   }

   public void MainMenu () {
       SceneManager.LoadScene("Main Menu");
   }

   public void NextLevel () {
       SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
   }
   public void MainGame(){
       SceneManager.LoadScene("Cut Scene 2");
       Time.timeScale = 1f;
   }
}

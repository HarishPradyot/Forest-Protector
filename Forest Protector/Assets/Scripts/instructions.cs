using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class instructions : MonoBehaviour
{
    public GameObject instructionsPanel;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0f;

    }

    // Update is called once per frame
    public void startGame(){
        Time.timeScale = 1f;
        instructionsPanel.SetActive(false);
    }
}

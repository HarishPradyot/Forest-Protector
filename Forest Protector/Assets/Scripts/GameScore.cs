using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameScore : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreTab;

    [SerializeField]
    private TextMeshProUGUI totalCoins;
    [SerializeField]
    private TextMeshProUGUI noOfPlays;
    // Start is called before the first frame update
    void Start()
    {
        PlayerMovement player = FindObjectOfType<PlayerMovement>();  
        scoreTab.text =  "Score : " + player.score;

        totalCoins.text = "Coins Collected: " + player.TotalNumberOfCoins;

        noOfPlays.text = "No Of Plays: " + player.NoOfPlays;

    }

    // Update is called once per frame
}

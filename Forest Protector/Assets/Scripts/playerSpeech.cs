using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class playerSpeech : MonoBehaviour
{
    
    [SerializeField]
    private GameObject panelImg;
    
    [SerializeField]
    private TextMeshProUGUI textBox;
    // Start is called before the first frame update

    void Start()
    {
        StartCoroutine("Speech");
    }

    IEnumerator Speech(){

        yield return new WaitForSeconds(5f);
        panelImg.SetActive(true);
        textBox.text = "Hi this is a Message";
        yield return new WaitForSeconds(5f);
        panelImg.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class playerSpeech : MonoBehaviour
{

    [SerializeField] private int maxLength = 100;
    
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
        string x= "What should i keep here?";
        string y= "i should become better!";
        Debug.Log(y.Length);
        textBox.text = "";
        for(int i=0;i<x.Length;i++){
            yield return new WaitForSeconds(0.1f);
            textBox.text = textBox.text + x[i];
        }
        yield return new WaitForSeconds(3f);
        textBox.text = "";
        for(int i=0;i<y.Length;i++){
            yield return new WaitForSeconds(0.1f);
            textBox.text = textBox.text + y[i];
        }
        yield return new WaitForSeconds(3f);
        StopCoroutine("Speech");
        panelImg.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class playerSpeech : MonoBehaviour
{

    [SerializeField] private int maxLength = 100;
    
    [SerializeField]
    private GameObject panelImg;

    List<List<string>> conversation = new List<List<string>>();
    
    [SerializeField]
    private TextMeshProUGUI textBox;
    // Start is called before the first frame update

    private bool active;
    void Start()
    {
        active = false;
        // Add all the Conversation u want and iterate through them through them...>!
        conversation.Add(new List<string>(){"This part is game intro","split into pieces based on word length","this is one"});
        conversation.Add(new List<string>(){"This part is game intro","split into pieces based on word length","This is two"});
        conversation.Add(new List<string>(){"This part is game intro","split into pieces based on word length",""});
        conversation.Add(new List<string>(){"This part is game intro","split into pieces based on word length",""});
        conversation.Add(new List<string>(){"This part is game intro","split into pieces based on word length",""});
        conversation.Add(new List<string>(){"This part is game intro","split into pieces based on word length",""});
        Debug.Log(conversation[0][0]);
        StartCoroutine(Speech(conversation[1]));
    }

    public void startConversation(int Number){
        if(active==false){
            StartCoroutine(Speech(conversation[Number]));
        }
    }
    IEnumerator Speech(List<string> conversation){

        if(active == false){
            active = true;
            yield return new WaitForSeconds(1f);
            panelImg.SetActive(true);

            foreach(string s in conversation){
                textBox.text = "";
                for(int i=0;i<s.Length;i++){
                    yield return new WaitForSeconds(0.1f);
                    textBox.text = textBox.text + s[i];
                }
                yield return new WaitForSeconds(2f);
            }
            StopCoroutine("Speech");
            panelImg.SetActive(false);
            active = false;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

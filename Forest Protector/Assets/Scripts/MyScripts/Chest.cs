using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Chest : MonoBehaviour
{
    [SerializeField]
    private float waitDuration=4f;  // Time for which chest should remain opened before closing again
    private Animator chestAnimator;
    private string OPEN_CHEST, CLOSE_CHEST;
    private EnemyRegionalSpawner spawner;
    // Start is called before the first frame update
    void Start()
    {
        chestAnimator=GetComponent<Animator>();
        OPEN_CHEST="Base Layer.Chest Open";
        CLOSE_CHEST="Base Layer.Chest Close";
        spawner=transform.parent.GetComponent<EnemyRegionalSpawner>();
    }

    public void openChest()
    {
        if(spawner.getLocalEnemy() <= 0)
        {
            chestAnimator.Play(OPEN_CHEST);
            gameObject.GetComponent<AudioSource>().Play();
            StartCoroutine(closeChest());
            spawner.ChestOpen=true;
        }
        else{
            FindObjectOfType<playerSpeech>().startConversation(2);
        }
    }
    IEnumerator closeChest()
    {
        float startTime=Time.time;
        while(Time.time-startTime<=waitDuration)
            yield return null;
        chestAnimator.Play(CLOSE_CHEST);
        
        gameObject.GetComponent<FadeInAndOut>().destroy();
    }
}

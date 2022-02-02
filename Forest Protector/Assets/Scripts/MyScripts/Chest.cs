using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Chest : MonoBehaviour
{

    public int spawnLocation;

    [SerializeField]
    public GameObject ChestSpawner;

    [SerializeField]
    private float waitDuration=4f;  // Time for which chest should remain opened before closing again
    private Animator chestAnimator;
    private string OPEN_CHEST, CLOSE_CHEST;
    // Start is called before the first frame update
    void Start()
    {
        chestAnimator=GetComponent<Animator>();
        OPEN_CHEST="Base Layer.Chest Open";
        CLOSE_CHEST="Base Layer.Chest Close";
    }

    public void openChest()
    {
        Debug.Log("Long Presss Working");
        chestAnimator.Play(OPEN_CHEST);
        StartCoroutine(closeChest());
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

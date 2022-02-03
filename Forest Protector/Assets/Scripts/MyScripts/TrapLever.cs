using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TrapLever : MonoBehaviour
{
    [HideInInspector]
    public GameObject[] enemies;
    
    [HideInInspector]
    public GameObject relatedAnimal;
    [HideInInspector]
    public GameObject relatedCage;
    [HideInInspector]
    public int spawnLocation;

    [SerializeField]
    public GameObject TrapSpawner;


    [SerializeField] private float cageOffset;

    [SerializeField]
    private float fadeSpeed = 1f;

    [SerializeField]
    private Sprite cageOpen;


    [SerializeField]
    private float waitDuration=4f;  // Time for which chest should remain opened before closing again
    private Animator trapAnimator;
    private string OPEN_TRAP, CLOSE_TRAP;
    // Start is called before the first frame update
    [HideInInspector]
    public int noOfEnemies;
    void Start()
    {
        noOfEnemies = enemies.Length;
        trapAnimator=GetComponent<Animator>();
        OPEN_TRAP="Base Layer.Trap Open";
        CLOSE_TRAP="Base Layer.Trap Close";
    }

    public void openTrap()
    {
        if(noOfEnemies==0){
            Debug.Log("Long Presss Working");
            trapAnimator.Play(OPEN_TRAP);
            relatedCage.GetComponent<SpriteRenderer>().sprite = cageOpen;
            StartCoroutine(closeTrap());
        
        }
    }
    IEnumerator closeTrap()
    {
        float startTime=Time.time;
        while(Time.time-startTime<=waitDuration)
            yield return null;
        trapAnimator.Play(CLOSE_TRAP);
        // StartCoroutine("destroyTrap");
        gameObject.GetComponent<FadeInAndOut>().destroy();
        relatedAnimal.GetComponent<FadeInAndOut>().destroy();
        relatedCage.GetComponent<FadeInAndOut>().destroy();

    }


    IEnumerator destroyTrap()
    {
        Material trap=gameObject.GetComponent<Renderer>().material;
        while(trap.color.a > 0.01f)
        {
            trap.color = new Color(trap.color.r, trap.color.g, trap.color.b, trap.color.a-fadeSpeed*Time.deltaTime);
            yield return null;
        }
        gameObject.GetComponent<FadeInAndOut>().destroy();
    }
}

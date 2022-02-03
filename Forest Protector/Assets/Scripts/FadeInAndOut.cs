using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInAndOut : MonoBehaviour
{
    [SerializeField]
    private float fadeSpeed;
    private Material specialObject;
    void Awake() {
        specialObject=gameObject.GetComponent<Renderer>().material;
        specialObject.color = new Color(specialObject.color.r, specialObject.color.g, specialObject.color.b, 0);

    }
    void OnEnable() {
        StartCoroutine("awakeGarbage");
    }
    IEnumerator destroyObject()
    {
        
        while(specialObject.color.a > 0.01f)
        {
            specialObject.color = new Color(specialObject.color.r, specialObject.color.g, specialObject.color.b, specialObject.color.a-fadeSpeed*Time.deltaTime);
            yield return null;
        }

        // if(gameObject.CompareTag("Chest")){
        //     Chest chest = gameObject.GetComponent<Chest>();
            
        //     chest.ChestSpawner.GetComponent<ChestSpawner>().resetSpawner(chest.spawnLocation);
        // }
        // if(gameObject.CompareTag("Trap")){
        //     TrapLever trap= gameObject.GetComponent<TrapLever>();
            
        //     trap.TrapSpawner.GetComponent<TrapSpawner>().resetSpawner(trap.spawnLocation);
        // }
        Destroy(gameObject);
    }
    IEnumerator awakeGarbage()
    {
        
        while(specialObject.color.a < 1f)
        {
            specialObject.color = new Color(specialObject.color.r, specialObject.color.g, specialObject.color.b, specialObject.color.a+fadeSpeed*Time.deltaTime);
            yield return null;
        }
    }

    public void destroy(){
        StartCoroutine("destroyObject");
    }
}

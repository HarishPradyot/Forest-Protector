using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject[] sprites;
    [SerializeField] private int start,end;


    private GameObject spawn;

    private bool spawned; 

    void Start()
    {
        spawned = false;
        StartCoroutine("GarbageGenerator");
    }

    IEnumerator GarbageGenerator(){
        while(true){
            float x = Random.Range(start,end);
            yield return(new WaitForSeconds(x));
            if(spawn==null)
                spawned = false;
            if(!spawned){
                spawned = true;
                yield return(new WaitForSeconds(x));
                int y = Random.Range(0,sprites.Length);
                spawn = Instantiate(sprites[y], new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
                // 
                // spawn.gameObject
            }
            
        }
    }
}

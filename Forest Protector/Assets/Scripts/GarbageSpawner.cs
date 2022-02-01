using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject[] sprites;

    void Start()
    {
        StartCoroutine("GarbageGenerator");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator GarbageGenerator(){
        while(true){
            float x = Random.Range(0,5);
            yield return(new WaitForSeconds(x));
            int y = Random.Range(0,sprites.Length);
            Instantiate(sprites[y], new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
            
        }
    }
}

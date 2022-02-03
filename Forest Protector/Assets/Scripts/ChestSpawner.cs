using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestSpawner : MonoBehaviour
{
    [SerializeField] private GameObject chestSprite;
    [SerializeField] private Transform[] spawners;
    private bool[] spawnerBool;
    private GameObject spawnedChest;
    
    private int randomSpawner;
    private int chestCount=0;
    [SerializeField] private int chestLimit=20;
    void Start()
    {
        spawnerBool=new bool[spawners.Length];
        for(int i=0;i< spawnerBool.Length;i++){
            spawnerBool[i] = false;
        }
        StartCoroutine(SpawnChest());
    }
    IEnumerator SpawnChest() 
    {
        while(chestCount<chestLimit) 
        {
            chestCount++;
            yield return new WaitForSeconds(Random.Range(3,7));

            randomSpawner = Random.Range(0, spawners.Length);
            

            if(spawnerBool[randomSpawner]==false){
                spawnedChest = Instantiate(chestSprite);
                Debug.Log("Chest Spawned " + Time.time);
                spawnedChest.transform.position = spawners[randomSpawner].position;
                spawnedChest.GetComponent<Chest>().spawnLocation = randomSpawner;
                spawnedChest.GetComponent<Chest>().ChestSpawner = gameObject;
                spawnerBool[randomSpawner] = true;
            }
            
        }
    }

    public void resetSpawner(int i){
        Debug.Log("TransformReset");
        spawnerBool[i] = false;
    }
}

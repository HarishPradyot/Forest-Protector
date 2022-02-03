using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{

    [SerializeField] private GameObject coinSprite;
    [SerializeField] private Transform[] spawners;
    private bool[] spawnerBool;
    private GameObject spawnedCoin;
    
    private int randomSpawner;
    private int coinCount=0;
    [SerializeField] private int coinLimit=20;
    void Start()
    {
        spawnerBool=new bool[spawners.Length];
        for(int i=0;i< spawnerBool.Length;i++){
            spawnerBool[i] = false;
        }
        StartCoroutine(SpawnCoin());
    }
    IEnumerator SpawnCoin() 
    {
        while(coinCount<coinLimit) 
        {
            coinCount++;
            yield return new WaitForSeconds(Random.Range(3,7));

            randomSpawner = Random.Range(0, spawners.Length);
            

            if(spawnerBool[randomSpawner]==false){
                spawnedCoin = Instantiate(coinSprite);
                Debug.Log("Coin Spawned " + Time.time);
                spawnedCoin.transform.position = spawners[randomSpawner].position;
                spawnedCoin.GetComponent<Coin>().spawnLocation = randomSpawner;
                spawnedCoin.GetComponent<Coin>().CoinSpawner = gameObject;
                spawnerBool[randomSpawner] = true;
            }
            
        }
    }

    public void resetSpawner(int i){
        Debug.Log("TransformReset");
        coinCount--;
        spawnerBool[i] = false;
    }
}

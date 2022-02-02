using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private Transform[] spawners;
    private bool[] spawnerBool;
    private int randomEnemy;
    private GameObject spawnedEnemy;
    
    private int randomSpawner;
    private int enemyCount=0;
    [SerializeField] private int enemyLimit=20;
    void Start()
    {
        spawnerBool=new bool[spawners.Length];
        for(int i=0;i< spawnerBool.Length;i++){
            spawnerBool[i] = false;
        }
        StartCoroutine(SpawnEnemies());
    }
    IEnumerator SpawnEnemies() 
    {
        while(enemyCount<enemyLimit) 
        {
            enemyCount++;
            yield return new WaitForSeconds(Random.Range(3,7));

            randomEnemy = Random.Range(0, enemies.Length);
            randomSpawner = Random.Range(0, spawners.Length);
            

            if(spawnerBool[randomSpawner]==false){
                spawnedEnemy = Instantiate(enemies[randomEnemy]);
                Debug.Log("Enemy Spawned " + Time.time);
                spawnedEnemy.transform.position = spawners[randomSpawner].position;
                spawnerBool[randomSpawner] = true;
            }
            // spawnedEnemy.GetComponent<PlayerMovement>().speed = Random.Range(4, 10);
            
        }
    }
    
}

   
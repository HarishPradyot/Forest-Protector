using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemies;
    [HideInInspector]
    public GameObject spawnedEnemy;
    public Transform[] spawners;
    [HideInInspector]
    public int randomEnemy;
    [HideInInspector]
    public int randomSpawner;
    public int randomSide;
    public int spawnerCount=0;
    public int spawnerLimit=20;
    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }
    IEnumerator SpawnEnemies() 
    {
        while(spawnerCount<spawnerLimit) 
        {
            spawnerCount++;
            yield return new WaitForSeconds(Random.Range(1,3));

            randomEnemy = Random.Range(0, enemies.Length);
            randomSpawner = Random.Range(0, spawners.Length);
            
            spawnedEnemy = Instantiate(enemies[randomEnemy]);
            spawnedEnemy.transform.position = spawners[randomSpawner].position;
            // spawnedEnemy.GetComponent<PlayerMovement>().speed = Random.Range(4, 10);
            
        }
    }
    
}

   
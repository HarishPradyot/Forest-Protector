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
    public int spawnerLimit=100;
    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }
    IEnumerator SpawnEnemies() 
    {
        while(spawnerCount<spawnerLimit) 
        {
            spawnerCount++;
            yield return new WaitForSeconds(Random.Range(1, 25));
            randomEnemy = Random.Range(0, enemies.Length);
            randomSpawner = Random.Range(0, spawners.Length);
            randomSide=Random.Range(0,4);
            spawnedEnemy = Instantiate(enemies[randomEnemy]);
            spawnedEnemy.transform.position = spawners[randomSpawner].position;
            //left
            if(randomSide==0)
            {
                 spawnedEnemy.GetComponent<Monster>().speed = -Random.Range(4, 10);
                 spawnedMonster.transform.localScale = new Vector3(-1f, 1f, 1f);
            }

        }
    }
    
}

   
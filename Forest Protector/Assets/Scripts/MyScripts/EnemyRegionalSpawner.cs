using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRegionalSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private Transform[] wayPoints; // Hotspots of the region that will be patrolled by enemies
    [SerializeField] private int enemyLimit = 5; // Max number of enemies in a specific region
    [SerializeField] private float timeBetweenSpawn = 3f;
    private int enemyIndex;

    private int enemyCount;
    void Start()
    {
        enemyCount=0;
        StartCoroutine(SpawnEnemies());
    }
    IEnumerator SpawnEnemies() 
    {
        while(GameManager.isGameRunning) 
        {
            yield return new WaitForSeconds(timeBetweenSpawn);
            if(enemyCount>=enemyLimit)
                continue; 
            enemyCount++;
            enemyIndex = Random.Range(0, enemyPrefabs.Length);
            
            GameObject spawnedEnemy = Instantiate(enemyPrefabs[enemyIndex], transform.position, Quaternion.identity, transform);
        }
    }
    public Transform randomWayPoint(ref int previousWayPoint)
    {
        int index=Random.Range(0, wayPoints.Length);
        if(index==previousWayPoint)
        {
            int il=Random.Range(0, previousWayPoint), ir=Random.Range(previousWayPoint+1, wayPoints.Length);
            if(Random.Range(0, 2)==0 || previousWayPoint==wayPoints.Length-1)
                index=il;
            else
                index=ir;
        }
        previousWayPoint=index;
        return wayPoints[index];
    }
    public Transform wayPoint(int wayPointIndex)
    {
        if(wayPointIndex<0 || wayPointIndex>wayPoints.Length)
            return randomWayPoint(ref wayPointIndex);
        return wayPoints[wayPointIndex];
    }
}

   
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRegionalSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private GameObject chestPrefab, trapPrefab, cageClose, cageOpen;
    [SerializeField] private GameObject[] animalPrefabs;

    [SerializeField] private Transform[] wayPoints; // Hotspots of the region that will be patrolled by enemies
    [SerializeField] private Transform chestPoint, trapPoint;
    [SerializeField] private int enemyLimit = 5; // Max number of enemies in a specific region
    [SerializeField] private float timeBetweenSpawn = 3f;
    private int enemyIndex;
    private bool isChestOpen, isTrapOpen, spawnable;
    public bool ChestOpen
    {
        set
        {
            isChestOpen=value;
        }
    }
    public bool TrapOpen
    {
        set
        {
            isTrapOpen=value;
        }
    }

    private int enemyCount;
    void Start()
    {
        enemyCount=0;
        isChestOpen=false;
        isTrapOpen=false;
        spawnable=true;
        SpawnChest();
        SpawnTrap();
        StartCoroutine(SpawnEnemies());
    }
    void SpawnChest()
    {
        GameObject spawnedChest = Instantiate(chestPrefab, chestPoint.position, Quaternion.identity, transform);
    }
    void SpawnTrap()
    {
        GameObject spawnedTrap = Instantiate(trapPrefab, trapPoint.position, Quaternion.identity, transform);
        GameObject spawnedAnimal= Instantiate(animalPrefabs[Random.Range(0,animalPrefabs.Length)], trapPoint.position+new Vector3(5, 0, 0), Quaternion.identity, transform);
        GameObject spawnedCage= Instantiate(cageClose, trapPoint.position+new Vector3(5, 0, 0), Quaternion.identity, transform);
        spawnedTrap.GetComponent<TrapLever>().relatedAnimal=spawnedAnimal;
        spawnedTrap.GetComponent<TrapLever>().relatedCage=spawnedCage;
    }
    IEnumerator SpawnEnemies() 
    {
        while(GameManager.isGameRunning) 
        {
            yield return new WaitForSeconds(timeBetweenSpawn);
            if(spawnable)
            {
                if(enemyCount>=enemyLimit)
                {
                    spawnable=false;
                    continue; 
                }
                enemyCount++;
                enemyIndex = Random.Range(0, enemyPrefabs.Length);
                
                GameObject spawnedEnemy = Instantiate(enemyPrefabs[enemyIndex], transform.position, Quaternion.identity, transform);
            }
            else
                spawnable=isChestOpen&&isTrapOpen;
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
    public int getLocalEnemy()
    {
        return enemyCount;
    }
    public void reduceEnemyCount()
    {
        enemyCount--;
    }
}

   
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapSpawner : MonoBehaviour
{
    [SerializeField] private GameObject trapSprite;
    [SerializeField] private GameObject[] spawnedEnemies;

    [SerializeField] private GameObject spawnedAnimal;
    [SerializeField] private GameObject spawnedCage;
    
    [SerializeField] private GameObject[] enemiesSelection;
    [SerializeField] private GameObject[] animalSelection;
    
    [SerializeField] private GameObject cageSprite;
    [SerializeField] private Transform[] spawners;
    private bool[] spawnerBool;
    private GameObject spawnedTrap;
    
    private int randomSpawner;
    private int trapCount=0;
    [SerializeField] private int trapLimit=20;
    void Start()
    {
        spawnerBool=new bool[spawners.Length];
        for(int i=0;i< spawnerBool.Length;i++){
            spawnerBool[i] = false;
        }
        StartCoroutine(SpawnTrap());
    }
    IEnumerator SpawnTrap() 
    {
        while(trapCount<trapLimit) 
        {
            trapCount++;
            yield return new WaitForSeconds(Random.Range(3,7));

            randomSpawner = Random.Range(0, spawners.Length);
            

            if(spawnerBool[randomSpawner]==false){
                spawnedTrap = Instantiate(trapSprite);
                spawnedAnimal= Instantiate(animalSelection[Random.Range(0,animalSelection.Length)]);
                spawnedCage= Instantiate(cageSprite);

                spawnedEnemies = new GameObject[2];
                spawnedEnemies[0] = Instantiate(enemiesSelection[Random.Range(0,enemiesSelection.Length)]);
                spawnedEnemies[1] = Instantiate(enemiesSelection[Random.Range(0,enemiesSelection.Length)]);
                Debug.Log("Trap Spawned " + Time.time);

                spawnedTrap.transform.position = spawners[randomSpawner].position;
                spawnedTrap.GetComponent<TrapLever>().spawnLocation = randomSpawner;
                spawnedTrap.GetComponent<TrapLever>().TrapSpawner = gameObject;
                spawnedTrap.GetComponent<TrapLever>().relatedAnimal = spawnedAnimal;
                spawnedTrap.GetComponent<TrapLever>().relatedCage = spawnedCage;

                spawnedAnimal.transform.position = spawners[randomSpawner].position;
                spawnedAnimal.transform.position += new Vector3(5f,0,0);

                spawnedCage.transform.position = spawners[randomSpawner].position;
                spawnedCage.transform.position  += new Vector3(5f,0,0);

                spawnedEnemies[0].transform.position = spawners[randomSpawner].position;
                spawnedEnemies[0].transform.position  += new Vector3(5f,3f,0);


                spawnedEnemies[1].transform.position = spawners[randomSpawner].position;
                spawnedEnemies[1].transform.position  += new Vector3(3f,1f,0);
                

                spawnerBool[randomSpawner] = true;
            }
            
        }
    }

    public void resetSpawner(int i){
        Debug.Log("TransformReset");
        spawnerBool[i] = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{

    public int spawnLocation;

    [SerializeField]
    public GameObject CoinSpawner;
    

    // Start is called before the first frame update
    [SerializeField]
    private float fadeSpeed = 1f;
    void Start()
    {
        
    }
    IEnumerator destroyCoin()
    {
        Material coin=gameObject.GetComponent<Renderer>().material;
        while(coin.color.a > 0.01f)
        {
            coin.color = new Color(coin.color.r, coin.color.g, coin.color.b, coin.color.a-fadeSpeed*Time.deltaTime);
            yield return null;
        }
        CoinSpawner.GetComponent<CoinSpawner>().resetSpawner(spawnLocation);
        Destroy(gameObject);
    }
    public void onCoinCollected()
    {
        Destroy(gameObject.GetComponent<BoxCollider2D>());
        StartCoroutine(destroyCoin());
    }
}

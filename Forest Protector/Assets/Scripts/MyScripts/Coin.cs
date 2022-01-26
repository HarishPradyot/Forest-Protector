using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private float fadeSpeed = 1f;
    void Start()
    {
        
    }
    IEnumerator destroyCoin(Material coin, float fadeSpeed)
    {
        while(coin.color.a > 0.01f)
        {
            coin.color = new Color(coin.color.r, coin.color.g, coin.color.b, coin.color.a-coin.color.a*fadeSpeed*Time.deltaTime);
            yield return null;
        }
        Destroy(gameObject);
    }
    public void onCoinCollected()
    {
        Destroy(gameObject.GetComponent<BoxCollider2D>());
        Material coin=gameObject.GetComponent<Renderer>().material;
        StartCoroutine(destroyCoin(coin, fadeSpeed));
    }
}

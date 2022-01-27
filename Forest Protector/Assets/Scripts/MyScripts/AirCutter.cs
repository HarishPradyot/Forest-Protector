using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirCutter : MonoBehaviour
{
    [SerializeField]
    //damage inflicted can be analogous to distance from source
    private float speed=20f, range=40f, maxDamage=20f, fadeSpeed = 0.5f;

    private Vector2 velocity, displacement;
    private float distance;
    private bool canInflictDamage;
    // Start is called before the first frame update
    void Start()
    {
        velocity=(Vector2)transform.right * speed;
        distance=0;
        canInflictDamage=true;
        Debug.Log(transform.rotation.eulerAngles);
    }

    // Update is called once per frame
    void Update()
    {
        displacement=velocity*Time.deltaTime;
        distance+=displacement.magnitude;
        
        if(distance>range && canInflictDamage)
        {
            canInflictDamage=false;
            StartCoroutine(fadeOff());
        }
    }
    void LateUpdate()
    {
        transform.position = (Vector2)transform.position + displacement;
    }
    IEnumerator fadeOff()
    {
        Destroy(gameObject.GetComponent<BoxCollider2D>());
        Material airCutter=gameObject.GetComponent<Renderer>().material;
        while(airCutter.color.a > 0.01f)
        {
            airCutter.color=new Color(airCutter.color.r, airCutter.color.g, airCutter.color.b, airCutter.color.a-fadeSpeed*Time.deltaTime);
            yield return null;
        }
        Destroy(gameObject);
    }
    public int getDamage()
    {
        return (int)Mathf.Clamp(distance, 1f, maxDamage);
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log(col.tag);
    }
}

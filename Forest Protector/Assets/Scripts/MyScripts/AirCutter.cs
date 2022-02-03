using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirCutter : MonoBehaviour
{
    [SerializeField]
    // Damage inflicted can be analogous to distance from source
    private float speed=20f, range=40f, maxDamage=20f, fadeSpeed = 0.5f, damageMultiplier=3f;

    private Vector2 velocity, displacement;
    private float distance;
    private bool canInflictDamage;
    private Transform launchTransform;
    public bool Damage
    {
        get
        {
            return canInflictDamage;
        }
        set
        {
            canInflictDamage=value;
            StartCoroutine(fadeOff());
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        velocity=(Vector2)transform.right * speed;
        distance=0;
        canInflictDamage=true;
    }

    // Update is called once per frame
    void Update()
    {
        displacement=velocity*Time.deltaTime;
        distance+=displacement.magnitude;
        
        if(distance>range && canInflictDamage)
            Damage=false;
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
        FindObjectOfType<PlayerMovement>().restoreWeaponCount(gameObject.tag);
        Destroy(gameObject);
    }
    public int getDamage()
    {
        return (int)Mathf.Clamp(damageMultiplier*distance, 1f, maxDamage);
    }
    public void setLaunchTransform(Transform launchTransform)
    {
        this.launchTransform=launchTransform;
    }
    public Transform getLaunchTransform()
    {
        return launchTransform;
    }
}

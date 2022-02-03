using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : MonoBehaviour
{
    [SerializeField]
    // Damage inflicted can be analogous to distance from source
    private float speed=12f, rotateSpeed=2f, range=15f, maxDamage=10f;
    private float offset;
    private string ENEMY_TAG, PLAYER_TAG;

    private Vector2 velocity, displacement;
    private Vector3 angularDisplacement;
    private float distance;
    private bool canInflictDamage;
    public bool Damage
    {
        get
        {
            return canInflictDamage;
        }
        set
        {
            canInflictDamage=value;
            Destroy(gameObject.GetComponent<CapsuleCollider2D>());
        }
    }
    public float Offset
    {
        set
        {
            offset=value;
        }
    }
    private Transform returnTransform;
    // Start is called before the first frame update
    void Start()
    {
        ENEMY_TAG="Enemy";
        PLAYER_TAG="Player";
        velocity=(Vector2)transform.right*speed;
        angularDisplacement=Vector3.zero;
        distance=0;
        canInflictDamage=true;
    }

    // Update is called once per frame
    void Update()
    {
        angularDisplacement.z=angularDisplacement.z+rotateSpeed*Time.deltaTime;
        angularDisplacement.z=angularDisplacement.z%360;
        if(canInflictDamage)
            forwardMotion();
        else
            returnMotion();
    }
    void forwardMotion()
    {
        displacement=velocity*Time.deltaTime;
        distance+=displacement.magnitude;
        if(distance>range)
        {
            Damage=false;
            transform.SetParent(returnTransform);
        }
    }
    void returnMotion()
    {
        if(returnTransform==null)
        {
            Destroy(gameObject); 
            return;
        }
        else if((transform.position-returnTransform.position).magnitude < offset)
        {
            if(returnTransform.gameObject.CompareTag(ENEMY_TAG))
                returnTransform.GetComponent<EnemyAIMovement>().restoreWeaponCount(gameObject.tag);
            else if(returnTransform.gameObject.CompareTag(PLAYER_TAG))
                returnTransform.GetComponent<PlayerMovement>().restoreWeaponCount(gameObject.tag);
            Destroy(gameObject);
        }
        velocity=(Vector2)(returnTransform.position-transform.position).normalized*speed;
        displacement=velocity*Time.deltaTime;
    }
    void LateUpdate()
    {
        transform.position=(Vector2)transform.position+displacement;
        transform.Rotate(angularDisplacement);
    }
    public int getDamage()
    {
        return (int)Mathf.Clamp(distance, 1f, maxDamage);
    }
    public void setReturnTransform(Transform returnTransform)
    {
        this.returnTransform=returnTransform;
    }
    public Transform getReturnTransform()
    {
        return returnTransform;
    }
}

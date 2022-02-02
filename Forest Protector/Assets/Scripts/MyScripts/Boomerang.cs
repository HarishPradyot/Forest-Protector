using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : MonoBehaviour
{
    [SerializeField]
    // Damage inflicted can be analogous to distance from source
    private float speed=12f, rotateSpeed=2f, range=15f, maxDamage=10f;

    private Vector2 velocity, displacement;
    private Vector3 angularDisplacement;
    private float distance;
    private bool canInflictDamage;
    private Transform returnTransform;
    // Start is called before the first frame update
    void Start()
    {
        velocity=(Vector2)transform.right*speed;
        angularDisplacement=Vector3.zero;
        distance=0;
        canInflictDamage=true;
        Debug.Log(transform.rotation.eulerAngles);
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
            canInflictDamage=false;
            transform.SetParent(returnTransform);
        }
    }
    void returnMotion()
    {
        if((transform.position-returnTransform.position).magnitude < 0.01f)
        {
            FindObjectOfType<PlayerMovement>().restoreWeaponCount(gameObject.tag);
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
}

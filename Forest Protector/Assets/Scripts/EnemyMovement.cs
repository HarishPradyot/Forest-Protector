using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float myspeed = 3f;
    Transform leftWayPoint, rightWayPoint;
    Vector3 localScale;
    bool movingright = true;
    public Rigidbody2D mybody;
    void Start()
    {
        localScale = transform.localScale;
        mybody = GetComponent<Rigidbody2D>();
        leftWayPoint = GameObject.Find("LeftWayPoint").GetComponent<Transform>();
        rightWayPoint = GameObject.Find("rightWayPoint").GetComponent<Transform>();

    }
    void Update()
    {
        if (transform.position.x > rightWayPoint.position.x)
            movingright = false;
        if (transform.position.x < leftWayPoint.position.x)
            movingright = true;

        if (movingright)
            moveRight();
        else
            moveLeft();
    }
    void moveRight()
    {
        movingright = true;
        localScale.x = 1;
        transform.localScale = localScale;
        mybody.velocity = new Vector2(localScale.x * myspeed, mybody.velocity.y);
    }
    void moveLeft()
    {
        movingright = false;
        localScale.x = -1;
        transform.localScale = localScale;
        mybody.velocity = new Vector2(localScale.x * myspeed, mybody.velocity.y);
    }


    // public float speed = 20f;
    // public Rigidbody2D myBody;
    // void Awake()
    // {
    //     myBody = GetComponent<Rigidbody2D>();
    // }

    // // Update is called once per frame
    // void FixedUpdate()
    // {
    //     myBody.velocity = new Vector2(speed, myBody.velocity.y);
    // }

}


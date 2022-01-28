using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Global Game Varibles
    [SerializeField]
    private float minAccessRange=1.5f;  // Min distance between player and Collactables so that they are accessible (Example. Chests)
    [SerializeField]
    private float longPressDuration=4f;  // Time for which key must be pressed down to open Collactables (Example. Chests)
    
    // Position and Direction Parameters
    [SerializeField]
    private int speed=10;
    private int direction;

    // Position and Direction
    [SerializeField] 
    private float angle, smoothInputMagnitude=0f, inputMagnitude=0f;
    private Vector2 inputDirection, velocity;

    // Properties
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D playerBody;
    private Animator playerAnimator;
    private BoxCollider2D playerCollider;

    // Additional Attributes
    private float offset_a_d, offset_w_s, offset_diagonal;  // Offset values for raycast so that the ray originates outside player collider for horizontal, vertical and diagonal motion
    private string WALK_PARAMETER, COIN_TAG, CHEST_TAG;

    // Collectables and Weapons
    private int points, maxPoint=4; // Coin Points
    [SerializeField]
    private GameObject AirCutter;

    public Vector2 Velocity
    {
        get
        {
            return velocity;
        }
    }
    public Vector2 Position
    {
        get
        {
            return playerBody.position;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer=GetComponent<SpriteRenderer>();
        playerBody=GetComponent<Rigidbody2D>();
        playerAnimator=GetComponent<Animator>();
        playerCollider=GetComponent<BoxCollider2D>();
        WALK_PARAMETER="Direction";
        COIN_TAG="Coin";
        CHEST_TAG="Chest";

        offset_a_d=playerCollider.size.x*transform.localScale.x/2;
        offset_w_s=playerCollider.size.y*transform.localScale.y/2;
        offset_diagonal=Mathf.Sqrt(offset_a_d*offset_a_d + offset_w_s*offset_w_s);
        Physics2D.queriesStartInColliders=false;
    }

    // Update is called once per frame
    void Update()
    {
        inputDirection=new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        inputMagnitude = inputDirection.magnitude;
        smoothInputMagnitude = Mathf.Lerp(smoothInputMagnitude, inputMagnitude, speed * Time.deltaTime);

        velocity = inputDirection *  smoothInputMagnitude * speed;
        
    }
    void FixedUpdate()
    {
        playerMovement();
    }
    void LateUpdate()
    {
        playerAnimation();
        airCutter();
        lineOfSight();
    }
    void airCutter()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            Instantiate(AirCutter, playerBody.position, Quaternion.Euler(0, 0, angle*Mathf.Rad2Deg));
        }
    }
    void lineOfSight()
    {
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        float offset=offset_diagonal;
        switch(angle)
        {
            case Mathf.PI/2     :   // W Direction
            case -Mathf.PI/2    :offset=offset_w_s;break;   // S Direction
            case 0              :   // D Direction
            case Mathf.PI       :offset=offset_a_d;break;   // A Direction
        }    
        Vector2 origin=playerBody.position+direction*offset;
        RaycastHit2D visibleObject=Physics2D.Raycast(origin, direction, minAccessRange);
        if(visibleObject.collider != null)
        {
            Debug.DrawLine(origin, visibleObject.point, Color.red);
            if(visibleObject.collider.CompareTag(CHEST_TAG))
                StartCoroutine(canOpenChest(visibleObject.collider));
        }
        else
            Debug.DrawLine(origin, origin+direction*minAccessRange, Color.green);
    }
    void playerMovement()
    {
        playerBody.MovePosition(playerBody.position + velocity * Time.deltaTime);
    }
    void playerAnimation()
    {
        direction=-1;
        if(inputDirection.magnitude == 0f)
            smoothInputMagnitude=0f;
        else
        {
            angle=Mathf.Atan2(inputDirection.y, inputDirection.x);
            switch(angle)
            {
                case Mathf.PI/2     :direction=0;break; // W Direction
                case -Mathf.PI/2    :direction=1;break; // S Direction
                case 0              :direction=2;break; // D Direction
                case Mathf.PI       :direction=3;break; // A Direction
                case Mathf.PI/4     :direction=4;break; // WD Direction
                case 3*Mathf.PI/4   :direction=5;break; // SD Direction
                case -Mathf.PI/4    :direction=6;break; // WA Direction
                case -3*Mathf.PI/4  :direction=7;break; // SA Direction
            }
        }
        playerAnimator.SetInteger(WALK_PARAMETER, direction);
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag(COIN_TAG))
        {
            points+=Random.Range(1, maxPoint+1);
            collider.GetComponent<Coin>().onCoinCollected();
        }
    }
    IEnumerator canOpenChest(Collider2D Chest)
    {
        float startTime=-1f;
        if(Input.GetKeyDown(KeyCode.Z))
        {
            startTime=Time.time;
            while(Time.time-startTime<=longPressDuration && Input.GetKey(KeyCode.Z))
                yield return null;
            if(Time.time-startTime>=longPressDuration)
                Chest.GetComponent<Chest>().openChest();
            else
                Debug.Log("Press Longer");
        }
    }
}
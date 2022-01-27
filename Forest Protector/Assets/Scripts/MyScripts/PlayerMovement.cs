using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Position and Direction Parameters
    [SerializeField]
    private int speed;
    private int direction;

    //Position and Direction
    [SerializeField] 
    private float angle, smoothInputMagnitude=0f, inputMagnitude=0f;
    private Vector2 inputDirection, velocity;

    //Properties
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D playerBody;
    private Animator playerAnimator;
    private string WALK_PARAMETER, COIN, CHEST;

    //Collectables and Weapons
    private int points, maxPoint=4; // coin points
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
        WALK_PARAMETER="Direction";
        COIN="Coin";
        CHEST="Chest";
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
        Ray2D los=new Ray2D(playerBody.position, inputDirection);
        RaycastHit2D visibleObject=Physics2D.Raycast(playerBody.position, inputDirection, 1f);
        if(visibleObject)
            Debug.Log("Something There " + visibleObject.collider.gameObject.name);
        if(visibleObject.collider.CompareTag(CHEST))
            Debug.Log("Chest Nearby");
        //RaycastHit2D visibleObject=Physics2D.Raycast(los.origin, los.direction, 1f);
        Debug.DrawRay(los.origin, los.direction, Color.red);
    }
    void playerMovement()
    {
        playerBody.MovePosition(playerBody.position + velocity * Time.deltaTime);
    }
    void playerAnimation()
    {
        
        direction=-1;
        /*
        if(inputDirection.x == inputDirection.y)
        {
            //For diagonal movement if required?
            if(inputDirection.x > 0)
            {
                if(inputDirection.y > 0)  //WD Direction
                    direction=4;
                else                    //SD Direction
                    direction=5;
            }
            if(inputDirection.x < 0)
            {
                if(inputDirection.y > 0)  //WA Direction
                    direction=6;
                else                    //SA Direction
                    direction=7;
            }
        }
        else if(inputDirection.y > 0)  //W Direction
            direction=0;
        else if(inputDirection.y < 0)  //S Direction
            direction=1;
        else if(inputDirection.x > 0)  //D Direction
            direction=2;
        else if(inputDirection.x < 0)  //A Direction
            direction=3;
        */


        if(inputDirection.magnitude == 0f)
            smoothInputMagnitude=0f;
        else
        {
            angle=Mathf.Atan2(inputDirection.y, inputDirection.x);
            switch(angle)
            {
                case Mathf.PI/2     :direction=0;break;
                case -Mathf.PI/2    :direction=1;break;
                case 0              :direction=2;break;
                case Mathf.PI       :direction=3;break;
                case Mathf.PI/4     :direction=4;break;
                case 3*Mathf.PI/4   :direction=5;break;
                case -Mathf.PI/4    :direction=6;break;
                case -3*Mathf.PI/4  :direction=7;break;
            }
        }
        playerAnimator.SetInteger(WALK_PARAMETER, direction);
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag(COIN))
        {
            points+=Random.Range(1, maxPoint+1);
            collider.GetComponent<Coin>().onCoinCollected();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

    private int NoOfPlays;
    [SerializeField]
    private float fadeSpeed = 1f;
    public int maxHealth=100;
    [SerializeField]
    public int currentHealth;
    public Slider healthBar;

    [SerializeField]
    private TextMeshProUGUI health;
    private bool isPlayerDead;
    private Vector3 startPosition;

    // Global Game Varibles
    [SerializeField]
    private float minAccessRange=1.5f;  // Min distance between player and Collactables so that they are accessible (Example. Chests)
    [SerializeField]
    private float longPressDuration=4f;  // Time for which key must be pressed down to open Collactables (Example. Chests)
    [SerializeField]
    private Dictionary<string, int> Weapon_to_Index;

    [SerializeField]
    private TextMeshProUGUI coinText;


    private int numberOfCoins;
    // Position and Direction Parameters
    [SerializeField]
    private int speed=14;
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
    private string WALK_PARAMETER, COIN_TAG, CHEST_TAG, TRAP_TAG, BOOMERANG_TAG, AIRCUTTER_TAG, GARBAGE_TAG;
    
    // Collectables and Weapons
    private int points, maxPoint=4; // Coin Points
    [SerializeField]
    private GameObject[] Weapons;
    [SerializeField]
    private int[] weaponMaxCount;
    private int weaponIndex=1;

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
    
    void Awake()
    {
        velocity=Vector2.zero;
    }
    // Start is called before the first frame update
    void Start()
    {
        NoOfPlays=1;
        
        spriteRenderer=GetComponent<SpriteRenderer>();
        playerBody=GetComponent<Rigidbody2D>();
        playerAnimator=GetComponent<Animator>();
        playerCollider=GetComponent<BoxCollider2D>();
        WALK_PARAMETER="Direction";
        COIN_TAG="Coin";
        CHEST_TAG="Chest";
        TRAP_TAG="Trap";
        BOOMERANG_TAG="Boomerang";
        AIRCUTTER_TAG="Air Cutter";
        TRAP_TAG="Trap";
        GARBAGE_TAG="Garbage";

        isPlayerDead=false;
        numberOfCoins=0;
        currentHealth=maxHealth;
        startPosition=transform.position;
        offset_a_d=playerCollider.size.x*transform.localScale.x/2;
        offset_w_s=playerCollider.size.y*transform.localScale.y/2;
        offset_diagonal=Mathf.Sqrt(offset_a_d*offset_a_d + offset_w_s*offset_w_s);
        Physics2D.queriesStartInColliders=false;
        initializeWeaponIndexMap();
    }

    // Update is called once per frame
    void Update()
    {
        inputDirection=new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        inputMagnitude = inputDirection.magnitude;
        smoothInputMagnitude = Mathf.Lerp(smoothInputMagnitude, inputMagnitude, speed * Time.deltaTime);

        velocity = inputDirection *  smoothInputMagnitude * speed;
        

    
    }
    void initializeWeaponIndexMap()
    {
        Weapon_to_Index=new Dictionary<string, int>();
        int index=0;
        foreach(GameObject weapon in Weapons)
            Weapon_to_Index.Add(weapon.tag, index++);
    }
    void FixedUpdate()
    {
        playerMovement();
    }
    void LateUpdate()
    {
        playerAnimation();
        launchWeapon();
        StartCoroutine(changeWeapon());
        lineOfSight();
    }
    Vector2 getMovementDirection()
    {
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }
    float getOffset()
    {
        float offset=offset_diagonal;
        switch(angle)
        {
            case Mathf.PI/2     :   // W Direction
            case -Mathf.PI/2    :offset=offset_w_s;break;   // S Direction
            case 0              :   // D Direction
            case Mathf.PI       :offset=offset_a_d;break;   // A Direction
        }
        return offset;
    }
    void launchWeapon()
    {
        if(Input.GetKeyDown(KeyCode.X) && weaponMaxCount[weaponIndex]>0)
        {
            Vector2 direction=getMovementDirection();
            float offset=getOffset();
            Vector2 origin=playerBody.position+direction*offset;
            GameObject weapon=Instantiate(Weapons[weaponIndex], origin, Quaternion.Euler(0, 0, angle*Mathf.Rad2Deg));
            GameManager.addToReleasedWeaponStash(weapon.transform);
            if(weapon.CompareTag(BOOMERANG_TAG))
            {
                Boomerang boomerang=weapon.GetComponent<Boomerang>();
                boomerang.setReturnTransform(transform);
                boomerang.Offset=offset;
            }
            else if(weapon.CompareTag(AIRCUTTER_TAG))
                weapon.GetComponent<AirCutter>().setLaunchTransform(transform);
            weaponMaxCount[weaponIndex]--;
        }
    }
    void lineOfSight()
    {
        Vector2 direction=getMovementDirection();
        float offset=getOffset();
        Vector2 origin=playerBody.position+direction*offset;
        RaycastHit2D visibleObject=Physics2D.Raycast(origin, direction, minAccessRange);
        if(visibleObject.collider != null)
        {
            Debug.DrawLine(origin, visibleObject.point, Color.red);
            if(visibleObject.collider.CompareTag(CHEST_TAG))
                StartCoroutine(canOpenChestOrTrap(visibleObject.collider, CHEST_TAG));
            else if(visibleObject.collider.CompareTag(TRAP_TAG))
                StartCoroutine(canOpenChestOrTrap(visibleObject.collider, TRAP_TAG));
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
            numberOfCoins +=1;
            coinText.text ="Coins: " + numberOfCoins.ToString(); 
            collider.GetComponent<Coin>().onCoinCollected();
        }
        if(collider.CompareTag(GARBAGE_TAG)){
            collider.GetComponent<GarbageAnimation>().destroy();
        }

        if(collider.CompareTag(BOOMERANG_TAG))
        {
            Boomerang boomerang=collider.GetComponent<Boomerang>();
            if(boomerang.Damage && boomerang.getReturnTransform()!=transform)
            {
                currentHealth-=boomerang.getDamage();
                healthBar.value = (float)currentHealth/maxHealth;
                if(currentHealth>=0){
                    health.text = "Health : " + currentHealth.ToString();
                }
                else{
                    health.text = "Health : 0";
                }
                if(currentHealth<=0 && !isPlayerDead)
                {
                    isPlayerDead=true;
                    playerDeath();
                }
                boomerang.Damage=false;
            }
        }
        else if(collider.CompareTag(AIRCUTTER_TAG))
        {
            AirCutter aircutter=collider.GetComponent<AirCutter>();
            if(aircutter.Damage && aircutter.getLaunchTransform()!=transform)
            {
                currentHealth-=aircutter.getDamage();
                healthBar.value = (float)currentHealth/maxHealth;
                if(currentHealth>=0){
                    health.text = "Health : " + currentHealth.ToString();
                }
                else{
                    health.text = "Health : 0";
                }
                if(currentHealth<=0 && !isPlayerDead)
                {
                    isPlayerDead=true;
                    playerDeath();
                }
                aircutter.Damage=false;
            }
        }
    }
    IEnumerator canOpenChestOrTrap(Collider2D ChestOrTrap, string tag)
    {
        float startTime=-1f;
        if(Input.GetKeyDown(KeyCode.Z))
        {
            startTime=Time.time;
            while(Time.time-startTime<=longPressDuration && Input.GetKey(KeyCode.Z))
                yield return null;
            if(Time.time-startTime>=longPressDuration)
            {
                if(tag.Equals(CHEST_TAG))
                    ChestOrTrap.GetComponent<Chest>().openChest();
                else if(tag.Equals(TRAP_TAG))
                    ChestOrTrap.GetComponent<TrapLever>().openTrap();
            }   
            else
                Debug.Log("Press Longer");
        }
    }
    IEnumerator changeWeapon()
    {
        float startTime=-1f;
        if(Input.GetKeyDown(KeyCode.Q))
        {
            startTime=Time.time;
            while(Time.time-startTime<=longPressDuration && Input.GetKey(KeyCode.Q))
                yield return null;
            if(Time.time-startTime>=longPressDuration)
            {
                weaponIndex=(weaponIndex+1)%Weapons.Length;
            }
            else
                Debug.Log("Press Longer");
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            startTime=Time.time;
            while(Time.time-startTime<=longPressDuration && Input.GetKey(KeyCode.E))
                yield return null;
            if(Time.time-startTime>=longPressDuration)
            {
                if(numberOfCoins>=10){
                    numberOfCoins -=10;
                    coinText.text ="Coins: " + numberOfCoins.ToString(); 
                }
            }
            else
                Debug.Log("Press Longer");
        }
    }
    /*
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("--------------------------"+collision.gameObject.name+"------------------------------"+collision.gameObject.tag);
        if(collision.gameObject.CompareTag(BOOMERANG_TAG))
        {
            Boomerang boomerang=collision.gameObject.GetComponent<Boomerang>();
            if(boomerang.Damage && boomerang.getReturnTransform()!=transform)
            {
                currentHealth-=boomerang.getDamage();
                if(currentHealth<=0 && !isPlayerDead)
                {
                    isPlayerDead=true;
                    playerDeath();
                }
            }
            boomerang.Damage=false;
        }
        else if(collision.gameObject.CompareTag(AIRCUTTER_TAG))
        {
            AirCutter aircutter=collision.gameObject.GetComponent<AirCutter>();
            if(aircutter.Damage && aircutter.getLaunchTransform()!=transform)
            {
                currentHealth-=aircutter.getDamage();
                if(currentHealth<=0 && !isPlayerDead)
                {
                    isPlayerDead=true;
                    playerDeath();
                }
            }
            aircutter.Damage=false;
        }
    }
    */
    void playerDeath()
    {
        StartCoroutine(resetPlayer());
    }
    IEnumerator resetPlayer()
    {
        Material player=gameObject.GetComponent<Renderer>().material;
        while(player.color.a > 0.01f)
        {
            player.color = new Color(player.color.r, player.color.g, player.color.b, player.color.a-fadeSpeed*Time.deltaTime);
            yield return null;
        }
        int noOfChildren=transform.childCount;
        for(int i=0;i<noOfChildren;i++)
        {
            GameObject child=transform.GetChild(i).gameObject;
            if(child!=null)
                DestroyImmediate(child);
        }
        transform.position=startPosition;
        currentHealth=maxHealth;
        healthBar.value=1;
        health.text = "Health : " + maxHealth.ToString();
        isPlayerDead=false;
        NoOfPlays++;
        Debug.Log("Respawn??");
        player.color = new Color(player.color.r, player.color.g, player.color.b, 1);
    }
    public void restoreWeaponCount(string weaponName)
    {
        weaponMaxCount[Weapon_to_Index[weaponName]]++;
    }
}
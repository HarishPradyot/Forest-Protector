using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

    public int NoOfPlays;
    [SerializeField]
    private float fadeSpeed = 1f;
    public int maxHealth=100;
    [SerializeField]
    public int currentHealth;
    public Slider healthBar;

    [SerializeField]
    private TextMeshProUGUI health;
    public TextMeshProUGUI scoreText;
    public int score;
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
    public int TotalNumberOfCoins;
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
    private List<int> initialWeaponMaxCount;
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
        score =0;
        TotalNumberOfCoins=0;
        
        health.text = "Health : 100"; 
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
        initialWeaponMaxCount=new List<int>();
        int index=0;
        foreach(GameObject weapon in Weapons)
            Weapon_to_Index.Add(weapon.tag, index++);

        for(int i=0;i<weaponMaxCount.Length;i++)
            initialWeaponMaxCount.Add(weaponMaxCount[i]);
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
            gameObject.GetComponent<AudioSource>().Play();
            Vector2 origin=playerBody.position+direction*offset;
            GameObject weapon=Instantiate(Weapons[weaponIndex], origin, Quaternion.Euler(0, 0, angle*Mathf.Rad2Deg));
            weapon.layer=LayerMask.NameToLayer("PlayerWeapon");
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
            numberOfCoins +=1;
            TotalNumberOfCoins +=1;
            coinText.text ="Coins: " + numberOfCoins.ToString(); 
            score+=1;
            
            scoreText.text = "Score : " + score.ToString();
            collider.GetComponent<Coin>().onCoinCollected();
        }
        if(collider.CompareTag(GARBAGE_TAG)){
            score+=1;
            
            scoreText.text = "Score : " + score.ToString();
            FindObjectOfType<playerSpeech>().startConversation(3);
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
                currentHealth=Mathf.Clamp(currentHealth, 0, maxHealth);
                healthBar.value = (float)currentHealth/maxHealth;
                health.text = "Health : " + currentHealth.ToString();
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
                {

                    bool success = ChestOrTrap.GetComponent<Chest>().openChest();
                    if(success){
                        numberOfCoins +=10;
                        TotalNumberOfCoins +=10;
                        coinText.text ="Coins: " + numberOfCoins.ToString(); 
                        score+=10;
                        scoreText.text="Score : " + score.ToString();
                    }

                }
                else if(tag.Equals(TRAP_TAG)){

                    bool success = ChestOrTrap.GetComponent<TrapLever>().openTrap();
                    if(success){
                        score+=10;
                        scoreText.text="Score : " + score.ToString();
                    }
                }
            }   
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
        }

        if(Input.GetKeyDown(KeyCode.E))
        {
            startTime=Time.time;
            while(Time.time-startTime<=longPressDuration && Input.GetKey(KeyCode.E))
                yield return null;
            if(Time.time-startTime>=longPressDuration)
            {
                if(numberOfCoins>=5){
                    numberOfCoins -=5;
                    coinText.text ="Coins: " + numberOfCoins.ToString(); 
                    currentHealth = maxHealth;
                    health.text = "Health : " + currentHealth;
                    healthBar.value =(float)currentHealth/maxHealth; 
                }
            }
        }
    }
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
        for(int i=0;i<weaponMaxCount.Length;i++)
            weaponMaxCount[i]=initialWeaponMaxCount[i];
        player.color = new Color(player.color.r, player.color.g, player.color.b, 1);
    }
    public void restoreWeaponCount(string weaponName)
    {
        weaponMaxCount[Weapon_to_Index[weaponName]]++;
    }
}
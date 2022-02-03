using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAIMovement : MonoBehaviour
{
    // Health and other Parameters
    [SerializeField]
    private float fadeSpeed = 1f;
    private float maxHealth=50f;
    [SerializeField]
    private float currentHealth;
    [SerializeField]
    private GameObject[] Weapons;
    [SerializeField]
    private int[] weaponMaxCount;
    private int weaponIndex=0;
    private Dictionary<string, int> Weapon_to_Index;
    private bool isEnemyDead;

    // For PathFinder
    [SerializeField]
    private float pathUpdateSeconds=0.5f;
    private GameObject target;
    private bool reachedEndofPath;


    // Movement and Physics parameters
    [SerializeField]
    private int speed_Patrol=5, speed_Follow=12, forceSpeed=100;
    [SerializeField]
    private float nextWaypointDistance=2f, stopDistanceFromEnemy=5f, maxDistanceFromPatrolSite=100f, maxFollowDistance=100f, angle;
    private float distanceFollowed;
    
    // Additional References
    private Transform targetTransform;
    private Animator enemyAnimator;
    private string WALK_PARAMETER, PLAYER_TAG;
    private PlayerMovement targetMovement;
    private CircleCollider2D enemyCollider;
    private EnemyRegionalSpawner spawner;
    private float offset;  // Offset values for raycast so that the ray originates outside player collider
    private string BOOMERANG_TAG, AIRCUTTER_TAG;

    private Path path;
    private int currentWaypoint;
    private Seeker seeker;
    private Rigidbody2D enemyBody;
    
    // Movement and Physics actual values
    private int mode=1;   //0 - Force Mode    1 - Velocity Mode
    private Vector2 velocity;
    private Vector2 force;

    // State variables
    // Follow State
    [SerializeField]
    bool isFollowing;

    // Patrol State
    int previousWayPointIndex;
    bool isPatrolling;
    [SerializeField]
    private float playerFindDistance=10f, weaponLaunchDistance=15f;

    // Start is called before the first frame update
    void Start()
    {
        isEnemyDead=false;
        currentHealth=maxHealth;
        seeker=GetComponent<Seeker>();
        enemyBody=GetComponent<Rigidbody2D>();
        enemyAnimator=GetComponent<Animator>();
        spawner=transform.parent.GetComponent<EnemyRegionalSpawner>();

        reachedEndofPath=false;
        currentWaypoint=0;
        WALK_PARAMETER="Direction";
        PLAYER_TAG="Player";
        BOOMERANG_TAG="Boomerang";
        AIRCUTTER_TAG="Air Cutter";
        velocity=Vector2.zero;
        isFollowing=false;
        isPatrolling=true;
        setPatrolMode();

        enemyCollider=GetComponent<CircleCollider2D>();
        offset=enemyCollider.radius*1.2f;

        previousWayPointIndex=-1;
        targetTransform=spawner.randomWayPoint(ref previousWayPointIndex);
        seeker.StartPath(enemyBody.position, targetTransform.position, OnGenerationComplete);
        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
        Physics2D.queriesStartInColliders=false;
        initializeWeaponIndexMap();

        if(mode==0)
            enemyBody.drag=1;
        else
            enemyBody.drag=0;
    }
    void initializeWeaponIndexMap()
    {
        Weapon_to_Index=new Dictionary<string, int>();
        int index=0;
        foreach(GameObject weapon in Weapons)
            Weapon_to_Index.Add(weapon.tag, index++);
    }

    // For both follow and patrol
    void OnGenerationComplete(Path p)
    {
        if(!p.error)
        {    
            path=p;
            currentWaypoint=0;
        }
    }
    void UpdatePath()
    {
        if(!seeker.IsDone())
            return;
        if(isFollowing)
        {
            if(isFollowing)
            {
                if(!(isTargetFarAway() || isTargetMoving()))
                {    
                    if(target!=null)
                    {
                        Debug.Log("Stopped before Player");
                        return;
                    }
                    else
                        setPatrolMode();
                }
            }
            if(!followedForLong())    
                seeker.StartPath(enemyBody.position, targetTransform.position, OnGenerationComplete);
            else
                setPatrolMode();
        }
        if(isPatrolling && reachedEndofPath)
            seeker.StartPath(enemyBody.position, targetTransform.position, OnGenerationComplete);
    }
    void enemyAnimation()
    {
        int direction=-1;
        if((mode==1 && velocity.magnitude > 0.01f) || (mode==0 && force.magnitude > 0.01f))
        {    
            if(angle>Mathf.PI/4 && angle<3*Mathf.PI/4)
                direction=0;
            else if(angle<-Mathf.PI/4 && angle>-3*Mathf.PI/4)
                direction=1;
            else if(angle>=-Mathf.PI/4 && angle<=Mathf.PI/4)
                direction=2;
            else if((angle>=3*Mathf.PI/4 && angle<=Mathf.PI) || (angle>=-Mathf.PI && angle<=-3*Mathf.PI/4))
                direction=3;
        }
        enemyAnimator.SetInteger(WALK_PARAMETER, direction);
    }
    Vector2 getMovementDirection()
    {
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }
    void lineOfSight()
    {
        Vector2 direction=getMovementDirection();
        Vector2 origin=enemyBody.position+direction*offset;
        RaycastHit2D hitBody;
        if(isFollowing)
            hitBody=Physics2D.Raycast(origin, direction, weaponLaunchDistance);
        else if(isPatrolling)
            hitBody=Physics2D.Raycast(origin, direction, playerFindDistance);
        else
            hitBody=Physics2D.Raycast(origin, direction, 0.01f);
        if(hitBody.collider != null)
        {
            Debug.DrawLine(origin, hitBody.point, Color.yellow);
            if(hitBody.collider.CompareTag(PLAYER_TAG))
            {
                setFollowMode(hitBody.collider.gameObject);
                if(isFollowing)
                    launchWeapon();
            }
        }
        else
            Debug.DrawLine(origin, origin+direction*playerFindDistance, Color.blue);
    }

    // For Follow State
    bool isTargetMoving()
    {
        if(target==null)
            return false;
        return targetMovement.Velocity.magnitude > 0.01f;
    }
    bool isTargetFarAway()
    {
        if(target==null)
            return false;
        return (targetMovement.Position-(Vector2)transform.position).magnitude > stopDistanceFromEnemy;
    }
    bool followedForLong()
    {
        return distanceFollowed>maxFollowDistance;
    }
    bool farFromPatrolSite()
    {
        return Vector2.Distance(enemyBody.position, spawner.wayPoint(previousWayPointIndex).position)>maxDistanceFromPatrolSite;
    }
    void setFollowMode(GameObject hitBody)
    {
        // Set just this enemy as follow or all of them under that spawner?
        target=hitBody;
        targetTransform=hitBody.GetComponent<Transform>();
        targetMovement=hitBody.GetComponent<PlayerMovement>();
        isFollowing=true;
        isPatrolling=false;
    }
    void launchWeapon()
    {
        if(weaponMaxCount[weaponIndex]>0)
        {
            Vector2 direction=getMovementDirection();
            Vector2 origin=enemyBody.position+direction*offset;
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
    
    void followPath_FollowState()
    {
        if(path==null)
            return;
        
        if(currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndofPath=true;
            return;   
        }
        else
            reachedEndofPath=false;

        Vector2 direction=((Vector2)path.vectorPath[currentWaypoint]-enemyBody.position).normalized;
        angle=Mathf.Atan2(direction.y, direction.x);
        velocity=direction*speed_Follow;
        force=direction*forceSpeed*Time.deltaTime;

        if(mode==1 && velocity.magnitude > 0.01f)
        {    
            Vector2 displacement=velocity*Time.deltaTime;
            enemyBody.MovePosition(enemyBody.position+displacement);
            distanceFollowed+=displacement.magnitude;
        }
        else if(mode==0 && force.magnitude > 0.01f)
            enemyBody.AddForce(force);

        float distanceToCurrentWaypoint=Vector2.Distance(enemyBody.position, path.vectorPath[currentWaypoint]);
        if(distanceToCurrentWaypoint < nextWaypointDistance)
            currentWaypoint++;
    }


    // For Patrol State
    void setPatrolMode()
    {
        // Set just this enemy as follow or all of them under that spawner?
        distanceFollowed=0;
        target=null;
        targetTransform=null;
        targetMovement=null;

        isFollowing=false;
        isPatrolling=true;
        targetTransform=spawner.randomWayPoint(ref previousWayPointIndex);
    }
    void followPath_PatrolState()
    {
        if(path==null)
            return;
        
        if(currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndofPath=true;
            targetTransform=spawner.randomWayPoint(ref previousWayPointIndex);
            UpdatePath();
            return;   
        }
        else
            reachedEndofPath=false;

        Vector2 direction=((Vector2)path.vectorPath[currentWaypoint]-enemyBody.position).normalized;
        angle=Mathf.Atan2(direction.y, direction.x);
        velocity=direction*speed_Patrol;
        force=direction*forceSpeed*Time.deltaTime;

        if(mode==1 && velocity.magnitude > 0.01f)
            enemyBody.MovePosition(enemyBody.position+velocity*Time.deltaTime);
        
        else if(mode==0 && force.magnitude > 0.01f)
            enemyBody.AddForce(force);

        float distanceToCurrentWaypoint=Vector2.Distance(enemyBody.position, path.vectorPath[currentWaypoint]);
        if(distanceToCurrentWaypoint < nextWaypointDistance)
            currentWaypoint++;
    }

    // Update is called once per frame
    void Update()
    {   
        
    }
    void FixedUpdate()
    {
        velocity=Vector2.zero;
        if(isFollowing)
            followPath_FollowState();
        if(isPatrolling)
            followPath_PatrolState();
    }
    void LateUpdate()
    {
        lineOfSight();
        enemyAnimation();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(BOOMERANG_TAG))
        {
            Boomerang boomerang=collision.GetComponent<Boomerang>();
            if(boomerang.Damage && boomerang.getReturnTransform()!=transform)
            {
                currentHealth-=boomerang.getDamage();
                if(currentHealth<=0 && !isEnemyDead)
                {
                    isEnemyDead=true;
                    enemyDeath();
                }
                boomerang.Damage=false;
            }
        }
        else if(collision.CompareTag(AIRCUTTER_TAG))
        {
            AirCutter aircutter=collision.GetComponent<AirCutter>();
            if(aircutter.Damage && aircutter.getLaunchTransform()!=transform)
            {
                currentHealth-=aircutter.getDamage();
                if(currentHealth<=0 && !isEnemyDead)
                {
                    isEnemyDead=true;
                    enemyDeath();
                }
                aircutter.Damage=false;
            }
        }
    }
    void enemyDeath()
    {
        FindObjectOfType<PlayerMovement>().score += 20;
        FindObjectOfType<PlayerMovement>().scoreText.text = "Score : " + FindObjectOfType<PlayerMovement>().score.ToString();
         
        spawner.reduceEnemyCount();
        StartCoroutine(destroyEnemy());
    }
    IEnumerator destroyEnemy()
    {
        Material enemy=gameObject.GetComponent<Renderer>().material;
        while(enemy.color.a > 0.01f)
        {
            enemy.color = new Color(enemy.color.r, enemy.color.g, enemy.color.b, enemy.color.a-fadeSpeed*Time.deltaTime);
            yield return null;
        }
        int noOfChildren=transform.childCount;
        for(int i=0;i<noOfChildren;i++)
        {
            GameObject child=transform.GetChild(i).gameObject;
            if(child!=null)
                DestroyImmediate(child);
        }
        Destroy(gameObject);
    }
    public void restoreWeaponCount(string weaponName)
    {
        weaponMaxCount[Weapon_to_Index[weaponName]]++;
    }
}
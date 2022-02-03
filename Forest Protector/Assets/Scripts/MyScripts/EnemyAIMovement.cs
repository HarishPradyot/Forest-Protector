using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAIMovement : MonoBehaviour
{

    // For PathFinder
    [SerializeField]
    private float pathUpdateSeconds=0.5f;
    [SerializeField]
    private GameObject target;
    private bool reachedEndofPath;


    // Movement and Physics parameters
    [SerializeField]
    private int speed=12, forceSpeed=100;
    [SerializeField]
    private float nextWaypointDistance=2f, maxDistanceFromPatrolSite=100f, angle;
    private float distanceFollowed;
    
    // Additional References
    private Transform targetTransform;
    private Animator enemyAnimator;
    private string WALK_PARAMETER, PLAYER_TAG;
    private PlayerMovement targetMovement;
    private CircleCollider2D enemyCollider;
    private EnemyRegionalSpawner spawner;
    private float offset;  // Offset values for raycast so that the ray originates outside player collider

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
    private float playerFindDistance=10f;

    // Start is called before the first frame update
    void Start()
    {
        seeker=GetComponent<Seeker>();
        enemyBody=GetComponent<Rigidbody2D>();
        enemyAnimator=GetComponent<Animator>();
        targetTransform=target.GetComponent<Transform>();
        targetMovement=target.GetComponent<PlayerMovement>();
        spawner=transform.parent.GetComponent<EnemyRegionalSpawner>();

        reachedEndofPath=false;
        currentWaypoint=0;
        WALK_PARAMETER="Direction";
        PLAYER_TAG="Player";
        velocity=Vector2.zero;
        isFollowing=false;
        isPatrolling=true;
        setPatrolMode();

        enemyCollider=GetComponent<CircleCollider2D>();
        offset=enemyCollider.radius*1.2f;

        seeker.StartPath(enemyBody.position, targetTransform.position, OnGenerationComplete);
        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);

        if(mode==0)
            enemyBody.drag=1;
        else
            enemyBody.drag=0;
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
        RaycastHit2D hitBody=Physics2D.Raycast(origin, direction, playerFindDistance);
        if(hitBody.collider != null)
        {
            Debug.DrawLine(origin, hitBody.point, Color.yellow);
            if(hitBody.collider.CompareTag(PLAYER_TAG))
                setFollowMode(hitBody.collider.gameObject);
        }
        else
            Debug.DrawLine(origin, origin+direction*playerFindDistance, Color.blue);
    }


    // For Follow State
    bool isTargetMoving()
    {
        return targetMovement.Velocity.magnitude > 0.01f;
    }
    bool isTargetFarAway()
    {
        return (targetMovement.Position-(Vector2)transform.position).magnitude > nextWaypointDistance;
    }
    bool farFromPatrolSite()
    {
        return distanceFollowed>maxDistanceFromPatrolSite;
        return false;
        // && distance walked from patrol point is more than a value
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
    void UpdatePath()
    {
        if(seeker.IsDone())
        {   
            if(isFollowing)
            {
                if(!farFromPatrolSite() && (true || isTargetFarAway() || isTargetMoving()))    
                    seeker.StartPath(enemyBody.position, targetTransform.position, OnGenerationComplete);
                else
                    setPatrolMode();
            }
            if(isPatrolling && reachedEndofPath)
                seeker.StartPath(enemyBody.position, targetTransform.position, OnGenerationComplete);
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
        velocity=direction*speed;
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
        velocity=direction*speed;
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
        if(isFollowing && isPatrolling)
            Debug.Log("BOTH");
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
}
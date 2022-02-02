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
    private float nextWaypointDistance=2f, angle;
    
    // Additional References
    private Transform targetTransform;
    private Animator enemyAnimator;
    private string WALK_PARAMETER;
    private PlayerMovement targetMovement;


    private Path path;
    private int currentWaypoint;
    private Seeker seeker;
    private Rigidbody2D enemyBody;
    
    // Movement and Physics actual values
    private int mode=1;   //0 - Force Mode    1 - Velocity Mode
    private Vector2 velocity;
    private Vector2 force;

    // State variables
    [SerializeField]
    bool isFollowing;

    // Start is called before the first frame update
    void Start()
    {
        seeker=GetComponent<Seeker>();
        enemyBody=GetComponent<Rigidbody2D>();
        enemyAnimator=GetComponent<Animator>();
        targetTransform=target.GetComponent<Transform>();
        targetMovement=target.GetComponent<PlayerMovement>();

        reachedEndofPath=false;
        currentWaypoint=0;
        WALK_PARAMETER="Direction";
        velocity=Vector2.zero;

        seeker.StartPath(enemyBody.position, targetTransform.position, OnGenerationComplete);
        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);

        if(mode==0)
            enemyBody.drag=1;
        else
            enemyBody.drag=0;
    }

    void OnGenerationComplete(Path p)
    {
        if(!p.error)
        {    
            path=p;
            currentWaypoint=0;
        }
    }
    void isTargetMoving()
    {
        isFollowing = isFollowing || targetMovement.Velocity.magnitude > 0.01f;
    }
    void isTargetFarAway()
    {
        isFollowing= isFollowing || (targetMovement.Position-(Vector2)transform.position).magnitude > nextWaypointDistance;
    }
    void UpdatePath()
    {
        isFollowing=false;
        isTargetMoving();
        isTargetFarAway();
        if(seeker.IsDone() && isFollowing)
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
    void followPath()
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
            followPath();
    }
    void LateUpdate()
    {
        enemyAnimation();
    }
}
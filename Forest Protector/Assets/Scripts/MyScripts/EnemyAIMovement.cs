using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAIMovement : MonoBehaviour
{
    [SerializeField]
    private int speed=8;
    [SerializeField]
    private float nextWaypointDistance=3f, angle;
    [SerializeField]
    private GameObject target;
    private Transform targetTransform;
    private Animator enemyAnimator;
    private string WALK_PARAMETER;
    private PlayerMovement targetMovement;

    private Path path;
    private int currentWaypoint, direction;
    private Seeker seeker;
    private Rigidbody2D enemyBody;
    private bool reachedEndofPath;

    private Vector2 velocity;

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
        angle=-Mathf.PI/2;

        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

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
        if(seeker.IsDone() && (enemyBody.position-targetMovement.Position).magnitude > nextWaypointDistance)
            seeker.StartPath(enemyBody.position, targetTransform.position, OnGenerationComplete);
    }
    void enemyAnimation()
    {
        direction=-1;
        if(angle>Mathf.PI/4 && angle<3*Mathf.PI/4)
            direction=0;
        else if(angle<-Mathf.PI/4 && angle>-3*Mathf.PI/4)
            direction=1;
        else if(angle>=-Mathf.PI/4 && angle<=Mathf.PI/4)
            direction=2;
        else if((angle>=3*Mathf.PI/4 && angle<=Mathf.PI) || (angle>=-Mathf.PI && angle<=-3*Mathf.PI/4))
            direction=3;
        enemyAnimator.SetInteger(WALK_PARAMETER, direction);
    }
    // Update is called once per frame
    void Update()
    {
        if(path==null)
            return;
        
        if(currentWaypoint >= path.vectorPath.Count-1)
        {
            reachedEndofPath=true;
            return;   
        }
        else
            reachedEndofPath=false;

        Vector2 direction=((Vector2)path.vectorPath[currentWaypoint]-enemyBody.position).normalized;
        angle=Mathf.Atan2(direction.y, direction.x);
        velocity=direction*speed;

        float distanceToCurrentWaypoint=Vector2.Distance(enemyBody.position, path.vectorPath[currentWaypoint]);
        if(distanceToCurrentWaypoint < nextWaypointDistance)
            currentWaypoint++;
    }
    void FixedUpdate()
    {
        if(!reachedEndofPath)
            enemyBody.MovePosition(enemyBody.position+velocity*Time.deltaTime);
    }
    void LateUpdate()
    {
        enemyAnimation();
    }
}

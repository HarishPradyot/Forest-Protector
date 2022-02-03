using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class WanderingNPC : MonoBehaviour
{
    [SerializeField]
    private int speed_Wandering=3;
    private int currentWaypoint, previousWayPointIndex;
    private Vector2 velocity;
    private bool reachedEndofPath;
    private Seeker seeker;
    private Rigidbody2D NPCBody;
    private Path path;
    private Animator NPCAnimator;
    private string WALK_PARAMETER;
    private Transform[] wayPoints;
    private Transform targetTransform;
    [SerializeField]
    private float nextWaypointDistance=2f, pathUpdateSeconds=0.5f;
    private float angle;
    // Start is called before the first frame update
    void Start()
    {
        currentWaypoint=0;
        previousWayPointIndex=-1;
        reachedEndofPath=true;
        WALK_PARAMETER="Direction";

        seeker=GetComponent<Seeker>();
        NPCBody=GetComponent<Rigidbody2D>();
        NPCAnimator=GetComponent<Animator>();
        targetTransform=randomWayPoint();

        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        followPath_WanderingState();
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
        if(!seeker.IsDone())
            return;
        if(reachedEndofPath)
            seeker.StartPath(NPCBody.position, targetTransform.position, OnGenerationComplete);
    }
    void NPC_Animation()
    {
        int direction=-1;
        if(velocity.magnitude > 0.01f)
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
        NPCAnimator.SetInteger(WALK_PARAMETER, direction);
    }
    void followPath_WanderingState()
    {
        if(path==null)
            return;
        
        if(currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndofPath=true;
            targetTransform=randomWayPoint();
            UpdatePath();
            return;   
        }
        else
            reachedEndofPath=false;

        Vector2 direction=((Vector2)path.vectorPath[currentWaypoint]-NPCBody.position).normalized;
        angle=Mathf.Atan2(direction.y, direction.x);
        velocity=direction*speed_Wandering;

        if(velocity.magnitude > 0.01f)
            NPCBody.MovePosition(NPCBody.position+velocity*Time.deltaTime);
        
        float distanceToCurrentWaypoint=Vector2.Distance(NPCBody.position, path.vectorPath[currentWaypoint]);
        if(distanceToCurrentWaypoint < nextWaypointDistance)
            currentWaypoint++;
    }
    private Transform randomWayPoint()
    {
        int index=Random.Range(0, wayPoints.Length);
        if(index==previousWayPointIndex)
        {
            int il=Random.Range(0, previousWayPointIndex), ir=Random.Range(previousWayPointIndex+1, wayPoints.Length);
            if(Random.Range(0, 2)==0 || previousWayPointIndex==wayPoints.Length-1)
                index=il;
            else
                index=ir;
        }
        previousWayPointIndex=index;
        return wayPoints[index];
    }
}

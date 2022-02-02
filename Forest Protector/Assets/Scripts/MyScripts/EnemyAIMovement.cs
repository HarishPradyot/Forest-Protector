using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAIMovement : MonoBehaviour
{
    [SerializeField]
    private int speed=8;
    [SerializeField]
    private float nextWaypointDistance=3f;
    [SerializeField]
    private GameObject target;
    private Transform targetTransform;
    private PlayerMovement targetMovement;

    private Path path;
    private int currentWaypoint;
    private Seeker seeker;
    private Rigidbody2D enemyBody;
    private bool reachedEndofPath;

    private Vector2 velocity;

    // Start is called before the first frame update
    void Start()
    {
        seeker=GetComponent<Seeker>();
        enemyBody=GetComponent<Rigidbody2D>();
        targetTransform=target.GetComponent<Transform>();
        targetMovement=target.GetComponent<PlayerMovement>();

        reachedEndofPath=false;
        currentWaypoint=0;

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
}

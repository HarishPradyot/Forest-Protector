using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform playerTransform;

    private Vector3 cameraPosition;
    private float cameraDepth;

    [SerializeField]
    private float minX, maxX,minY,maxY;

    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        cameraDepth = transform.position.z;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!playerTransform)
            return;

        cameraPosition = playerTransform.position;
        cameraPosition.z = cameraDepth;
        
        if (cameraPosition.x < minX)
            cameraPosition.x = minX;

        if (cameraPosition.x > maxX)
            cameraPosition.x = maxX;

        if (cameraPosition.y < minY)
            cameraPosition.y = minY;

        if (cameraPosition.y > maxY)
            cameraPosition.y = maxY;

        transform.position = cameraPosition;
    }
}























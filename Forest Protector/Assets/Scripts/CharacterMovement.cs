using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    // Start is called before the first frame update
    
    [SerializeField]
    public float speed;

    private float movementX;

    private float movementY;


    // Update is called once per frame
    void Update()
    {
        
        PlayerMoveKeyboard();
    }

    void PlayerMoveKeyboard() {

        movementX = Input.GetAxisRaw("Horizontal");
        movementY = Input.GetAxisRaw("Vertical");

        transform.position += new Vector3(movementX, movementY, 0f)*speed*Time.deltaTime;

    }

    /// <summary>
    /// Sent when an incoming collider makes contact with this object's
    /// collider (2D physics only).
    /// </summary>
    /// <param name="other">The Collision2D data associated with this collision.</param>
    void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Collided");
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    // Start is called before the first frame update
    
    [SerializeField]
    private float speed;

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
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private int speed, direction;
    [SerializeField] 
    private float angle;

    private Vector2 displacement;
    private SpriteRenderer spriteRenderer;
    private Animator playerAnimator;
    private string WALK_PARAMETER;

    void Start()
    {
        spriteRenderer=GetComponent<SpriteRenderer>();
        playerAnimator=GetComponent<Animator>();
        WALK_PARAMETER="Direction";
    }

    // Update is called once per frame
    void Update()
    {
        playerMovement();
        playerAnimation();   
    }
    void playerMovement()
    {
        displacement=new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized*speed*Time.deltaTime;
        transform.position=transform.position+(Vector3)displacement;
    }
    void playerAnimation()
    {
        
        direction=-1;
        /*
        if(displacement.x == displacement.y)
        {
            //For diagonal movement if required?
            if(displacement.x > 0)
            {
                if(displacement.y > 0)  //WD Direction
                    direction=4;
                else                    //SD Direction
                    direction=5;
            }
            if(displacement.x < 0)
            {
                if(displacement.y > 0)  //WA Direction
                    direction=6;
                else                    //SA Direction
                    direction=7;
            }
        }
        else if(displacement.y > 0)  //W Direction
            direction=0;
        else if(displacement.y < 0)  //S Direction
            direction=1;
        else if(displacement.x > 0)  //D Direction
            direction=2;
        else if(displacement.x < 0)  //A Direction
            direction=3;
        */

        if(displacement.magnitude != 0)
        {
            angle=Mathf.Atan2(displacement.y, displacement.x);
            switch(angle)
            {
                case Mathf.PI/2     :direction=0;break;
                case -Mathf.PI/2    :direction=1;break;
                case 0              :direction=2;break;
                case Mathf.PI       :direction=3;break;
                case Mathf.PI/4     :direction=4;break;
                case 3*Mathf.PI/4   :direction=5;break;
                case -Mathf.PI/4    :direction=6;break;
                case -3*Mathf.PI/4  :direction=7;break;
            }
        }

        playerAnimator.SetInteger(WALK_PARAMETER, direction);
    }
}

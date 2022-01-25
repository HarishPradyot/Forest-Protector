using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private int speed, direction;

    [SerializeField]
    private Sprite[] idleSprites;
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
        // int prevDirection=direction;
        direction=-1;
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

        playerAnimator.SetInteger(WALK_PARAMETER, direction);
        // if(direction==-1 && prevDirection>=0 && prevDirection<=3)
        //     spriteRenderer.sprite=idleSprites[prevDirection];
    }
}

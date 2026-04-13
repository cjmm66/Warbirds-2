using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomberController : MonoBehaviour
{
    [SerializeField] float FlightSpeed = 1.2f;
    [SerializeField] GameObject Bomb1Prefab;
    [SerializeField] Rigidbody2D bomberRb;
    private float bombDropInterval;
    private float timeSinceLastBomb;
    [SerializeField] GameObject Bomb2Prefab;
    [SerializeField] SpriteRenderer plane;
    [SerializeField] float maxBorderX = 15f; // boundary X for turning around
    public bool movingRight = true;

    private Bomb bombLogic;
    private float oldPosition;




    void Start()
    {
        SetRandomBombDropInterval();
        timeSinceLastBomb = 0f;
        oldPosition = transform.position.x; // initialize so first TrackPosition() frame is correct
    }

    private void LateUpdate()
    {
        oldPosition = transform.position.x;
    }

    void Update()
    {
        BomberMovement();
        HandleBombDropping();
        TrackPosition();
    }

    void TrackPosition()
    {
        if (transform.position.x > oldPosition)
        {
            movingRight = true;
            plane.flipX = true;
        }
        if (transform.position.x < oldPosition)
        {
            movingRight = false;
            plane.flipX = false;
        }
    }

    public void BomberMovement()
    {
        // Hit right boundary while moving right → reverse
        if (transform.position.x >= maxBorderX && movingRight)
        {
            movingRight = false;
        }
        // Hit left boundary while moving left → reverse
        else if (transform.position.x <= -maxBorderX && !movingRight)
        {
            movingRight = true;
        }

        bomberRb.linearVelocity = new Vector2(movingRight ? FlightSpeed : -FlightSpeed, bomberRb.linearVelocity.y);
    }


    void HandleBombDropping()
    {

        timeSinceLastBomb += Time.deltaTime;
        if (timeSinceLastBomb >= bombDropInterval)
        {
            DropBomb();
            SetRandomBombDropInterval();
            timeSinceLastBomb = 0f;
        }
    } 

    void DropBomb() //Rewrok can be done on this function, Unecessary Repetition
    {
        int bombtype = Random.Range(1, 3);
        if ((bombtype == 1)&&(movingRight))
        // Instantiate a bomb at the bomber's position with the bomber's rotation
        {  
            GameObject bomb = Instantiate(Bomb1Prefab, transform.position, Quaternion.Euler(0, 0, 0));

            // I set the reference of Instantiated Bomber, it is repeatede through each one
            bombLogic = bomb.GetComponent<Bomb>();
            bombLogic.parentBomber = this.gameObject.GetComponent<BomberController>();
            //SpriteRenderer bombsprite = bomb.GetComponent<SpriteRenderer>();



            // Set the bomb's initial velocity to match the bomber's velocity
            Rigidbody2D bombRb = bomb.GetComponent<Rigidbody2D>();
            if (bombRb != null)
            {
                bombRb.linearVelocity = bomberRb.linearVelocity;

            }
        }else if((bombtype == 1) && (!movingRight))
        {
           
            GameObject bomb = Instantiate(Bomb1Prefab, transform.position, Quaternion.Euler(0, 0, 90));

            bombLogic = bomb.GetComponent<Bomb>();
            bombLogic.parentBomber = this.gameObject.GetComponent<BomberController>();
            //SpriteRenderer bombsprite = bomb.GetComponent<SpriteRenderer>();
            //bombsprite.flipY = true;

            // Set the bomb's initial velocity to match the bomber's velocity
            Rigidbody2D bombRb = bomb.GetComponent<Rigidbody2D>();
            if (bombRb != null)
            {
                bombRb.linearVelocity = bomberRb.linearVelocity;

            }
        }



        else if((bombtype == 2) && (movingRight)) 

        {
            GameObject bomb = Instantiate(Bomb2Prefab, transform.position, Quaternion.Euler(0, 0, 0 ));

            bombLogic = bomb.GetComponent<Bomb>();
            bombLogic.parentBomber = this.gameObject.GetComponent<BomberController>();

            // Set the bomb's initial velocity to match the bomber's velocity
            Rigidbody2D bombRb = bomb.GetComponent<Rigidbody2D>();
            if (bombRb != null)
            {
                bombRb.linearVelocity = bomberRb.linearVelocity;

            }
        }
        else if(bombtype == 2 && (!movingRight))
        {
            GameObject bomb = Instantiate(Bomb2Prefab, transform.position, Quaternion.Euler(0, 0, 90));

            bombLogic = bomb.GetComponent<Bomb>();
            bombLogic.parentBomber = this.gameObject.GetComponent<BomberController>();
            //SpriteRenderer bombsprite = bomb.GetComponent<SpriteRenderer>();
            //bombsprite.flipY = true;
            // Set the bomb's initial velocity to match the bomber's velocity
            Rigidbody2D bombRb = bomb.GetComponent<Rigidbody2D>();
            if (bombRb != null)
            {
                bombRb.linearVelocity = bomberRb.linearVelocity;

            }
        }


    }

    

    void SetRandomBombDropInterval()
    {
        // Set a random interval between 1 and 8 seconds
        bombDropInterval = Random.Range(1f, 8f);
    }
}
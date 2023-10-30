using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using AudioManagerLib;

public class PlayerLogic : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator anim;
    [SerializeField] GameObject particleGO;
    ParticleSystem ps;
    bool isOnGround;
    float jumpHorizontal;
    float jumpVertical;
    float jumpStrength;
    Direction facing;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        ps = particleGO.GetComponent<ParticleSystem>();

        facing = Direction.Right;
        jumpHorizontal = 6.5f;
        jumpVertical = 14.0f;
        jumpStrength = 0.1f;
        isOnGround = true;

        Physics2D.gravity = new Vector2(0.00f, -20.00f);
    }

    void Update()
    {
        UpdateDirection(); // Direction Inputs        
        JumpLogic(); // Jump Logic

        anim.SetFloat("Vertical Speed", rb.velocity.y); // Update Animation
    }

    // Listen for LEFT and RIGHT inputs, and change player Direction accordingly
    void UpdateDirection()
    {
        if (Input.GetKeyDown(KeyCode.A) && isOnGround && facing == Direction.Right)
        {
            facing = Direction.Left;
            sr.flipX = true;

        }
        else if (Input.GetKeyDown(KeyCode.D) && isOnGround && facing == Direction.Left)
        {
            facing = Direction.Right;
            sr.flipX = false;
        }
    }

    // When on ground, listen for JUMP input
    // Holding down JUMP input results in higher jumpStrength, but it cannot exceed 1
    // Minimum jumpStrength is 0.1
    void JumpLogic()
    {
        if (isOnGround)
        {
            if (Input.GetKeyUp(KeyCode.Space) || jumpStrength > 1)
            {
                Jump();
                jumpStrength = 0.1f;
            }
            else if (Input.GetKey(KeyCode.Space))
            {
                jumpStrength += Time.deltaTime;
            }
        }
    }

    // Jump in predetermined direction, based on player Direction
    void Jump()
    {
        // Jump in a direction
        if (facing == Direction.Left)
        {
            rb.velocity = new Vector2(-jumpHorizontal * jumpStrength, jumpVertical * jumpStrength);
        }
        else if (facing == Direction.Right)
        {
            rb.velocity = new Vector2(jumpHorizontal * jumpStrength, jumpVertical * jumpStrength);
        }

        // Play jump sound
        AudioManager.PlaySound("Jump");
    }

    // Actions to do on landing
    void OnLanding(bool isLandingSoft)
    {
        // Set "Is on Ground"
        isOnGround = true;
        anim.SetBool("Is on Ground", isOnGround);

        if (!isLandingSoft)
        {
            // Generate particles
            particleGO.transform.position = new Vector2(transform.position.x, transform.position.y - 0.6f);
            ps.Play();

            // Play hard landing sound
            AudioManager.PlaySound("Hard Landing");
        }
        else
        {
            // Play soft landing sound
            AudioManager.PlaySound("Soft Landing");
        }
    }

    // Actions to do on bonking your stupid fookin head on a wall
    void OnBounce()
    {
        // Play wall bonk sound
        AudioManager.PlaySound("Wall Bonk");
    }

    // On Triggers
    void OnTriggerEnter2D(Collider2D other) { }
    void OnTriggerStay2D(Collider2D other) { }
    void OnTriggerExit2D(Collider2D other) { }

    // On Collisions
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Standable") && rb.velocity.magnitude < 0.01)
        {
            OnLanding(collision.relativeVelocity.y < 10);
        }
        else if (collision.gameObject.CompareTag("Bounceable"))
        {
            OnBounce();
        }
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Standable") && !isOnGround && rb.velocity.magnitude < 0.01f)
        {
            OnLanding(false);
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Standable"))
        {
            isOnGround = false;
            anim.SetBool("Is on Ground", isOnGround);
        }
    }

}

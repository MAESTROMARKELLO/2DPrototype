using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Movement
    public float movementSpeed = 1f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;

    // Animation
    private bool canMove = true;
   
    // Attack
    public Transform attackPoint;
    public float radius;
    public float damage = 50f;

    // Player components
    Vector2 movementInput;
    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (movementInput != Vector2.zero)
        {
            bool success = TryMove(movementInput);

            // Updates the animator's parameters based on the movement direction to trigger appropriate animations.
            animator.SetBool("isMoving", success && movementInput.x != 0);
            animator.SetBool("isMovingUp", success && movementInput.y > 0);
            animator.SetBool("isMovingDown", success && movementInput.y < 0);

            // Makes the player keep moving when hitting corner of a collidable object instead of stopping.
            if (!success && movementInput.x != 0)
            {
                success = TryMove(new Vector2(movementInput.x, 0));

                if (!success && movementInput.y != 0)
                {
                    success = TryMove(new Vector2(0, movementInput.y));
                }
            }
        }
        else
        {
            // Animation stuff
            animator.SetBool("isMoving", false);
            animator.SetBool("isMovingUp", false);
            animator.SetBool("isMovingDown", false);
        }

        if (canMove) 
        {
            // Set direction of sprite to movement direction
            if (movementInput.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            else if (movementInput.x > 0)
            {
                spriteRenderer.flipX = false;
            }
        }
    }

    private bool TryMove (Vector2 direction)
    {
        int count = rb.Cast(direction, movementFilter, castCollisions, movementSpeed * Time.fixedDeltaTime + collisionOffset); // Casts to detect collision.

        if (count == 0) // If there's no collision then move the player
        {
            rb.MovePosition(rb.position + direction * movementSpeed * Time.fixedDeltaTime); // Moves the player
            return true;
        }
        return false; // Indicates the movement was unsuccessful.
    }

    void OnMove(InputValue movementValue)
    {
       movementInput = movementValue.Get<Vector2>();
    }

    void Update()
    {
        // Track current animation to locate the position of the hitbox 
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.player_attack_down") || animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.player_idle") 
            || animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.player_walk_down"))
        {
            attackPoint.position = new Vector3(rb.transform.position.x, rb.transform.position.y - 1f, rb.transform.position.z);
        }
        else if ((animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.player_attack_right") || animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.player_walk")) && !spriteRenderer.flipX)
        {
            attackPoint.position = new Vector3(rb.transform.position.x + 0.85f, rb.transform.position.y, rb.transform.position.z);
        }
        else if ((animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.player_attack_right") || animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.player_walk")) && spriteRenderer.flipX)
        {
            attackPoint.position = new Vector3(rb.transform.position.x - 0.85f, rb.transform.position.y, rb.transform.position.z);
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.player_attack_up") || animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.player_walk_up")) 
        {
            attackPoint.position = new Vector3(rb.transform.position.x, rb.transform.position.y + 0.85f, rb.transform.position.z);
        }

        // Forbid change of the attack direction
        if (canMove)
        {
            // Attack when player presses space bar
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // Stop character from moving while animation is played
                rb.bodyType = RigidbodyType2D.Static;
                canMove = false;

                // Start attack animation 
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack()
    {
        // Play animation
        animator.SetTrigger("Attack");

        //Detect all enemies hit
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, radius);

        // Destroy/Deal damage to every enemy hit
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Enemy hit :" + enemy.name);
            enemy.GetComponent<EnemyController>().GetHit(damage);
        }

        // Wait until animation is finished
        yield return new WaitForSeconds(0.55f);

        // Lets character move
        rb.bodyType = RigidbodyType2D.Kinematic;
        canMove = true;
    }

    public void GetHit(float damage)
    {
        Debug.Log($"I have been hit for {damage} damage!");
    }
}
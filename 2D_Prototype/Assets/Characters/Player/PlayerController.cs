using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 1f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;

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
}
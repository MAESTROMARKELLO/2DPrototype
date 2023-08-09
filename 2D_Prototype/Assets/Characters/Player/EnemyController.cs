using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    
    public float speed;
    public float checkRadius;
    public float attackRadius;
    public bool shouldRotate;
    private float attackRate = 2f;
    private float nextAttack = 0f;
    public Transform attackPoint;
    public float radius;
    public float damage = 50f;

    public LayerMask whatIsPlayer;

    private Transform target;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 movement;
    public Vector3 dir;

    private bool isInChaseRange;
    private bool isInAttackRange;
    private bool canAttack;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    { 
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        target = GameObject.FindWithTag("Player").transform;
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        isInChaseRange = Physics2D.OverlapCircle(transform.position, checkRadius, whatIsPlayer);
        isInAttackRange = Physics2D.OverlapCircle(transform.position, attackRadius, whatIsPlayer);

        anim.SetBool("IsRunning", isInChaseRange);

        dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        dir.Normalize();
        movement = dir;

        if (dir.x < 0) sr.flipX = true;
        else sr.flipX = false;

        if (shouldRotate)
        {
            anim.SetFloat("X", dir.x);
            anim.SetFloat("Y", dir.y);

            float curX = anim.GetFloat("X");
            float curY = anim.GetFloat("Y");

            if (curX == 0 && curY == 0 || (curY < 0 && -curY > curX))
            {
                attackPoint.position = new Vector2(rb.transform.position.x, rb.transform.position.y - 1.3f);
            }
            else if ((curX > 0 && curX > curY) && !sr.flipX)
            {
                attackPoint.position = new Vector2(rb.transform.position.x + 1f, rb.transform.position.y);
            }
            else if ((curX < 0 && -curX > curY) && sr.flipX)
            {
                attackPoint.position = new Vector2(rb.transform.position.x - 1f, rb.transform.position.y);
            }
            else if (curY > 0 && curY > curX)
            {
                attackPoint.position = new Vector2(rb.transform.position.x, rb.transform.position.y + 1f);
            }
        }

        if (Time.time >= nextAttack)
        {
            canAttack = true;
        }
        else
        {
            canAttack = false;
        }
    }

    private void FixedUpdate()
    {
        if (isInChaseRange && !isInAttackRange)
        {
            MoveCharacter(movement);
        }

        if (isInAttackRange && canAttack)
        {
            rb.bodyType = RigidbodyType2D.Static;
            anim.SetBool("IsAttacking", true);
            nextAttack = Time.time + attackRate;

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, radius);

            foreach (Collider2D enemy in hitEnemies)
            {
                if (enemy != null)
                {
                    Debug.Log("Enemy hit :" + enemy.name);
                    enemy.GetComponent<PlayerController>().GetHit(damage);
                }
            }
        }
        else 
        {
            anim.SetBool("IsAttacking", false);
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    private void MoveCharacter(Vector2 dir)
    {
        rb.MovePosition((Vector2)transform.position + (dir * speed * Time.deltaTime));
    }

    public void GetHit(float damage) 
    {
        currentHealth -= damage;

        if (currentHealth <= 0) 
        {
            Die();
        }
    }

    void Die() 
    {
        Destroy(this.gameObject);
    }
}

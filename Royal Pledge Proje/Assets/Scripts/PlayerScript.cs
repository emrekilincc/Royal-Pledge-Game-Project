using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator playerAnimator;
    
    // player run speed
    public float speed = 5f;
    
    // player jump force
    public float jumpForce = 12.2f;
    public int jumpLimit = 2;
    private int jumpCounter;
    
    // attack variables
    public int attackDamage = 50;
    public Transform attackPoint;
    public float attackRange = 0.9f;
    public LayerMask enemyLayers;
    
    // player is ground check variables
    public Transform groundCheck;
    public float groundCheckRange = 0.2f;
    public LayerMask groundLayer;
    private bool isGround;
    
    private bool facingRight = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
    }

    private void Update()
    {
        // set isGround
        CheckGround();
        
        MovePlayer();
        
        // player jumping
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
        // player do attack
        if (Input.GetKeyDown(KeyCode.F))
        {
            Attack();
        }
        
        playerAnimator.SetBool("isGround", isGround);
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void MovePlayer()
    {
        // player movement
        float horizontalInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);
        
        // play run animation
        playerAnimator.SetFloat("speed", Mathf.Abs(rb.velocity.x));

        // player face direction
        if ((horizontalInput > 0 && !facingRight) || (horizontalInput < 0 && facingRight))
        {
            Flip();
        }
    }

    private void Jump()
    {
        if (isGround)
        {
            jumpCounter = 0;
            
            // player is jump
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);

            jumpCounter += 1;
        }
        else
        {
            if (jumpCounter < jumpLimit)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpCounter += 1;
            }
        }
    }

    private void Attack()
    {
        // play attack animation
        playerAnimator.SetTrigger("Attack");

        // detect enemies
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Damage enemies
        foreach (Collider2D Enemy in hitEnemies)
        {
            print("Hit " + Enemy.name);
            Enemy.GetComponent<EnemyController>().TakeDamage(attackDamage);
        }
    }

    private void CheckGround()
    {
        // detect grounds
        isGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRange, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null || groundCheck == null)
            return;
        
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRange);
    }
}
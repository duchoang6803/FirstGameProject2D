using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

public class DemonAxeController : MonoBehaviour,IDamagable
{
    private enum State
    {
        Walking,
        Knockback,
        Dead
    }

    private State currentState;
    [SerializeField]
    private Transform groundCheck, wallCheck, checkTouchDamage;

    [SerializeField]
    private GameObject hitParticle;

    public GameObject alive;

    [SerializeField]
    private float groundCheckDistance, wallCheckDistance, movementSpeed, knockbackSpeedX,knockbackSpeedY,maxHealth, knockbackDuration,
        touchDamageCoolDown,checkTouchDamageWidth,checkTouchDamageHeigh;

    private float currentHealth,knockbackStartTime,timeDemonDeath,timeDemonAnimDeath = 2.8f, lastTouchDamageTime;

    [SerializeField]
    private LayerMask whatisGround, whatIsPlayer;
    private Vector2 movement, touchDamageBotLeft, touchDamageTopRight;

    private Rigidbody2D aliveRB;

    private int facingDirection, damageDirection;
    [SerializeField]
    private int touchDamage = 20;

    private bool groundDetected, wallDetected;
    private bool isDead;

    private Animator aliveAnim;
    private PlayerController player;
    private PlayerCombatController playerCombat;
    private Collider2D hit;
    private HealthBar healthBar;


    private void Start()
    {
        alive = transform.Find("Alive").gameObject;
        aliveRB = alive.GetComponent<Rigidbody2D>();
        aliveAnim = alive.GetComponent<Animator>();
        currentHealth = maxHealth;
        player = GameObject.Find("player").GetComponent<PlayerController>();
        playerCombat = GetComponent<PlayerCombatController>();

        facingDirection = 1;
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Walking:
                UpdateWalkingState();
                break;
            case State.Knockback:
                UpdateKnockbackState();
                break;
            case State.Dead:
                UpdateDeadState();
                break;
        }
    }


    // Walking State ----------------------------

    private void EnterWalkingState()
    {

    }

    private void UpdateWalkingState()
    {
        groundDetected = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatisGround);
        wallDetected = Physics2D.Raycast(wallCheck.position,Vector2.right, wallCheckDistance, whatisGround);
        CheckTouchDamage();
        if (!groundDetected || wallDetected)
        {
            Flip();
        }

        else
        {
            movement.Set(facingDirection * movementSpeed, aliveRB.velocity.y);
            aliveRB.velocity = movement;
        }
    }

    private void ExitWalkingState()
    {

    }

    // Knockback State -------------------------

    private void EnterKnockbackState()
    {
        knockbackStartTime = Time.time;
        movement.Set(knockbackSpeedX * damageDirection, knockbackSpeedY);
        aliveRB.velocity = movement;
        aliveAnim.SetBool("knockback", true);
    }

    private void UpdateKnockbackState()
    {
        if (Time.time > knockbackStartTime + knockbackDuration)
        {
            SwitchState(State.Walking);
        }
    }

    private void ExitKnockbackState()
    {
        aliveAnim.SetBool("knockback", false);
    }

    // Deade State -----------------------------

    private void EnterDeadState()
    {
        if(currentHealth <= 0f)
        {
            isDead = true;
            aliveAnim.SetBool("isDead", true);
        }
        if (isDead)
        {
            timeDemonDeath = Time.deltaTime;
            Destroy(gameObject, timeDemonDeath + timeDemonAnimDeath);
        }
    }

    private void UpdateDeadState()
    {
       
    }

    private void ExitDeadState()
    {

    }

    // OTHER FUNTIONS

    public void Damage(DamageInfo damageInfo)
    {
        if(damageInfo.DamageAmount <= 0)
        {
            return;
        }
        currentHealth -= damageInfo.DamageAmount;
        Instantiate(hitParticle,new Vector3(alive.transform.position.x,alive.transform.position.y - 1f), Quaternion.Euler(0, 0, Random.Range(0, 360f)));
        if (damageInfo.DamageSource.transform.position.x > alive.transform.position.x)
        {
            damageDirection = -1;
        }
        else
        {
            damageDirection = 1;
        }
        if (currentHealth > 0f)
        {
            SwitchState(State.Knockback);
        }
        else if(currentHealth <= 0f)
        {
            SwitchState(State.Dead);
        }
    }

    private void CheckTouchDamage()
    {
        if (Time.time > lastTouchDamageTime + touchDamageCoolDown)
        {           
            touchDamageBotLeft.Set(checkTouchDamage.position.x - (checkTouchDamageWidth / 2), checkTouchDamage.position.y - (checkTouchDamageHeigh / 2));
            touchDamageTopRight.Set(checkTouchDamage.position.x + (checkTouchDamageWidth / 2), checkTouchDamage.position.y + (checkTouchDamageHeigh / 2));

            hit = Physics2D.OverlapArea(touchDamageBotLeft, touchDamageTopRight, whatIsPlayer);

            if (hit != null)
            {
                lastTouchDamageTime = Time.time;
                IDamagable iDamageable = hit.GetComponent<IDamagable>();
                if (iDamageable == null)
                {
                    iDamageable = hit.GetComponentInParent<IDamagable>();
                }
                if (iDamageable != null)
                {
                    iDamageable.Damage(new DamageInfo()
                    {
                        DamageAmount = touchDamage,
                        DamageSource = this.alive.gameObject,
                    });
                }
            }
        }
    }

    private void Flip()
    {
        facingDirection *= -1;
        alive.transform.Rotate(0, 180f, 0);

    }

    private void SwitchState(State state)
    {
        switch (currentState)
        {
            case State.Walking:
                ExitWalkingState();
                break;
            case State.Knockback:
                ExitKnockbackState();
                break;
            case State.Dead:
                ExitDeadState();
                break;
        }

        switch (state)
        {
            case State.Walking:
                EnterWalkingState();
                break;
            case State.Knockback:
                EnterKnockbackState();
                break;
            case State.Dead:
                EnterDeadState();
                break;
        }
        currentState = state;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y + groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));

        Vector2 botLeft = new Vector2(checkTouchDamage.position.x - (checkTouchDamageWidth / 2), checkTouchDamage.position.y - (checkTouchDamageHeigh / 2));
        Vector2 botRight = new Vector2(checkTouchDamage.position.x + (checkTouchDamageWidth / 2), checkTouchDamage.position.y - (checkTouchDamageHeigh / 2));
        Vector2 topLeft = new Vector2(checkTouchDamage.position.x - (checkTouchDamageWidth / 2), checkTouchDamage.position.y + (checkTouchDamageHeigh / 2));
        Vector2 topRight = new Vector2(checkTouchDamage.position.x + (checkTouchDamageWidth / 2), checkTouchDamage.position.y + (checkTouchDamageHeigh / 2));

        Gizmos.DrawLine(botLeft, botRight);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(botLeft, topLeft);
        Gizmos.DrawLine(botRight, topRight);
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CombatDummyController : MonoBehaviour,IDamagable
{
    private readonly string IsPlayerFacingRight = "PlayerOnLeft";

    [SerializeField]
    private float maxHealth, knockbackSpeedX, knockbackSpeedY, knockbackDeadSpeedX, knockbackDeadSpeedY, deadTorque;

    [SerializeField]
    private GameObject hitParticle;

    [SerializeField]
    private float knockbackDuration;

    [SerializeField]
    private bool applyKnockback;

    private float currentHealth, knockbackStart;

    private int playerFacingDirection;

    private bool knockback, isDead, playerOnLeft;

    private PlayerController player;

    private GameObject aliveGO, brokenTopGO, brokenBotGO;
    private Rigidbody2D rbAlive, rbBrokenTop, rbBrokenBot;
    private Animator aliveAnim;

    private void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.Find("player").GetComponent<PlayerController>();
        aliveGO = transform.Find("Alive").gameObject;
        brokenTopGO = transform.Find("Broken Top").gameObject;
        brokenBotGO = transform.Find("Broken Bottom").gameObject;

        aliveAnim = aliveGO.GetComponent<Animator>();
        rbAlive = aliveGO.GetComponent<Rigidbody2D>();
        rbBrokenTop = brokenTopGO.GetComponent<Rigidbody2D>();
        rbBrokenBot = brokenBotGO.GetComponent<Rigidbody2D>();

        aliveGO.SetActive(true);
        brokenTopGO.SetActive(false);
        brokenBotGO.SetActive(false);
    }

    private void Update()
    {
        CheckKnockBack();
    }

    //private void Damage(float amount)
    //{
    //    currentHealth -= amount;
    //    playerFacingDirection = pc.GetFacingDirection();

    //    Instantiate(hitParticle,aliveGO.transform.position,Quaternion.Euler(0,0,Random.Range(0,360f)));

    //    if (playerFacingDirection == 1)
    //    {
    //        playerOnLeft = true;

    //    }
    //    else
    //    {
    //        playerOnLeft = false;
    //    }

    //    aliveAnim.SetBool("PlayerOnLeft", playerOnLeft);
    //    aliveAnim.SetTrigger("damage");

    //    if (applyKnockback && currentHealth > 0.0f)
    //    {
    //        Knockback();
    //    }

    //    if (currentHealth < 0.0f)
    //    {
    //        Die();
    //    }
    //}
    //private void Damage2(float amount)
    //{
    //    currentHealth -= amount;
    //    playerFacingDirection = player.GetFacingDirection();

    //    Instantiate(hitParticle, aliveGO.transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360f)));

    //    aliveAnim.SetBool("PlayerOnLeft", playerFacingDirection == 1);
    //    aliveAnim.SetTrigger("damage");

    //    if (applyKnockback && currentHealth > 0.0f)
    //    {
    //        Knockback();
    //    }

    //    if (currentHealth < 0.0f)
    //    {
    //        Die();
    //    }
    //}
    public void Damage(DamageInfo damageInfo)
    {
        if (damageInfo.DamageAmount <= 0)
        {
            return;
        }

        currentHealth -= damageInfo.DamageAmount;

        Instantiate(hitParticle, aliveGO.transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360f)));

        playerFacingDirection = player.GetFacingDirection();

        if (playerFacingDirection == 1)
        {
            playerOnLeft = true;
        }

        if (playerFacingDirection == -1)
        {
            playerOnLeft = false;
        }


        aliveAnim.SetBool(IsPlayerFacingRight,playerOnLeft);
        aliveAnim.SetTrigger("damage");

        if (applyKnockback && currentHealth > 0f)
        {
            Knockback();
        }

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Knockback()
    {
        knockback = true;
        knockbackStart = Time.time;
        rbAlive.velocity = new Vector2(0.0f, knockbackSpeedY);
    }

    private void CheckKnockBack()
    {
        bool isKnockBackOver = Time.time > knockbackStart + knockbackDuration;
        if (isKnockBackOver && knockback)
        {
            knockback = false;
            rbAlive.velocity = new Vector2(0.0f, rbAlive.velocity.y);

        }
    }

    private void Die()
    {
        aliveGO.SetActive(false);
        brokenTopGO.SetActive(true);
        brokenBotGO.SetActive(true);

        brokenTopGO.transform.position = aliveGO.transform.position;
        brokenBotGO.transform.rotation = aliveGO.transform.rotation;

        isDead = true;
        rbBrokenBot.velocity = new Vector2(knockbackSpeedX * playerFacingDirection, knockbackSpeedY);
        rbBrokenTop.velocity = new Vector2(knockbackDeadSpeedX * playerFacingDirection, knockbackDeadSpeedY);
        rbBrokenTop.AddTorque(deadTorque * -playerFacingDirection,ForceMode2D.Impulse);

    }

    
}

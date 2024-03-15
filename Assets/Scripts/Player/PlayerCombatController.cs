using System;
using System.Collections;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour, IDamagable
{
    public int noOfClick;
    private int direction;
    public int currentHealth;
    public int newHealth;
    public const int maxHealth = 100;
    private int amount = 10;

    private bool isAttaking;
    private bool isClickMouse;
    public bool isPlayerDead = false;


    [SerializeField]
    private float attack1Radius, attack1Damage;
    private float lastClickTime = 0;
    private float maxComboDelay = 1f;
    private float nextAttackTime;
    private float cooldownTime = 0.5f;
    private float nextAttackComboTime = 0.5f;
    private float timeAttackComboAllow = 0.2f;
    private float timeDelayCombo = 0.05f;
    private float timeComboClick = 0.3f;
    private float timePlayerDead;
    private float timePlayerAnimDead = 2.8f;


    //private float[] attackDetails = new float[2];


    private Animator anim;
    [SerializeField]
    private Transform attack1HitBoxPos;
    [SerializeField]
    private LayerMask whatisDamageable;
    private RaycastHit2D[] hits;
    private DemonAxeController demonaxe;
    private PlayerController player;
    private GameManager gamemanager;
    private HealthBar healthbar;
    public Action OnHealthChanged;
    public GameObject playerClone;



    private void Start()
    {
        anim = GetComponent<Animator>();
        demonaxe = FindObjectOfType<DemonAxeController>();
        player = FindObjectOfType<PlayerController>();
        currentHealth = maxHealth;
        newHealth = maxHealth;
        gamemanager = GameObject.Find("GameManager").GetComponent<GameManager>();

    }

    private void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartCoroutine(TimeOfClick());
            }
        }
        CheckAttack();

        if ((Time.time - lastClickTime) > maxComboDelay || noOfClick == 4)
        {
            noOfClick = 0;
        }


    }

    IEnumerator TimeOfClick()
    {

        lastClickTime = Time.time;
        noOfClick++;
        if (noOfClick == 1)
        {
            anim.SetBool("attack1", true);
        }
        yield return new WaitForSeconds(timeDelayCombo + timeComboClick);

        if (noOfClick >= 2 && nextAttackComboTime > 0 && isAttaking)
        {
            anim.SetBool("attack1", false);
            anim.SetBool("attack2", true);
        }

        yield return new WaitForSeconds(timeDelayCombo + timeComboClick);

        if (noOfClick >= 3 && nextAttackComboTime > 0 && isAttaking)
        {
            anim.SetBool("attack2", false);
            anim.SetBool("attack3", true);
        }

    }

    private void CheckAttack()
    {
        if (nextAttackComboTime > 0) // Dieu kien cho don danh tiep theo
        {
            isAttaking = true;
            nextAttackComboTime -= Time.deltaTime; // Giam thoi gian cua don danh
        }
        else if (nextAttackComboTime <= 0) // Neu nhu thoi gian cua don danh nho hon khong
        {
            nextAttackComboTime = cooldownTime; // Thoi gian cua don danh tiep theo tuong duong voi thoi gian hoi chieu
        }
    }

    private void CheckAttackHitBox()
    {
        hits = Physics2D.CircleCastAll(attack1HitBoxPos.position, attack1Radius, transform.right, 0f, whatisDamageable);

        foreach (RaycastHit2D hit in hits)
        {
            IDamagable iDamageable = hit.collider.GetComponent<IDamagable>();
            iDamageable ??= hit.collider.GetComponentInParent<IDamagable>();

            if (iDamageable != null)
            {
                iDamageable.Damage(new DamageInfo()
                {
                    DamageAmount = amount,
                    DamageSource = this.gameObject,
                });

            }
        }
    }

    public void Damage(DamageInfo damageInfo)
    {
        if (damageInfo.DamageAmount <= 0)
        {
            return;
        }
        else if(damageInfo.DamageAmount > 0 && !player.GetDashingState())
        {
            currentHealth -= damageInfo.DamageAmount;
            OnHealthChanged?.Invoke();
        }

        if (currentHealth == 0)
        {
            StartCoroutine(PlayerDeath());
        }


        if (!player.GetDashingState() && currentHealth > 0)
        {

            if (damageInfo.DamageSource.transform.position.x <= player.transform.position.x)
            {
                direction = 1;
            }

            else if (damageInfo.DamageSource.transform.position.x >= player.transform.position.x)
            {
                direction = -1;
            }
            player.KnockBack(direction);
        }

    }

    IEnumerator PlayerDeath()
    {
        isPlayerDead = true;
        timePlayerDead = Time.time;
        anim.SetTrigger("playerDead");
        yield return new WaitForSeconds(2.05f);
        Destroy(gameObject);
        gamemanager.Respawn();
        if (gamemanager.isRespawn)
        {
            OnHealthChanged?.Invoke();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attack1HitBoxPos.position, attack1Radius);
    }
}

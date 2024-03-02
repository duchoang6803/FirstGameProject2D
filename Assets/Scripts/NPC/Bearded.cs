using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Analytics;

public class Bearded : MonoBehaviour
{
    //private Rigidbody2D rb_beard;
    private Animator anim;

    [SerializeField]
    private float beardMoveSpeed, rotateSpeed;


    private bool isWalking;
    private bool isWandering = false;

    private int rotateDirection = 1;


    private void Start()
    {
        anim = GetComponent<Animator>();

    }
    private void Update()
    {
        if (isWandering == false)
        {
            StartCoroutine(NPCWalking());
        }
        if (isWalking)
        {
            NPCMovement();
        }
    }

    public void NPCMovement()
    {
        if (rotateDirection == 1)
        {
            transform.Translate(rotateDirection * beardMoveSpeed * Time.deltaTime, 0, 0);
        }
        else if (rotateDirection == -1)
        {
            transform.Translate(-rotateDirection * beardMoveSpeed * Time.deltaTime, 0, 0);
        }
    }

    IEnumerator NPCWalking()
    {
        isWandering = true;
        int waitToWalk = Random.Range(1, 2);
        int walkTime = Random.Range(2, 3);
        int rotationTime = Random.Range(0, 1);
        anim.SetBool("isWalking", false);
        yield return new WaitForSeconds(waitToWalk);
        isWalking = true;
        anim.SetBool("isWalking", true);
        yield return new WaitForSeconds(walkTime);
        isWalking = false;
        yield return new WaitForSeconds(rotationTime);
        Flip();
        isWandering = false;
    }

    private void Flip()
    {
        rotateDirection *= -1;
        transform.Rotate(0, 180f, 0);
    }


}

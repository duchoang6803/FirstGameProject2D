using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using Cinemachine;

public class GameManager : MonoBehaviour
{

    public Transform respawnPoint;

    [SerializeField]

    public GameObject player;
    [SerializeField]

    private float respondTime;

    private CinemachineVirtualCamera cinemachine;
    private HealthBar healthBar;
    private PlayerCombatController _player;

    private float respondStartTime;

    public bool isRespawn;

    private void Start()
    {
        cinemachine = GameObject.Find("Player Camera").GetComponent<CinemachineVirtualCamera>();
        _player = GameObject.Find("player").GetComponent<PlayerCombatController>();
    }

    private void Update()
    {
        CheckRespawn();
    }
    public void Respawn()
    {
        respondStartTime = Time.time; // 15
        isRespawn = true;

    }

    private void CheckRespawn()
    {
        bool respond =  Time.time >= respondStartTime + respondTime; // 15 + 4
        if(respond && isRespawn)
        {
            var respawnPlayer = Instantiate(player, respawnPoint);
            cinemachine.m_Follow = respawnPlayer.transform;
            isRespawn=false;
        }
    }
}

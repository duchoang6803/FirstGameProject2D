using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Transform respondPoint;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private float respondTime;

    private CinemachineVirtualCamera cinemachine;
    private PlayerCombatController _player;

    private float respondStartTime;

    private bool isRespond;

    private void Start()
    {
        cinemachine = GameObject.Find("Player Camera").GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        CheckRespond();
    }
    public void Respond()
    {
        respondStartTime = Time.time; // 15
        isRespond = true;

    }

    private void CheckRespond()
    {
        bool respond =  Time.time >= respondStartTime + respondTime; // 15 + 4
        if(respond && isRespond)
        {
            var respondPlayer = Instantiate(player, respondPoint);
            cinemachine.m_Follow = respondPlayer.transform;
            isRespond=false;
        }
    }
}

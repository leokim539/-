using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerTp : MonoBehaviourPunCallbacks
{
    [Header("순간이동위치")]
    public Transform teleportLocation; // 순간이동할 위치
    public float stayDuration = 2f; // 머무는 시간
    private bool isInZone = false; // 플레이어가 특정 지역에 있는지 여부
    private float stayTimer = 0f; // 머무는 시간 카운터

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInZone = true;
            stayTimer = 0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInZone = false;
            stayTimer = 0f;
        }
    }

    private void Update()
    {
        if (isInZone)
        {
            stayTimer += Time.deltaTime;

            if (stayTimer >= stayDuration)
            {
                TeleportPlayerToLocation();
            }
        }
    }

    private void TeleportPlayerToLocation()
    {
        photonView.RPC("Teleport", RpcTarget.All);
    }

    [PunRPC]
    private void Teleport()
    {
        transform.position = teleportLocation.position;
    }
}

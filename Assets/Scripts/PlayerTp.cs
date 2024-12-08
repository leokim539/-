using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerTp : MonoBehaviourPunCallbacks
{
    [Header("�����̵���ġ")]
    public Transform teleportLocation; // �����̵��� ��ġ
    public float stayDuration = 2f; // �ӹ��� �ð�
    private bool isInZone = false; // �÷��̾ Ư�� ������ �ִ��� ����
    private float stayTimer = 0f; // �ӹ��� �ð� ī����

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

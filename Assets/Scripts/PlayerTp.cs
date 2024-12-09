using Photon.Pun;
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
            Debug.Log("Player entered the teleport zone");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInZone = false;
            stayTimer = 0f;
            Debug.Log("Player exited the teleport zone");
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
        if (photonView == null)
        {
            Debug.LogError("PhotonView is missing!");
            return;
        }

        Debug.Log("Calling RPC to teleport player");
        photonView.RPC("Teleport", RpcTarget.All);
    }

    [PunRPC]
    private void Teleport()
    {
        if (teleportLocation == null)
        {
            Debug.LogError("Teleport location is not set!");
            return;
        }

        transform.position = teleportLocation.position;
        Debug.Log("Player teleported to location");
    }
}

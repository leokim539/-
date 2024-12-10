using Photon.Pun;
using UnityEngine;

public class PlayerTp : MonoBehaviourPunCallbacks
{
    [Header("순간이동위치")] 
    public Transform portal1; // 포탈 A 위치
    public Transform portal2;
    public float stayDuration = 2f; // 머무는 시간
    private bool isInZone = false; // 플레이어가 특정 지역에 있는지 여부
    private float stayTimer = 0f; // 머무는 시간 카운터
    private PhotonView playerPhotonView; // 순간이동할 플레이어의 PhotonView

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInZone = true;
            stayTimer = 0f;
            playerPhotonView = other.GetComponent<PhotonView>(); // 충돌한 플레이어의 PhotonView 저장
            if (playerPhotonView != null)
            {
                // 현재 위치에 따라 포탈 A 또는 B로 순간이동
                if (Vector3.Distance(other.transform.position, portal1.position) < 1f)
                {
                    playerPhotonView.RPC("TeleportTo", RpcTarget.All, portal2.position);
                }
                else if (Vector3.Distance(other.transform.position, portal2.position) < 1f)
                {
                    playerPhotonView.RPC("TeleportTo", RpcTarget.All, portal1.position);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInZone = false;
            stayTimer = 0f;
            playerPhotonView = null; // 플레이어가 나가면 PhotonView 초기화
            Debug.Log("Player exited the teleport zone");
        }
    }

    private void Update()
    {
        if (isInZone && playerPhotonView != null)
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
        if (playerPhotonView == null)
        {
            Debug.LogError("Player's PhotonView is missing!");
            return;
        }

        Debug.Log("Calling RPC to teleport player");
        playerPhotonView.RPC("Teleport", RpcTarget.All);
    }
}
using Photon.Pun;
using UnityEngine;

public class PlayerTp : MonoBehaviourPunCallbacks
{
    [Header("순간이동위치")] 
    public Transform portal1; // 포탈 A 위치
    public Transform portal2;
<<<<<<< HEAD
    public float stayDuration = 1f; // 머무는 시간
=======
    public float stayDuration = 2f; // 머무는 시간
>>>>>>> 3d789fa3b5b5808e3c708510a1bced69630792a3
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
            Debug.Log("Player entered the teleport zone");
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

        // 현재 위치에 따라 포탈 A 또는 B로 순간이동
        Vector3 targetPosition = Vector3.Distance(playerPhotonView.transform.position, portal1.position) < 1f ? portal2.position : portal1.position;

        Debug.Log("Calling RPC to teleport player to: " + targetPosition);
        playerPhotonView.RPC("TeleportTo", RpcTarget.All, targetPosition);

        // 이동 후 상태 초기화
        isInZone = false;
        stayTimer = 0f;
    }
}
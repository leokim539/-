using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class FloorTP : MonoBehaviourPunCallbacks
{
    public GameObject triggerObject;
    public GameObject targetObject;
    public GameObject FUI;
    private PhotonView photonView;
    private bool isLocalPlayerInTrigger = false;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        // 모든 클라이언트에서 UI 비활성화
        FUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        // 오직 로컬 플레이어에 대해서만 UI 활성화
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            isLocalPlayerInTrigger = true;
            FUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        // 오직 로컬 플레이어에 대해서만 UI 비활성화
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            isLocalPlayerInTrigger = false;
            FUI.SetActive(false);
        }
    }

    void Update()
    {
        // 오직 로컬 플레이어의 트리거 상태에서만 F키 입력 처리
        if (isLocalPlayerInTrigger && Input.GetKeyDown(KeyCode.F))
        {
            // 현재 클라이언트의 플레이어 정보 가져오기
            GameObject localPlayer = GameObject.FindGameObjectWithTag("Player");
            if (localPlayer != null)
            {
                PhotonView playerPhotonView = localPlayer.GetComponent<PhotonView>();

                // 로컬 플레이어일 때만 마스터 클라이언트에게 텔레포트 요청
                if (playerPhotonView.IsMine)
                {
                    photonView.RPC("HandleTeleportRequest", RpcTarget.MasterClient, playerPhotonView.ViewID);
                }
            }
        }
    }

    [PunRPC]
    void HandleTeleportRequest(int playerViewID)
    {
        // 마스터 클라이언트만 실행
        if (PhotonNetwork.IsMasterClient)
        {
            // 텔레포트 명령을 요청한 특정 플레이어에게만 전달
            photonView.RPC("PerformTeleport", RpcTarget.All, playerViewID);
        }
    }

    [PunRPC]
    void PerformTeleport(int playerViewID)
    {
        // 특정 플레이어 ID로 플레이어 찾기
        PhotonView targetPlayerView = PhotonView.Find(playerViewID);

        if (targetPlayerView != null && targetPlayerView.IsMine)
        {
            // 오직 해당 플레이어만 이동
            targetPlayerView.gameObject.transform.position = targetObject.transform.position;
        }
    }
}
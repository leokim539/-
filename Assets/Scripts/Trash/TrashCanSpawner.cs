using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TrashCanSpawner : MonoBehaviourPunCallbacks
{
    public GameObject trashCanPrefab;
    public float spawnInterval = 30f;
    public Transform spawnPointsParent;
    public GameObject warningUI;

    private GameObject currentTrashCan;

    public void Start()
    {
        // 마스터 클라이언트에서만 TrashCan 생성
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(SpawnTrashCanCoroutine());
            StartCoroutine(MoveTrashCan());
        }
    }

    public IEnumerator SpawnTrashCanCoroutine()
    {
        // 스폰 TrashCan
        photonView.RPC("SpawnTrashCan", RpcTarget.All);
        yield return null;
    }

    [PunRPC]
    public void SpawnTrashCan()
    {
        // 이미 TrashCan이 존재하는 경우, 새로 생성하지 않음
        if (currentTrashCan != null)
        {
            Debug.Log("TrashCan already exists. Not spawning a new one.");
            return;
        }

        Vector3 spawnPosition = GetRandomSpawnPosition();
        Debug.Log("Spawning TrashCan at: " + spawnPosition);

        // TrashCan 인스턴스화 - 주요 변경 부분
        if (PhotonNetwork.IsMasterClient)
        {
            currentTrashCan = PhotonNetwork.Instantiate(trashCanPrefab.name, spawnPosition, Quaternion.identity);

            // 트래시캔 활성화 확인 및 강제 활성화
            if (currentTrashCan != null)
            {
                PhotonView photonView = currentTrashCan.GetComponent<PhotonView>();
                if (photonView != null)
                {
                    photonView.RPC("SetActiveRPC", RpcTarget.All, true);
                }
            }
        }
    }

    // 새로 추가된 RPC 메서드
    [PunRPC]
    public void SetActiveRPC(bool active)
    {
        if (currentTrashCan != null)
        {
            currentTrashCan.SetActive(active);
            Debug.Log($"TrashCan active state set to: {active}");
        }
    }

    public IEnumerator MoveTrashCan()
    {
        while (true)
        {
            // 10초 전에 UI 활성화
            yield return new WaitForSeconds(spawnInterval - 10f);

            // 모든 클라이언트에서 UI 활성화
            photonView.RPC("ShowWarningUIRPC", RpcTarget.All);

            // 지정된 시간 동안 대기
            yield return new WaitForSeconds(5f);

            // 모든 클라이언트에서 UI 비활성화
            photonView.RPC("HideWarningUIRPC", RpcTarget.All);

            // 랜덤 위치로 TrashCan 이동 (마스터 클라이언트에서만 실행)
            if (PhotonNetwork.IsMasterClient)
            {
                MoveToRandomPosition();
            }
        }
    }

    // UI 관련 RPC 메서드 추가
    [PunRPC]
    private void ShowWarningUIRPC()
    {
        warningUI.SetActive(true);
    }

    [PunRPC]
    private void HideWarningUIRPC()
    {
        warningUI.SetActive(false);
    }

    public void MoveToRandomPosition()
    {
        // SpawnPoints의 자식 오브젝트를 가져옴
        Transform[] spawnPoints = spawnPointsParent.GetComponentsInChildren<Transform>();

        // 첫 번째 자식(빈 게임 오브젝트 자신)을 제외
        List<Transform> validSpawnPoints = new List<Transform>();
        for (int i = 1; i < spawnPoints.Length; i++)
        {
            validSpawnPoints.Add(spawnPoints[i]);
        }

        // 유효한 스폰 포인트가 있을 때 랜덤 선택
        if (validSpawnPoints.Count > 0)
        {
            int randomIndex = Random.Range(0, validSpawnPoints.Count);
            Vector3 newPosition = validSpawnPoints[randomIndex].position;

            // TrashCan 이동 (마스터 클라이언트에서 모든 클라이언트에 RPC 호출)
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("MoveTrashCanRPC", RpcTarget.All, newPosition);
            }
        }
        else
        {
            Debug.LogWarning("No valid spawn points available!");
        }
    }

    [PunRPC]
    public void MoveTrashCanRPC(Vector3 newPosition)
    {
        // 모든 클라이언트에서 TrashCan을 새로운 위치로 이동
        if (currentTrashCan != null)
        {
            // PhotonView를 통해 네트워크 동기화 보장
            PhotonView photonView = currentTrashCan.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                currentTrashCan.transform.position = newPosition;
            }

            Debug.Log("TrashCan moved to: " + newPosition);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        // SpawnPoints의 자식 오브젝트를 가져옴
        Transform[] spawnPoints = spawnPointsParent.GetComponentsInChildren<Transform>();

        // 첫 번째 자식(빈 게임 오브젝트 자신)을 제외
        List<Transform> validSpawnPoints = new List<Transform>();
        for (int i = 1; i < spawnPoints.Length; i++)
        {
            validSpawnPoints.Add(spawnPoints[i]);
        }

        // 유효한 스폰 포인트가 있을 때 랜덤 선택
        if (validSpawnPoints.Count > 0)
        {
            int randomIndex = Random.Range(0, validSpawnPoints.Count);
            return validSpawnPoints[randomIndex].position;
        }

        Debug.LogError("No valid spawn points found! Returning Vector3.zero."); // 유효한 스폰 포인트가 없을 때 에러 메시지
        return Vector3.zero; // 기본값
    }

    private void ShowWarningUI()
    {
        warningUI.SetActive(true); // UI 활성화
    }

    private void HideWarningUI()
    {
        warningUI.SetActive(false); // UI 비활성화
    }
}
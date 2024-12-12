using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TrashCanSpawner : MonoBehaviourPunCallbacks
{
    public GameObject trashCan; // 씬에 이미 존재하는 트래시 캔
    public float spawnInterval = 30f;
    public Transform spawnPointsParent;
    public GameObject warningUI;

    public void Start()
    {
        // 마스터 클라이언트에서만 MoveTrashCan 코루틴 시작
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(MoveTrashCan());
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
            photonView.RPC("MoveTrashCanRPC", RpcTarget.All, newPosition);
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
        if (trashCan != null)
        {
            trashCan.transform.position = newPosition;
            Debug.Log("TrashCan moved to: " + newPosition);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 관련 네임스페이스 추가
using Photon.Pun; // 포톤 관련 네임스페이스 추가

public class TrashCanSpawner : MonoBehaviourPunCallbacks // PhotonBehaviour 상속
{
    public GameObject trashCanPrefab; // TrashCan 프리팹
    public float spawnInterval = 30f; // 스폰 간격 (30초)
    public Transform spawnPointsParent; // 스폰 포인트를 가진 빈 게임 오브젝트
    public GameObject warningUI; // 경고 UI 객체

    private GameObject currentTrashCan; // 현재 존재하는 TrashCan

    public void Start()
    {
        // 마스터 클라이언트에서만 TrashCan 생성
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(SpawnTrashCanCoroutine());
            StartCoroutine(MoveTrashCan());
        }
    }

    private IEnumerator SpawnTrashCanCoroutine()
    {
        // 스폰 TrashCan
        photonView.RPC("SpawnTrashCan", RpcTarget.All);
        yield return null; // 다음 프레임으로 넘어감
    }

    [PunRPC]
    private void SpawnTrashCan()
    {
        // 이미 TrashCan이 존재하는 경우, 새로 생성하지 않음
        if (currentTrashCan != null)
        {
            Debug.Log("TrashCan already exists. Not spawning a new one.");
            return;
        }

        Vector3 spawnPosition = GetRandomSpawnPosition();
        Debug.Log("Spawning TrashCan at: " + spawnPosition); // 스폰 위치 디버깅

        // TrashCan 인스턴스화
        currentTrashCan = PhotonNetwork.Instantiate(trashCanPrefab.name, spawnPosition, Quaternion.identity);

        // 생성된 TrashCan의 활성 상태 확인
        if (currentTrashCan != null)
        {
            Debug.Log("TrashCan spawned successfully. Active: " + currentTrashCan.activeSelf);
        }
        else
        {
            Debug.LogError("Failed to spawn TrashCan. CurrentTrashCan is null.");
        }
    }


    public IEnumerator MoveTrashCan()
    {
        while (true)
        {
            // 10초 전에 UI 활성화
            yield return new WaitForSeconds(spawnInterval - 10f);
            ShowWarningUI();

            // 지정된 시간 동안 대기
            yield return new WaitForSeconds(5f);
            HideWarningUI();

            // 랜덤 위치로 TrashCan 이동
            MoveToRandomPosition();
        }
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

            // TrashCan 이동
            Debug.Log("Moving TrashCan to: " + newPosition); // 이동 위치 디버깅
            photonView.RPC("MoveTrashCanRPC", RpcTarget.All, newPosition); // 모든 클라이언트에서 이동
        }
        else
        {
            Debug.LogWarning("No valid spawn points available!"); // 유효한 스폰 포인트가 없을 때 경고
        }
    }

    [PunRPC]
    public void MoveTrashCanRPC(Vector3 newPosition)
    {
        // 모든 클라이언트에서 TrashCan을 새로운 위치로 이동
        if (currentTrashCan != null)
        {
            currentTrashCan.transform.position = newPosition;
            Debug.Log("TrashCan moved to: " + newPosition); // 이동한 위치 로그
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

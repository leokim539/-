using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 관련 네임스페이스 추가

public class TrashCanSpawner : MonoBehaviour
{
    public GameObject trashCanPrefab; // TrashCan 프리팹
    public float spawnInterval = 30f; // 스폰 간격 (30초)
    public Transform spawnPointsParent; // 스폰 포인트를 가진 빈 게임 오브젝트
    public GameObject warningUI; // 경고 UI 객체

    private GameObject currentTrashCan; // 현재 존재하는 TrashCan

    public void Start()
    {
        // TrashCan 생성
        currentTrashCan = Instantiate(trashCanPrefab, GetRandomSpawnPosition(), Quaternion.identity);

        // 코루틴 시작
        StartCoroutine(MoveTrashCan());
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
            currentTrashCan.transform.position = newPosition;
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

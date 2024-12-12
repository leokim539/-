using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TrashSpawner : MonoBehaviourPunCallbacks
{
    public BeerCanObjectData[] beerCanTypes; // BeerCanObjectData 배열
    public PetBottleObjectData[] petBottleTypes; // PetBottleObjectData 배열
    public TrashBagObjectData[] trashBagTypes; // TrashBagObjectData 배열
    public int objectsPerType = 5; // 각 타입당 생성할 객체 수
    public Transform spawnPointsParent; // 스폰 포인트 부모 객체

    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>(); // 이미 점유된 위치
    private bool hasSpawned = false; // 스폰 상태

    void Start()
    {
        // 마스터 클라이언트에서만 스폰 실행
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnObjects();
        }
    }

    void SpawnObjects()
    {
        // 이미 스폰된 경우 종료
        if (hasSpawned)
        {
            Debug.Log("스폰 이미 완료됨. 종료.");
            return;
        }

        // 스폰 상태를 업데이트
        hasSpawned = true; // 스폰 상태 업데이트
        Debug.Log("스폰 상태 업데이트: hasSpawned = true");

        // 스폰 포인트 가져오기
        Transform[] spawnPoints = spawnPointsParent.GetComponentsInChildren<Transform>();
        List<Transform> validSpawnPoints = new List<Transform>();

        for (int i = 1; i < spawnPoints.Length; i++) // 0번 인덱스는 부모 객체일 가능성이 있으므로 제외
        {
            validSpawnPoints.Add(spawnPoints[i]);
        }

        // 각 타입에 대해 오브젝트 생성
        SpawnBeerCans(validSpawnPoints);
        SpawnPetBottles(validSpawnPoints);
        SpawnTrashBags(validSpawnPoints);

        Debug.Log("스폰 객체 생성 완료");
    }

    void SpawnBeerCans(List<Transform> validSpawnPoints)
    {
        foreach (var beerCan in beerCanTypes)
        {
            for (int i = 0; i < objectsPerType; i++)
            {
                Vector3 spawnPosition = GetUniqueSpawnPosition(validSpawnPoints);
                if (spawnPosition == Vector3.zero) break; // 유효한 위치가 없으면 종료

                PhotonNetwork.Instantiate(beerCan.prefab.name, spawnPosition, Quaternion.identity, 0);
                occupiedPositions.Add(spawnPosition);
            }
        }
    }

    void SpawnPetBottles(List<Transform> validSpawnPoints)
    {
        foreach (var petBottle in petBottleTypes)
        {
            for (int i = 0; i < objectsPerType; i++)
            {
                Vector3 spawnPosition = GetUniqueSpawnPosition(validSpawnPoints);
                if (spawnPosition == Vector3.zero) break; // 유효한 위치가 없으면 종료

                PhotonNetwork.Instantiate(petBottle.prefab.name, spawnPosition, Quaternion.identity, 0);
                occupiedPositions.Add(spawnPosition);
            }
        }
    }

    void SpawnTrashBags(List<Transform> validSpawnPoints)
    {
        foreach (var trashBag in trashBagTypes)
        {
            for (int i = 0; i < objectsPerType; i++)
            {
                Vector3 spawnPosition = GetUniqueSpawnPosition(validSpawnPoints);
                if (spawnPosition == Vector3.zero) break; // 유효한 위치가 없으면 종료

                PhotonNetwork.Instantiate(trashBag.prefab.name, spawnPosition, Quaternion.identity, 0);
                occupiedPositions.Add(spawnPosition);
            }
        }
    }

    private Vector3 GetUniqueSpawnPosition(List<Transform> validSpawnPoints)
    {
        for (int attempt = 0; attempt < 10; attempt++)
        {
            int randomIndex = Random.Range(0, validSpawnPoints.Count);
            Vector3 spawnPosition = validSpawnPoints[randomIndex].position;

            if (!occupiedPositions.Contains(spawnPosition))
            {
                return spawnPosition;
            }
        }

        Debug.LogWarning("No valid unique spawn position found!");
        return Vector3.zero;
    }
}

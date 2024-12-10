using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TrashSpawner : MonoBehaviourPunCallbacks
{
    public BeerCanObjectData[] beerCanTypes;
    public PetBottleObjectData[] petBottleTypes;
    public TrashBagObjectData[] trashBagTypes;
    public int objectsPerType = 5;
    public Transform spawnPointsParent;

    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();
    private bool hasSpawned = false;

    void Start()
    {
        // 모든 클라이언트에서 스폰 실행
        SpawnObjects();
    }

    void SpawnObjects()
    {
        // 이미 스폰된 경우 종료
        if (hasSpawned)
        {
            Debug.Log("스폰 이미 완료됨. 종료.");
            return;
        }

        // 모든 클라이언트에게 스폰을 알리기 위해 RPC 호출
        photonView.RPC("RPC_SpawnObjects", RpcTarget.All);
    }

    [PunRPC]
    void RPC_SpawnObjects()
    {
        // 메서드 호출 로그
        Debug.Log("RPC_SpawnObjects 호출됨");

        // 이미 스폰된 경우 종료
        if (hasSpawned)
        {
            Debug.Log("이미 스폰됨. 종료.");
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
        SpawnTrash(beerCanTypes, validSpawnPoints);
        SpawnTrash(petBottleTypes, validSpawnPoints);
        SpawnTrash(trashBagTypes, validSpawnPoints);

        Debug.Log("RPC_SpawnObjects 실행 완료");
    }

    void SpawnTrash(BeerCanObjectData[] trashTypes, List<Transform> validSpawnPoints)
    {
        foreach (var trash in trashTypes)
        {
            for (int i = 0; i < objectsPerType; i++)
            {
                Vector3 spawnPosition = GetUniqueSpawnPosition(validSpawnPoints);
                if (spawnPosition == Vector3.zero) break; // 유효한 위치가 없으면 종료

                PhotonNetwork.Instantiate(trash.prefab.name, spawnPosition, Quaternion.identity, 0);
                occupiedPositions.Add(spawnPosition);
            }
        }
    }

    void SpawnTrash(PetBottleObjectData[] trashTypes, List<Transform> validSpawnPoints)
    {
        foreach (var trash in trashTypes)
        {
            for (int i = 0; i < objectsPerType; i++)
            {
                Vector3 spawnPosition = GetUniqueSpawnPosition(validSpawnPoints);
                if (spawnPosition == Vector3.zero) break; // 유효한 위치가 없으면 종료

                PhotonNetwork.Instantiate(trash.prefab.name, spawnPosition, Quaternion.identity, 0);
                occupiedPositions.Add(spawnPosition);
            }
        }
    }

    void SpawnTrash(TrashBagObjectData[] trashTypes, List<Transform> validSpawnPoints)
    {
        foreach (var trash in trashTypes)
        {
            for (int i = 0; i < objectsPerType; i++)
            {
                Vector3 spawnPosition = GetUniqueSpawnPosition(validSpawnPoints);
                if (spawnPosition == Vector3.zero) break; // 유효한 위치가 없으면 종료

                PhotonNetwork.Instantiate(trash.prefab.name, spawnPosition, Quaternion.identity, 0);
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

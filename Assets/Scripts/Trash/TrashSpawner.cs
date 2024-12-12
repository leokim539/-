using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TrashSpawner : MonoBehaviourPunCallbacks
{
    public BeerCanObjectData[] beerCanTypes; // BeerCanObjectData �迭
    public PetBottleObjectData[] petBottleTypes; // PetBottleObjectData �迭
    public TrashBagObjectData[] trashBagTypes; // TrashBagObjectData �迭
    public int objectsPerType = 5; // �� Ÿ�Դ� ������ ��ü ��
    public Transform spawnPointsParent; // ���� ����Ʈ �θ� ��ü

    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>(); // �̹� ������ ��ġ
    private bool hasSpawned = false; // ���� ����

    void Start()
    {
        // ������ Ŭ���̾�Ʈ������ ���� ����
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnObjects();
        }
    }

    void SpawnObjects()
    {
        // �̹� ������ ��� ����
        if (hasSpawned)
        {
            Debug.Log("���� �̹� �Ϸ��. ����.");
            return;
        }

        // ���� ���¸� ������Ʈ
        hasSpawned = true; // ���� ���� ������Ʈ
        Debug.Log("���� ���� ������Ʈ: hasSpawned = true");

        // ���� ����Ʈ ��������
        Transform[] spawnPoints = spawnPointsParent.GetComponentsInChildren<Transform>();
        List<Transform> validSpawnPoints = new List<Transform>();

        for (int i = 1; i < spawnPoints.Length; i++) // 0�� �ε����� �θ� ��ü�� ���ɼ��� �����Ƿ� ����
        {
            validSpawnPoints.Add(spawnPoints[i]);
        }

        // �� Ÿ�Կ� ���� ������Ʈ ����
        SpawnBeerCans(validSpawnPoints);
        SpawnPetBottles(validSpawnPoints);
        SpawnTrashBags(validSpawnPoints);

        Debug.Log("���� ��ü ���� �Ϸ�");
    }

    void SpawnBeerCans(List<Transform> validSpawnPoints)
    {
        foreach (var beerCan in beerCanTypes)
        {
            for (int i = 0; i < objectsPerType; i++)
            {
                Vector3 spawnPosition = GetUniqueSpawnPosition(validSpawnPoints);
                if (spawnPosition == Vector3.zero) break; // ��ȿ�� ��ġ�� ������ ����

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
                if (spawnPosition == Vector3.zero) break; // ��ȿ�� ��ġ�� ������ ����

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
                if (spawnPosition == Vector3.zero) break; // ��ȿ�� ��ġ�� ������ ����

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

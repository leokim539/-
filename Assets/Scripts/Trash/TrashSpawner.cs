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
        // ��� Ŭ���̾�Ʈ���� ���� ����
        SpawnObjects();
    }

    void SpawnObjects()
    {
        // �̹� ������ ��� ����
        if (hasSpawned)
        {
            Debug.Log("���� �̹� �Ϸ��. ����.");
            return;
        }

        // ��� Ŭ���̾�Ʈ���� ������ �˸��� ���� RPC ȣ��
        photonView.RPC("RPC_SpawnObjects", RpcTarget.All);
    }

    [PunRPC]
    void RPC_SpawnObjects()
    {
        // �޼��� ȣ�� �α�
        Debug.Log("RPC_SpawnObjects ȣ���");

        // �̹� ������ ��� ����
        if (hasSpawned)
        {
            Debug.Log("�̹� ������. ����.");
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
        SpawnTrash(beerCanTypes, validSpawnPoints);
        SpawnTrash(petBottleTypes, validSpawnPoints);
        SpawnTrash(trashBagTypes, validSpawnPoints);

        Debug.Log("RPC_SpawnObjects ���� �Ϸ�");
    }

    void SpawnTrash(BeerCanObjectData[] trashTypes, List<Transform> validSpawnPoints)
    {
        foreach (var trash in trashTypes)
        {
            for (int i = 0; i < objectsPerType; i++)
            {
                Vector3 spawnPosition = GetUniqueSpawnPosition(validSpawnPoints);
                if (spawnPosition == Vector3.zero) break; // ��ȿ�� ��ġ�� ������ ����

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
                if (spawnPosition == Vector3.zero) break; // ��ȿ�� ��ġ�� ������ ����

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
                if (spawnPosition == Vector3.zero) break; // ��ȿ�� ��ġ�� ������ ����

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

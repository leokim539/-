using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    public BeerCanObjectData[] beerCanTypes; // ���� ĵ Ÿ�� �迭
    public PetBottleObjectData[] petBottleTypes; // PET �� Ÿ�� �迭
    public TrashBagObjectData[] trashBagTypes; // ������ ���� Ÿ�� �迭
    public int objectsPerType = 5; // �� Ÿ�Դ� ������ ����
    public Transform spawnPointsParent; // ���� ����Ʈ�� ���� �� ���� ������Ʈ

    void Start()
    {
        SpawnObjects();
    }

    void SpawnObjects()
    {
        Transform[] spawnPoints = spawnPointsParent.GetComponentsInChildren<Transform>();
        List<Transform> validSpawnPoints = new List<Transform>();

        for (int i = 1; i < spawnPoints.Length; i++)
        {
            validSpawnPoints.Add(spawnPoints[i]);
        }

        // �� Ÿ�Կ� ���� ������Ʈ ����
        foreach (var beerCan in beerCanTypes)
        {
            for (int i = 0; i < objectsPerType; i++)
            {
                if (validSpawnPoints.Count == 0) break;

                int randomIndex = Random.Range(0, validSpawnPoints.Count);
                Vector3 spawnPosition = validSpawnPoints[randomIndex].position;

                Instantiate(beerCan.prefab, spawnPosition, Quaternion.identity);
                validSpawnPoints.RemoveAt(randomIndex);
            }
        }

        // PET �� ����
        foreach (var petBottle in petBottleTypes)
        {
            for (int i = 0; i < objectsPerType; i++)
            {
                if (validSpawnPoints.Count == 0) break;

                int randomIndex = Random.Range(0, validSpawnPoints.Count);
                Vector3 spawnPosition = validSpawnPoints[randomIndex].position;

                Instantiate(petBottle.prefab, spawnPosition, Quaternion.identity);
                validSpawnPoints.RemoveAt(randomIndex);
            }
        }

        // ������ ���� ����
        foreach (var trashBag in trashBagTypes)
        {
            for (int i = 0; i < objectsPerType; i++)
            {
                if (validSpawnPoints.Count == 0) break;

                int randomIndex = Random.Range(0, validSpawnPoints.Count);
                Vector3 spawnPosition = validSpawnPoints[randomIndex].position;

                Instantiate(trashBag.prefab, spawnPosition, Quaternion.identity);
                validSpawnPoints.RemoveAt(randomIndex);
            }
        }
    }
}

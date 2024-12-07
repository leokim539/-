using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    public BeerCanObjectData[] beerCanTypes; // 맥주 캔 타입 배열
    public PetBottleObjectData[] petBottleTypes; // PET 병 타입 배열
    public TrashBagObjectData[] trashBagTypes; // 쓰레기 봉투 타입 배열
    public int objectsPerType = 5; // 각 타입당 생성할 개수
    public Transform spawnPointsParent; // 스폰 포인트를 가진 빈 게임 오브젝트

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

        // 각 타입에 대해 오브젝트 생성
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

        // PET 병 생성
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

        // 쓰레기 봉투 생성
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

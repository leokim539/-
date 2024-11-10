using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    public TrashObjectData[] objectsToSpawn; // 생성할 오브젝트 배열
    public int numberOfObjects; // 생성할 오브젝트의 개수

    void Start()
    {
        SpawnObjects();
    }

    void SpawnObjects()
    {
        foreach (var obj in objectsToSpawn)
        {
            for (int i = 0; i < numberOfObjects; i++) // 각 오브젝트를 2개씩 생성
            {
                int randomIndex = Random.Range(0, obj.spawnPosition.Length);
                Vector3 spawnPosition = obj.spawnPosition[randomIndex];

                Instantiate(obj.prefab, spawnPosition, Quaternion.identity);
            }
        }
    }
}
                                                
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    public TrashObjectData[] objectsToSpawn; // ������ ������Ʈ �迭
    public int numberOfObjects; // ������ ������Ʈ�� ����

    void Start()
    {
        SpawnObjects();
    }

    void SpawnObjects()
    {
        foreach (var obj in objectsToSpawn)
        {
            for (int i = 0; i < numberOfObjects; i++) // �� ������Ʈ�� 2���� ����
            {
                int randomIndex = Random.Range(0, obj.spawnPosition.Length);
                Vector3 spawnPosition = obj.spawnPosition[randomIndex];

                Instantiate(obj.prefab, spawnPosition, Quaternion.identity);
            }
        }
    }
}
                                                
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTrashObjectData", menuName = "TrashObjectDatas/TrashObjectData")]
public class TrashObjectData : ScriptableObject
{
    public GameObject prefab; // 생성할 프리팹
    public Vector3[] spawnPosition; // 생성할 위치
}
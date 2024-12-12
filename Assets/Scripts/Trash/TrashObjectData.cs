using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTrashObjectData", menuName = "TrashObjectDatas/TrashObjectData")]
public class TrashObjectData : ScriptableObject
{
    public GameObject prefab; // ������ ������
    public Vector3[] spawnPosition; // ������ ��ġ
}
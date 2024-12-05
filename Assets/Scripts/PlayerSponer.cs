using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class PlayerSponer : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab; // �÷��̾� ������
    public Vector3[] spawnPositions; // ���� ��ġ �迭
    void Awake()
    {
        spawnPositions = new Vector3[]
        {
            new Vector3(-1, 2, 0),
            new Vector3(-1, 2, -1)
        };
    }
    void Start()
    {
        // ���� �ε�� �� �÷��̾� ��ȯ
        SpawnPlayers();
    }

    void SpawnPlayers()
    {
        if (spawnPositions == null || spawnPositions.Length == 0)
        {
            Debug.LogError("Spawn positions are not set or empty.");
            return;
        }

        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab is not assigned.");
            return;
        }

        if (PhotonNetwork.InRoom)
        {
            int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            Debug.Log("Current player count: " + playerCount);
            if (playerCount > 0 && playerCount <= spawnPositions.Length)
            {
                Vector3 spawnPosition = spawnPositions[playerCount - 1]; // �÷��̾� ���� ���� ���� ��ġ ����
                PhotonNetwork.Instantiate("Player", spawnPosition, Quaternion.identity);
            }
            else
            {
                Debug.LogError("Spawn position is null.");
            }
        }
        else
        {
            Debug.LogError("Not in a room.");
        }
    }
}
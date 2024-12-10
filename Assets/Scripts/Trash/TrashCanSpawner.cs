using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI; // UI ���� ���ӽ����̽� �߰�
using Photon.Pun; // ���� ���� ���ӽ����̽� �߰�

public class TrashCanSpawner : MonoBehaviourPunCallbacks
public class TrashCanSpawner : MonoBehaviourPunCallbacks // PhotonBehaviour ���
{
    public GameObject trashCanPrefab; // TrashCan ������
    public float spawnInterval = 30f; // ���� ���� (30��)
    public Transform spawnPointsParent; // ���� ����Ʈ�� ���� �� ���� ������Ʈ
    public GameObject warningUI; // ��� UI ��ü

    private GameObject currentTrashCan; // ���� �����ϴ� TrashCan

    public void Start()
    {
        // TrashCan ����
        SpawnTrashCan();
        // �ڷ�ƾ ����
        StartCoroutine(MoveTrashCan());
    }

    private IEnumerator SpawnTrashCanCoroutine()
    {
        // ���� TrashCan
        photonView.RPC("SpawnTrashCan", RpcTarget.All);
        yield return null; // ���� ���������� �Ѿ
    }

    [PunRPC]
    private void SpawnTrashCan()
    {
        // �̹� TrashCan�� �����ϴ� ���, ���� �������� ����
        if (currentTrashCan != null)
        {
            Debug.Log("TrashCan already exists. Not spawning a new one.");
            return;
        }

        Vector3 spawnPosition = GetRandomSpawnPosition();
        Debug.Log("Spawning TrashCan at: " + spawnPosition); // ���� ��ġ �����

        // TrashCan �ν��Ͻ�ȭ
        currentTrashCan = PhotonNetwork.Instantiate(trashCanPrefab.name, spawnPosition, Quaternion.identity);

        // ������ TrashCan�� Ȱ�� ���� Ȯ��
        if (currentTrashCan != null)
        {
            Debug.Log("TrashCan spawned successfully. Active: " + currentTrashCan.activeSelf);
        }
        else
        {
            Debug.LogError("Failed to spawn TrashCan. CurrentTrashCan is null.");
        }
    }


    public IEnumerator MoveTrashCan()
    {
        while (true)
        {
            // 10�� ���� UI Ȱ��ȭ
            yield return new WaitForSeconds(spawnInterval - 10f);
            ShowWarningUI();

            // ������ �ð� ���� ���
            yield return new WaitForSeconds(5f);
            HideWarningUI();

            // ���� ��ġ�� TrashCan �̵�
            MoveToRandomPosition();
        }
    }

    public void MoveToRandomPosition()
    {
        // SpawnPoints�� �ڽ� ������Ʈ�� ������
        Transform[] spawnPoints = spawnPointsParent.GetComponentsInChildren<Transform>();

        // ù ��° �ڽ�(�� ���� ������Ʈ �ڽ�)�� ����
        List<Transform> validSpawnPoints = new List<Transform>();
        for (int i = 1; i < spawnPoints.Length; i++)
        {
            validSpawnPoints.Add(spawnPoints[i]);
        }

        // ��ȿ�� ���� ����Ʈ�� ���� �� ���� ����
        if (validSpawnPoints.Count > 0)
        {
            int randomIndex = Random.Range(0, validSpawnPoints.Count);
            Vector3 newPosition = validSpawnPoints[randomIndex].position;

            // TrashCan �̵�
            if (currentTrashCan != null)
            {
                currentTrashCan.transform.position = newPosition;
            }
        }
    }
    public void SpawnTrashCan()
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        currentTrashCan = PhotonNetwork.Instantiate(trashCanPrefab.name, spawnPosition, Quaternion.identity, 0);
    }
    private Vector3 GetRandomSpawnPosition()
    {
        // SpawnPoints�� �ڽ� ������Ʈ�� ������
        Transform[] spawnPoints = spawnPointsParent.GetComponentsInChildren<Transform>();

        // ù ��° �ڽ�(�� ���� ������Ʈ �ڽ�)�� ����
        List<Transform> validSpawnPoints = new List<Transform>();
        for (int i = 1; i < spawnPoints.Length; i++)
        {
            validSpawnPoints.Add(spawnPoints[i]);
        }

        // ��ȿ�� ���� ����Ʈ�� ���� �� ���� ����
        if (validSpawnPoints.Count > 0)
        {
            int randomIndex = Random.Range(0, validSpawnPoints.Count);
            return validSpawnPoints[randomIndex].position;
        }

        Debug.LogError("No valid spawn points found! Returning Vector3.zero."); // ��ȿ�� ���� ����Ʈ�� ���� �� ���� �޽���
        return Vector3.zero; // �⺻��
    }

    private void ShowWarningUI()
    {
        warningUI.SetActive(true); // UI Ȱ��ȭ
    }

    private void HideWarningUI()
    {
        warningUI.SetActive(false); // UI ��Ȱ��ȭ
    }
}

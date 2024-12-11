using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TrashCanSpawner : MonoBehaviourPunCallbacks
{
    public GameObject trashCanPrefab;
    public float spawnInterval = 30f;
    public Transform spawnPointsParent;
    public GameObject warningUI;

    private GameObject currentTrashCan;

    public void Start()
    {
        // ������ Ŭ���̾�Ʈ������ TrashCan ����
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(SpawnTrashCanCoroutine());
            StartCoroutine(MoveTrashCan());
        }
    }

    public IEnumerator SpawnTrashCanCoroutine()
    {
        // ���� TrashCan
        photonView.RPC("SpawnTrashCan", RpcTarget.All);
        yield return null;
    }

    [PunRPC]
    public void SpawnTrashCan()
    {
        // �̹� TrashCan�� �����ϴ� ���, ���� �������� ����
        if (currentTrashCan != null)
        {
            Debug.Log("TrashCan already exists. Not spawning a new one.");
            return;
        }

        Vector3 spawnPosition = GetRandomSpawnPosition();
        Debug.Log("Spawning TrashCan at: " + spawnPosition);

        // TrashCan �ν��Ͻ�ȭ - �ֿ� ���� �κ�
        if (PhotonNetwork.IsMasterClient)
        {
            currentTrashCan = PhotonNetwork.Instantiate(trashCanPrefab.name, spawnPosition, Quaternion.identity);

            // Ʈ����ĵ Ȱ��ȭ Ȯ�� �� ���� Ȱ��ȭ
            if (currentTrashCan != null)
            {
                PhotonView photonView = currentTrashCan.GetComponent<PhotonView>();
                if (photonView != null)
                {
                    photonView.RPC("SetActiveRPC", RpcTarget.All, true);
                }
            }
        }
    }

    // ���� �߰��� RPC �޼���
    [PunRPC]
    public void SetActiveRPC(bool active)
    {
        if (currentTrashCan != null)
        {
            currentTrashCan.SetActive(active);
            Debug.Log($"TrashCan active state set to: {active}");
        }
    }

    public IEnumerator MoveTrashCan()
    {
        while (true)
        {
            // 10�� ���� UI Ȱ��ȭ
            yield return new WaitForSeconds(spawnInterval - 10f);

            // ��� Ŭ���̾�Ʈ���� UI Ȱ��ȭ
            photonView.RPC("ShowWarningUIRPC", RpcTarget.All);

            // ������ �ð� ���� ���
            yield return new WaitForSeconds(5f);

            // ��� Ŭ���̾�Ʈ���� UI ��Ȱ��ȭ
            photonView.RPC("HideWarningUIRPC", RpcTarget.All);

            // ���� ��ġ�� TrashCan �̵� (������ Ŭ���̾�Ʈ������ ����)
            if (PhotonNetwork.IsMasterClient)
            {
                MoveToRandomPosition();
            }
        }
    }

    // UI ���� RPC �޼��� �߰�
    [PunRPC]
    private void ShowWarningUIRPC()
    {
        warningUI.SetActive(true);
    }

    [PunRPC]
    private void HideWarningUIRPC()
    {
        warningUI.SetActive(false);
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

            // TrashCan �̵� (������ Ŭ���̾�Ʈ���� ��� Ŭ���̾�Ʈ�� RPC ȣ��)
            if (PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("MoveTrashCanRPC", RpcTarget.All, newPosition);
            }
        }
        else
        {
            Debug.LogWarning("No valid spawn points available!");
        }
    }

    [PunRPC]
    public void MoveTrashCanRPC(Vector3 newPosition)
    {
        // ��� Ŭ���̾�Ʈ���� TrashCan�� ���ο� ��ġ�� �̵�
        if (currentTrashCan != null)
        {
            // PhotonView�� ���� ��Ʈ��ũ ����ȭ ����
            PhotonView photonView = currentTrashCan.GetComponent<PhotonView>();
            if (photonView != null && photonView.IsMine)
            {
                currentTrashCan.transform.position = newPosition;
            }

            Debug.Log("TrashCan moved to: " + newPosition);
        }
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
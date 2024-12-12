using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TrashCanSpawner : MonoBehaviourPunCallbacks
{
    public GameObject trashCan; // ���� �̹� �����ϴ� Ʈ���� ĵ
    public float spawnInterval = 30f;
    public Transform spawnPointsParent;
    public GameObject warningUI;

    public void Start()
    {
        // ������ Ŭ���̾�Ʈ������ MoveTrashCan �ڷ�ƾ ����
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(MoveTrashCan());
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
            photonView.RPC("MoveTrashCanRPC", RpcTarget.All, newPosition);
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
        if (trashCan != null)
        {
            trashCan.transform.position = newPosition;
            Debug.Log("TrashCan moved to: " + newPosition);
        }
    }
}

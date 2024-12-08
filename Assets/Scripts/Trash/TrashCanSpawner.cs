using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI ���� ���ӽ����̽� �߰�

public class TrashCanSpawner : MonoBehaviour
{
    public GameObject trashCanPrefab; // TrashCan ������
    public float spawnInterval = 30f; // ���� ���� (30��)
    public Transform spawnPointsParent; // ���� ����Ʈ�� ���� �� ���� ������Ʈ
    public GameObject warningUI; // ��� UI ��ü

    private GameObject currentTrashCan; // ���� �����ϴ� TrashCan

    public void Start()
    {
        // TrashCan ����
        currentTrashCan = Instantiate(trashCanPrefab, GetRandomSpawnPosition(), Quaternion.identity);

        // �ڷ�ƾ ����
        StartCoroutine(MoveTrashCan());
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
            currentTrashCan.transform.position = newPosition;
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

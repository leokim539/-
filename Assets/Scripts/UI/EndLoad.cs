using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLoad : MonoBehaviour
{
    private List<GameObject> playersInZone = new List<GameObject>();
    private SceneFader sceneFader; // SceneFader ����

    private void Start()
    {
        sceneFader = FindObjectOfType<SceneFader>(); // SceneFader ã��
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!playersInZone.Contains(other.gameObject))
            {
                playersInZone.Add(other.gameObject);
                Debug.Log("�÷��̾� �߰���: " + other.gameObject.name);
            }

            GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
            if (allPlayers.Length == playersInZone.Count)
            {
                Debug.Log("��� �÷��̾ �����߽��ϴ�. �� �ε� ��...");
                sceneFader.FadeToScene("Ending"); // �� ���̵� ȣ��
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playersInZone.Contains(other.gameObject))
            {
                playersInZone.Remove(other.gameObject);
                Debug.Log("�÷��̾� ���ŵ�: " + other.gameObject.name);
            }
        }
    }
}


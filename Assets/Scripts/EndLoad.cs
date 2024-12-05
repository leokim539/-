using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLoad : MonoBehaviour
{
    private List<GameObject> playersInZone = new List<GameObject>();
    private SceneFader sceneFader; // SceneFader 참조

    private void Start()
    {
        sceneFader = FindObjectOfType<SceneFader>(); // SceneFader 찾기
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!playersInZone.Contains(other.gameObject))
            {
                playersInZone.Add(other.gameObject);
                Debug.Log("플레이어 추가됨: " + other.gameObject.name);
            }

            GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");
            if (allPlayers.Length == playersInZone.Count)
            {
                Debug.Log("모든 플레이어가 접촉했습니다. 씬 로딩 중...");
                sceneFader.FadeToScene("Ending"); // 씬 페이드 호출
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
                Debug.Log("플레이어 제거됨: " + other.gameObject.name);
            }
        }
    }
}


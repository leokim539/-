using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TrashCanSpawner : MonoBehaviourPunCallbacks
{
    public GameObject trashCan; // 이동할 쓰레기통 오브젝트
    public float spawnInterval = 30f;
    public Transform spawnPointsParent;
    public GameObject warningUI;
    public AudioClip warningSound; // 경고 사운드 클립
    private AudioSource audioSource; // AudioSource 컴포넌트

    public void Start()
    {
        // AudioSource 컴포넌트를 가져옵니다.
        audioSource = GetComponent<AudioSource>();

        // 마스터 클라이언트일 경우 MoveTrashCan 코루틴 시작
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(MoveTrashCan());
        }
    }

    public IEnumerator MoveTrashCan()
    {
        while (true)
        {
            // 10초 전 UI 활성화
            yield return new WaitForSeconds(spawnInterval - 10f);

            // 사운드 재생
            photonView.RPC("PlayWarningSoundRPC", RpcTarget.All);

            // UI 활성화
            photonView.RPC("ShowWarningUIRPC", RpcTarget.All);

            // 5초 대기
            yield return new WaitForSeconds(5f);

            // UI 비활성화
            photonView.RPC("HideWarningUIRPC", RpcTarget.All);

            // 쓰레기통 이동
            if (PhotonNetwork.IsMasterClient)
            {
                MoveToRandomPosition();
            }
        }
    }

    // UI 표시 RPC 함수
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

    // 사운드 재생 RPC 함수
    [PunRPC]
    private void PlayWarningSoundRPC()
    {
        if (audioSource != null && warningSound != null)
        {
            audioSource.PlayOneShot(warningSound);
        }
    }

    public void MoveToRandomPosition()
    {
        // SpawnPoints에서 랜덤 위치 선택
        Transform[] spawnPoints = spawnPointsParent.GetComponentsInChildren<Transform>();

        List<Transform> validSpawnPoints = new List<Transform>();
        for (int i = 1; i < spawnPoints.Length; i++)
        {
            validSpawnPoints.Add(spawnPoints[i]);
        }

        // 유효한 위치가 있을 경우
        if (validSpawnPoints.Count > 0)
        {
            int randomIndex = Random.Range(0, validSpawnPoints.Count);
            Vector3 newPosition = validSpawnPoints[randomIndex].position;

            // TrashCan 이동 (RPC 호출)
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
        // 쓰레기통을 새로운 위치로 이동
        if (trashCan != null)
        {
            trashCan.transform.position = newPosition;
            Debug.Log("TrashCan moved to: " + newPosition);
        }
    }
}

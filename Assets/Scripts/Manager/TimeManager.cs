using System.Collections; // IEnumerator를 사용하기 위해 추가
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public Text timerText; // 시계
    public float totalTime; // 총 시간 (초 단위)
    private float remainingTime; // 줄어들 시간

    // 게임 종료 UI
    public GameObject GameEndUI;

    void Start()
    {
        remainingTime = totalTime; // 시간 초기화
        GameEndUI.SetActive(false); // 초기 상태에서 비활성화
    }

    void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime; // 시간 계산
            UpdateTimerDisplay(); // 시계 표시
        }
        else
        {
            remainingTime = 0; // 남은 시간을 0으로 설정
            timerText.color = Color.red; // 시간이 끝났을 때 색상 변경
            ShowGameEndUI(); // 게임 종료 UI 표시
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60); // 분 계산
        int seconds = Mathf.FloorToInt(remainingTime % 60); // 초 계산
        timerText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds); // 표시
    }

    void ShowGameEndUI()
    {
        GameEndUI.SetActive(true); // 게임 종료 UI 활성화
        StopPlayerMovement(); // 플레이어의 움직임 중지
        DetermineWinner(); // 승리자 결정
    }

    void StopPlayerMovement()
    {
        // "Player" 태그를 가진 모든 오브젝트 찾기
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            FirstPersonController controller = player.GetComponent<FirstPersonController>();
            if (controller != null)
            {
                controller.enabled = false; // FirstPersonController 스크립트 비활성화
                Debug.Log($"Stopped movement for player: {player.name}"); // 디버그 로그 추가
            }
            else
            {
                Debug.LogWarning($"No FirstPersonController found on player: {player.name}"); // 경고 로그 추가
            }
        }

        // 2초 후 Ending 씬 로드
        StartCoroutine(LoadEndingSceneAfterDelay(2f));
    }

    private IEnumerator LoadEndingSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 딜레이
        EndingScene endingScene = FindObjectOfType<EndingScene>(); // EndingScene 스크립트 찾기
        if (endingScene != null)
        {
            endingScene.LoadEndingScene(); // Ending 씬 로드
        }
        else
        {
            Debug.LogWarning("EndingScene script not found in the scene."); // 경고 로그 추가
        }
    }

    void DetermineWinner()
    {
        // 모든 플레이어의 TaskUIManager 찾기
        TaskUIManager[] players = FindObjectsOfType<TaskUIManager>();

        int player1Count = 0;
        int player2Count = 0;

        // 플레이어 수에 따라 점수 계산
        if (players.Length > 0)
        {
            player1Count = players[0].GetTotalTrashCount(); // 첫 번째 플레이어 점수

            if (players.Length > 1)
            {
                player2Count = players[1].GetTotalTrashCount(); // 두 번째 플레이어 점수
            }
        }

        // 승리자 결정 및 UI 처리
        if (player1Count > player2Count)
        {
            Debug.Log("Player 1 Wins!");
            // 승리 애니메이션 또는 UI 처리
        }
        else if (player1Count < player2Count)
        {
            Debug.Log("Player 2 Wins!");
            // 승리 애니메이션 또는 UI 처리
        }
        else
        {
            Debug.Log("It's a Draw!");
            // 무승부 처리
        }
    }
}

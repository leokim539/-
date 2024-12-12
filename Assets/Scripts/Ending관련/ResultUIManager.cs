using UnityEngine;
using TMPro;
using Photon.Pun;

public class ResultUIManager : MonoBehaviour
{
    public TextMeshProUGUI player1NameText;
    public TextMeshProUGUI player2NameText;
    public TextMeshProUGUI player1TotalText;
    public TextMeshProUGUI player2TotalText;

    public GameObject P1WinUI; // Player 1 승리 UI
    public GameObject P1LoseUI; // Player 1 패배 UI
    public GameObject P2WinUI; // Player 2 승리 UI
    public GameObject P2LoseUI; // Player 2 패배 UI
    public GameObject DrawUI; // Draw UI

    // 추가된 변수
    public GameObject player1WinObject; // Player 1 승리 오브젝트
    public GameObject player1LoseObject; // Player 1 패배 오브젝트
    public GameObject player2WinObject; // Player 2 승리 오브젝트
    public GameObject player2LoseObject; // Player 2 패배 오브젝트
    public GameObject player1DrawObject; // Player 1 무승부 오브젝트
    public GameObject player2DrawObject; // Player 2 무승부 오브젝트

    void Start()
    {
        UpdateResult();
        DetermineWinner(); // 승자 결정 메서드 호출
    }

    public void UpdateResult()
    {
        // PhotonNetwork.PlayerList를 사용하여 플레이어 정보를 가져옵니다.
        if (PhotonNetwork.PlayerList.Length >= 2)
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                var player = PhotonNetwork.PlayerList[i];

                // 플레이어 이름 설정
                if (i == 0)
                {
                    player1NameText.text = player.NickName;
                    player1TotalText.text = player.CustomProperties.ContainsKey("TotalTrashCount")
                        ? player.CustomProperties["TotalTrashCount"].ToString()
                        : "0"; // 기본값 0
                }
                else if (i == 1)
                {
                    player2NameText.text = player.NickName;
                    player2TotalText.text = player.CustomProperties.ContainsKey("TotalTrashCount")
                        ? player.CustomProperties["TotalTrashCount"].ToString()
                        : "0"; // 기본값 0
                }
            }
        }
        else
        {
            Debug.LogError("Not enough players in the game.");
        }
    }

    private void DetermineWinner()
    {
        int player1Total = int.Parse(player1TotalText.text);
        int player2Total = int.Parse(player2TotalText.text);

        // UI 초기화
        P1WinUI.SetActive(false);
        P1LoseUI.SetActive(false);
        P2WinUI.SetActive(false);
        P2LoseUI.SetActive(false);
        DrawUI.SetActive(false);
        player1DrawObject.SetActive(false);
        player2DrawObject.SetActive(false);

        // ResultAnimation 컴포넌트 찾기
        ResultAnimation resultAnimation = FindObjectOfType<ResultAnimation>();

        // 점수 비교
        if (player1Total > player2Total)
        {
            P1WinUI.SetActive(true);
            P2LoseUI.SetActive(true);

            // Player1 승리 애니메이션 설정
            if (resultAnimation != null)
            {
                resultAnimation.SetResult(true);
            }

            // Player1 승리 및 Player2 패배 오브젝트 활성화
            if (player1WinObject != null)
            {
                player1WinObject.SetActive(true);
            }

            if (player2LoseObject != null)
            {
                player2LoseObject.SetActive(true);
            }
        }
        else if (player1Total < player2Total)
        {
            P1LoseUI.SetActive(true);
            P2WinUI.SetActive(true);

            // Player2 승리 애니메이션 설정
            if (resultAnimation != null)
            {
                resultAnimation.SetResult(false);
            }

            // Player2 승리 및 Player1 패배 오브젝트 활성화
            if (player2WinObject != null)
            {
                player2WinObject.SetActive(true);
            }

            if (player1LoseObject != null)
            {
                player1LoseObject.SetActive(true);
            }
        }
        else
        {
            // 점수가 같을 경우
            DrawUI.SetActive(true);
            player1DrawObject.SetActive(true);
            player2DrawObject.SetActive(true);
            Debug.Log("It's a tie!");
        }
    }
}

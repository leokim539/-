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

    // 승자를 결정하고 UI 업데이트
    private void DetermineWinner()
    {
        int player1Total = int.Parse(player1TotalText.text);
        int player2Total = int.Parse(player2TotalText.text);

        // UI 초기화
        P1WinUI.SetActive(false);
        P1LoseUI.SetActive(false);
        P2WinUI.SetActive(false);
        P2LoseUI.SetActive(false);
        DrawUI.SetActive(false); // Draw UI 초기화

        // 점수 비교
        if (player1Total > player2Total)
        {
            P1WinUI.SetActive(true); // Player 1 승리 UI 활성화
            P2LoseUI.SetActive(true); // Player 2 패배 UI 활성화
        }
        else if (player1Total < player2Total)
        {
            P1LoseUI.SetActive(true); // Player 1 패배 UI 활성화
            P2WinUI.SetActive(true); // Player 2 승리 UI 활성화
        }
        else
        {
            DrawUI.SetActive(true); // Draw UI 활성화
            Debug.Log("It's a tie!"); // 동점 처리
        }
    }

    // PlayerInfo 배열을 이용한 업데이트 메서드
    public void UpdateResult(PlayerInfo[] playerInfos)
    {
        if (playerInfos != null && playerInfos.Length >= 2)
        {
            player1NameText.text = playerInfos[0].playerName;
            player1TotalText.text = playerInfos[0].totalTrashCount.ToString();

            player2NameText.text = playerInfos[1].playerName;
            player2TotalText.text = playerInfos[1].totalTrashCount.ToString();
        }
        else
        {
            Debug.LogError("Player info is null or insufficient.");
        }
    }
}

using UnityEngine;
using TMPro;
using Photon.Pun;

public class ResultUIManager : MonoBehaviour
{
    public TextMeshProUGUI player1NameText;
    public TextMeshProUGUI player2NameText;
    public TextMeshProUGUI player1TotalText;
    public TextMeshProUGUI player2TotalText;

    public GameObject P1WinUI; // Player 1 �¸� UI
    public GameObject P1LoseUI; // Player 1 �й� UI
    public GameObject P2WinUI; // Player 2 �¸� UI
    public GameObject P2LoseUI; // Player 2 �й� UI
    public GameObject DrawUI; // Draw UI

    void Start()
    {
        UpdateResult();
        DetermineWinner(); // ���� ���� �޼��� ȣ��
    }

    public void UpdateResult()
    {
        // PhotonNetwork.PlayerList�� ����Ͽ� �÷��̾� ������ �����ɴϴ�.
        if (PhotonNetwork.PlayerList.Length >= 2)
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                var player = PhotonNetwork.PlayerList[i];

                // �÷��̾� �̸� ����
                if (i == 0)
                {
                    player1NameText.text = player.NickName;
                    player1TotalText.text = player.CustomProperties.ContainsKey("TotalTrashCount")
                        ? player.CustomProperties["TotalTrashCount"].ToString()
                        : "0"; // �⺻�� 0
                }
                else if (i == 1)
                {
                    player2NameText.text = player.NickName;
                    player2TotalText.text = player.CustomProperties.ContainsKey("TotalTrashCount")
                        ? player.CustomProperties["TotalTrashCount"].ToString()
                        : "0"; // �⺻�� 0
                }
            }
        }
        else
        {
            Debug.LogError("Not enough players in the game.");
        }
    }

    // ���ڸ� �����ϰ� UI ������Ʈ
    private void DetermineWinner()
    {
        int player1Total = int.Parse(player1TotalText.text);
        int player2Total = int.Parse(player2TotalText.text);

        // UI �ʱ�ȭ
        P1WinUI.SetActive(false);
        P1LoseUI.SetActive(false);
        P2WinUI.SetActive(false);
        P2LoseUI.SetActive(false);
        DrawUI.SetActive(false); // Draw UI �ʱ�ȭ

        // ���� ��
        if (player1Total > player2Total)
        {
            P1WinUI.SetActive(true); // Player 1 �¸� UI Ȱ��ȭ
            P2LoseUI.SetActive(true); // Player 2 �й� UI Ȱ��ȭ
        }
        else if (player1Total < player2Total)
        {
            P1LoseUI.SetActive(true); // Player 1 �й� UI Ȱ��ȭ
            P2WinUI.SetActive(true); // Player 2 �¸� UI Ȱ��ȭ
        }
        else
        {
            DrawUI.SetActive(true); // Draw UI Ȱ��ȭ
            Debug.Log("It's a tie!"); // ���� ó��
        }
    }

    // PlayerInfo �迭�� �̿��� ������Ʈ �޼���
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

using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ResultUIManager : MonoBehaviour
{
    public Text player1NameText;
    public Text player2NameText;
    public Text player1TotalText;
    public Text player2TotalText;

    public GameObject P1WinUI; // Player 1 �¸� UI
    public GameObject P1LoseUI; // Player 1 �й� UI
    public GameObject P2WinUI; // Player 2 �¸� UI
    public GameObject P2LoseUI; // Player 2 �й� UI
    public GameObject DrawUI; // Draw UI

    // �߰��� ����
    public GameObject player1WinObject; // Player 1 �¸� ������Ʈ
    public GameObject player1LoseObject; // Player 1 �й� ������Ʈ
    public GameObject player2WinObject; // Player 2 �¸� ������Ʈ
    public GameObject player2LoseObject; // Player 2 �й� ������Ʈ
    public GameObject player1DrawObject; // Player 1 ���º� ������Ʈ
    public GameObject player2DrawObject; // Player 2 ���º� ������Ʈ

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

    private void DetermineWinner()
    {
        int player1Total = int.Parse(player1TotalText.text);
        int player2Total = int.Parse(player2TotalText.text);

        // UI �ʱ�ȭ
        P1WinUI.SetActive(false);
        P1LoseUI.SetActive(false);
        P2WinUI.SetActive(false);
        P2LoseUI.SetActive(false);
        DrawUI.SetActive(false);
        player1DrawObject.SetActive(false);
        player2DrawObject.SetActive(false);

        // ResultAnimation ������Ʈ ã��
        ResultAnimation resultAnimation = FindObjectOfType<ResultAnimation>();

        // ���� ��
        if (player1Total > player2Total)
        {
            P1WinUI.SetActive(true);
            P2LoseUI.SetActive(true);

            // Player1 �¸� �ִϸ��̼� ����
            if (resultAnimation != null)
            {
                resultAnimation.SetResult(true);
            }

            // Player1 �¸� �� Player2 �й� ������Ʈ Ȱ��ȭ
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

            // Player2 �¸� �ִϸ��̼� ����
            if (resultAnimation != null)
            {
                resultAnimation.SetResult(false);
            }

            // Player2 �¸� �� Player1 �й� ������Ʈ Ȱ��ȭ
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
            // ������ ���� ���
            DrawUI.SetActive(true);
            player1DrawObject.SetActive(true);
            player2DrawObject.SetActive(true);
            Debug.Log("It's a tie!");
        }
    }
}

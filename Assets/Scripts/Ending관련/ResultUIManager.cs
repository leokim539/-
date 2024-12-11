using UnityEngine;
using TMPro;
using Photon.Pun;

public class ResultUIManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI player1NameText;
    public TextMeshProUGUI player2NameText;
    public TextMeshProUGUI player1TotalText;
    public TextMeshProUGUI player2TotalText;

    public void UpdateResultFromTrashCounts(TrashCount[] trashCounts)
    {
        if (trashCounts == null || trashCounts.Length < 2)
        {
            Debug.LogError("Not enough TrashCount components found.");
            return;
        }

        // �÷��̾� �̸��� Photon �÷��̾� ��Ͽ��� ��������
        var players = PhotonNetwork.PlayerList;

        if (players.Length >= 2)
        {
            // ù ��° �÷��̾� ���� ������Ʈ
            player1NameText.text = players[0].NickName;
            player1TotalText.text = trashCounts[0].GetTotalTrashCount().ToString();

            // �� ��° �÷��̾� ���� ������Ʈ
            player2NameText.text = players[1].NickName;
            player2TotalText.text = trashCounts[1].GetTotalTrashCount().ToString();

            Debug.Log($"UI Updated: Player1 Trash = {player1TotalText.text}, Player2 Trash = {player2TotalText.text}");
        }
        else
        {
            Debug.LogError("Not enough players in the room.");
        }
    }

    void Start()
    {
        // �ʱ� UI ������Ʈ
        TrashCount[] trashCounts = FindObjectsOfType<TrashCount>();
        UpdateResultFromTrashCounts(trashCounts);
    }
}
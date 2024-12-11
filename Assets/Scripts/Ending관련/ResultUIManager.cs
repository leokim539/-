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

        // 플레이어 이름은 Photon 플레이어 목록에서 가져오기
        var players = PhotonNetwork.PlayerList;

        if (players.Length >= 2)
        {
            // 첫 번째 플레이어 정보 업데이트
            player1NameText.text = players[0].NickName;
            player1TotalText.text = trashCounts[0].GetTotalTrashCount().ToString();

            // 두 번째 플레이어 정보 업데이트
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
        // 초기 UI 업데이트
        TrashCount[] trashCounts = FindObjectsOfType<TrashCount>();
        UpdateResultFromTrashCounts(trashCounts);
    }
}
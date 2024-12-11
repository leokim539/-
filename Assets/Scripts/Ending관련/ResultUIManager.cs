using UnityEngine;
using TMPro;

public class ResultUIManager : MonoBehaviour
{
    public TextMeshProUGUI player1NameText;
    public TextMeshProUGUI player2NameText;
    public TextMeshProUGUI player1TotalText;
    public TextMeshProUGUI player2TotalText;

    void Start()
    {
        UpdateResult(); // 초기 UI 업데이트
    }

    public void UpdateResult()
    {
        // 디버그용 로그 추가
        Debug.Log("Attempting to load player data from PlayerPrefs");

        int playerCount = 2;
        for (int i = 0; i < playerCount; i++)
        {
            string playerName = PlayerPrefs.GetString("Player" + i + "Name", "Unknown Player");
            int trashCount = PlayerPrefs.GetInt("Player" + i + "TrashCount", 0);

            Debug.Log($"Player {i}: Name = {playerName}, Trash Count = {trashCount}");

            // UI 업데이트
            if (i == 0)
            {
                player1NameText.text = playerName;
                player1TotalText.text = trashCount.ToString();
            }
            else if (i == 1)
            {
                player2NameText.text = playerName;
                player2TotalText.text = trashCount.ToString();
            }
        }
    }

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


    public void UpdateResultFromTrashCounts(TrashCount[] trashCounts)
    {
        if (trashCounts != null && trashCounts.Length >= 2)
        {
            player1TotalText.text = trashCounts[0].GetTotalTrashCount().ToString();
            player2TotalText.text = trashCounts[1].GetTotalTrashCount().ToString();
        }
        else
        {
            Debug.LogError("Trash counts are null or insufficient.");
        }
    }
}

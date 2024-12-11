using UnityEngine;
using Photon.Pun;

public class TrashCount : MonoBehaviourPunCallbacks
{
    [SerializeField] private int totalTrashCount = 0;

    public void AddTrash(int count)
    {
        if (count > 0)
        {
            // 포톤 RPC를 통해 쓰레기 수와 플레이어 정보를 추가
            photonView.RPC("AddTrashRPC", RpcTarget.All, count, PhotonNetwork.LocalPlayer.NickName);
        }
    }

    [PunRPC]
    private void AddTrashRPC(int count, string playerName)
    {
        if (count > 0)
        {
            totalTrashCount += count;
            LogTrashCount(); // 쓰레기 수 로그 출력

            // 모든 플레이어에게 업데이트된 정보를 전파
            UpdatePlayerInfo(playerName, totalTrashCount);
        }
    }

    private void UpdatePlayerInfo(string playerName, int trashCount)
    {
        // ResultUIManager 인스턴스에 접근하여 업데이트 호출
        ResultUIManager resultUIManager = FindObjectOfType<ResultUIManager>();
        if (resultUIManager != null)
        {
            // PlayerInfo 배열 생성
            PlayerInfo[] playerInfos = new PlayerInfo[PhotonNetwork.PlayerList.Length];
            for (int i = 0; i < playerInfos.Length; i++)
            {
                playerInfos[i] = new PlayerInfo(PhotonNetwork.PlayerList[i].NickName, totalTrashCount); // 각 플레이어의 정보를 업데이트
            }
            resultUIManager.UpdateResult(playerInfos); // UI 업데이트
        }
    }

    public int GetTotalTrashCount()
    {
        return totalTrashCount;
    }

    public void ResetTrashCount()
    {
        totalTrashCount = 0;
    }

    public void LogTrashCount()
    {
        Debug.Log($"Total Trash Count: {totalTrashCount}");
    }
}

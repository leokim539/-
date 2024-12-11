using UnityEngine;
using Photon.Pun;

public class TrashCount : MonoBehaviourPunCallbacks
{
    [SerializeField] private int totalTrashCount = 0;

    public void AddTrash(int count)
    {
        if (count > 0)
        {
            // 포톤 RPC를 통해 쓰레기 수를 추가
            photonView.RPC("AddTrashRPC", RpcTarget.All, count);
        }
    }

    [PunRPC]
    private void AddTrashRPC(int count)
    {
        if (count > 0)
        {
            totalTrashCount += count;
            LogTrashCount(); // 쓰레기 수 로그 출력

            // 모든 플레이어의 쓰레기 수를 업데이트
            UpdateAllPlayerInfos();
        }
    }

    private void UpdateAllPlayerInfos()
    {
        ResultUIManager resultUIManager = FindObjectOfType<ResultUIManager>();
        if (resultUIManager != null)
        {
            PlayerInfo[] playerInfos = new PlayerInfo[PhotonNetwork.PlayerList.Length];
            for (int i = 0; i < playerInfos.Length; i++)
            {
                string playerName = PhotonNetwork.PlayerList[i].NickName;

                // 각 플레이어의 쓰레기 수를 저장
                int trashCount = (PhotonNetwork.PlayerList[i].ActorNumber == photonView.Owner.ActorNumber)
                                 ? totalTrashCount
                                 : 0; // 상대방의 쓰레기 수는 0으로 초기화

                playerInfos[i] = new PlayerInfo(playerName, trashCount);
            }
            resultUIManager.UpdateResult(playerInfos);
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

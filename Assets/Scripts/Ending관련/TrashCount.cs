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

            // 모든 플레이어의 정보를 업데이트
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
                // 각 플레이어의 쓰레기 수를 가져오기
                int trashCount = (i + 1 == photonView.Owner.ActorNumber) ? totalTrashCount : 0; // 이 플레이어의 쓰레기 수
                playerInfos[i] = new PlayerInfo(PhotonNetwork.PlayerList[i].NickName, trashCount);
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

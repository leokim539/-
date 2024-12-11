using UnityEngine;
using Photon.Pun;

public class TrashCount : MonoBehaviourPunCallbacks
{
    [SerializeField] private int totalTrashCount = 0;

    public void AddTrash(int count)
    {
        if (count > 0)
        {
            // ���� RPC�� ���� ������ ���� �÷��̾� ������ �߰�
            photonView.RPC("AddTrashRPC", RpcTarget.All, count, PhotonNetwork.LocalPlayer.NickName);
        }
    }

    [PunRPC]
    private void AddTrashRPC(int count, string playerName)
    {
        if (count > 0)
        {
            totalTrashCount += count;
            LogTrashCount(); // ������ �� �α� ���

            // ��� �÷��̾�� ������Ʈ�� ������ ����
            UpdatePlayerInfo(playerName, totalTrashCount);
        }
    }

    private void UpdatePlayerInfo(string playerName, int trashCount)
    {
        // ResultUIManager �ν��Ͻ��� �����Ͽ� ������Ʈ ȣ��
        ResultUIManager resultUIManager = FindObjectOfType<ResultUIManager>();
        if (resultUIManager != null)
        {
            // PlayerInfo �迭 ����
            PlayerInfo[] playerInfos = new PlayerInfo[PhotonNetwork.PlayerList.Length];
            for (int i = 0; i < playerInfos.Length; i++)
            {
                playerInfos[i] = new PlayerInfo(PhotonNetwork.PlayerList[i].NickName, totalTrashCount); // �� �÷��̾��� ������ ������Ʈ
            }
            resultUIManager.UpdateResult(playerInfos); // UI ������Ʈ
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

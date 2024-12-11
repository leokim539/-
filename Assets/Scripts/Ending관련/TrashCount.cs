using UnityEngine;
using Photon.Pun;

public class TrashCount : MonoBehaviourPunCallbacks
{
    [SerializeField] private int totalTrashCount = 0;

    public void AddTrash(int count)
    {
        if (count > 0)
        {
            // ���� RPC�� ���� ������ ���� �߰�
            photonView.RPC("AddTrashRPC", RpcTarget.All, count);
        }
    }

    [PunRPC]
    private void AddTrashRPC(int count)
    {
        if (count > 0)
        {
            totalTrashCount += count;
            LogTrashCount(); // ������ �� �α� ���

            // ��� �÷��̾��� ������ ������Ʈ
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
                // �� �÷��̾��� ������ ���� ��������
                int trashCount = (i + 1 == photonView.Owner.ActorNumber) ? totalTrashCount : 0; // �� �÷��̾��� ������ ��
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

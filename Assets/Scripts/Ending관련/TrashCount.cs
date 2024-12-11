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

            // ��� �÷��̾��� ������ ���� ������Ʈ
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

                // �� �÷��̾��� ������ ���� ����
                int trashCount = (PhotonNetwork.PlayerList[i].ActorNumber == photonView.Owner.ActorNumber)
                                 ? totalTrashCount
                                 : 0; // ������ ������ ���� 0���� �ʱ�ȭ

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

using UnityEngine;
using Photon.Pun;

public class TrashCount : MonoBehaviourPunCallbacks
{
    [SerializeField] private int totalTrashCount = 0;

    public void AddTrash(int count)
    {
        // ���� �� �÷��̾� ������Ʈ�� �����ڸ� ī��Ʈ �߰� ����
        if (photonView.IsMine)
        {
            totalTrashCount += count;

            // ��� Ŭ���̾�Ʈ�� ����ȭ
            photonView.RPC("SyncTrashCount", RpcTarget.All, totalTrashCount);
        }
    }

    [PunRPC]
    private void SyncTrashCount(int count)
    {
        totalTrashCount = count;

        // ��� Ŭ���̾�Ʈ�� ResultUIManager ������Ʈ
        ResultUIManager resultUIManager = FindObjectOfType<ResultUIManager>();
        if (resultUIManager != null)
        {
            resultUIManager.UpdateResultFromTrashCounts(FindObjectsOfType<TrashCount>());
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
using UnityEngine;
using Photon.Pun; // ���� ���� ���ӽ����̽� �߰�

public class TrashCount : MonoBehaviourPunCallbacks
{
    [SerializeField] private int totalTrashCount;

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
        }
    }

    public int GetTotalTrashCount() // �� �޼���� public���� ���ǵǾ�� �մϴ�.
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

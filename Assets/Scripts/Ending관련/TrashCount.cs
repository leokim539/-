using UnityEngine;
using Photon.Pun;

public class TrashCount : MonoBehaviourPunCallbacks
{
    [SerializeField] private int totalTrashCount = 0;

    public void AddTrash(int count)
    {
        // 오직 이 플레이어 오브젝트의 소유자만 카운트 추가 가능
        if (photonView.IsMine)
        {
            totalTrashCount += count;

            // 모든 클라이언트에 동기화
            photonView.RPC("SyncTrashCount", RpcTarget.All, totalTrashCount);
        }
    }

    [PunRPC]
    private void SyncTrashCount(int count)
    {
        totalTrashCount = count;

        // 모든 클라이언트의 ResultUIManager 업데이트
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
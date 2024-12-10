using UnityEngine;
using Photon.Pun; // 포톤 관련 네임스페이스 추가

public class TrashCount : MonoBehaviourPunCallbacks
{
    [SerializeField] private int totalTrashCount;

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
        }
    }

    public int GetTotalTrashCount() // 이 메서드는 public으로 정의되어야 합니다.
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

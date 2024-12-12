using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // Photon.Pun 네임스페이스 추가

public class ExitGame : MonoBehaviourPunCallbacks
{
    public void OnLeaveButtonClicked()
    {
        LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        // 방을 나갔을 때의 처리 (필요시 구현)
        Debug.Log("Left the room.");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}

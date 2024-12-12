using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // Photon.Pun ���ӽ����̽� �߰�

public class ExitGame : MonoBehaviourPunCallbacks
{
    public void OnLeaveButtonClicked()
    {
        LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        // ���� ������ ���� ó�� (�ʿ�� ����)
        Debug.Log("Left the room.");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}

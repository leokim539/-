using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemManager : MonoBehaviourPunCallbacks
{
    public static ItemManager instane;
    public FirstPersonController firstPersonController; // ��ų�� ����� ĳ����
    public Trash2 trash2;
    private void Awake()
    {
        instane = this;
    }
    // ��ų ��� �޼���
    public void UseSkill()
    {
        if (firstPersonController != null && firstPersonController.photonView.IsMine) // �ڽ��� ĳ������ ����
        {
            // ��Ʈ��ũ�� ���� ��ų ��� ��û
            photonView.RPC("RPCUseSkill", RpcTarget.Others);
        }
    }

    [PunRPC]
    public void RPCUseSkill0()
    {
        firstPersonController.m_PhotonView.RPC("ApplyObscuringEffects", RpcTarget.All);
    }
    [PunRPC]
    public void RPCUseSkill1()
    {
        firstPersonController.m_PhotonView.RPC("TaserGuns", RpcTarget.All);
    }
}

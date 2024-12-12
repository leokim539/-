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
    public void RPCUseSkill0()//����ź
    {
        firstPersonController.m_PhotonView.RPC("ApplyObscuringEffects", RpcTarget.All);
    }
    [PunRPC]
    public void RPCUseSkill1()//����ź
    {
        firstPersonController.m_PhotonView.RPC("TaserGuns", RpcTarget.All);
    }
    [PunRPC]
    public void RPCUseSkill2()//��� ��ŷ
    {
        firstPersonController.m_PhotonView.RPC("Hackings", RpcTarget.All);
    }
    [PunRPC]
    public void RPCUseSkill3()//��� ��ų 1�� ����ȭ
    {
        firstPersonController.m_PhotonView.RPC("SmartPhone", RpcTarget.All);
    }
    [PunRPC]
    public void RPCUseSkill4()
    {
        firstPersonController.m_PhotonView.RPC("HandCreams", RpcTarget.All);
    }
    [PunRPC]
    public void RPCUseSkill5()//��� ��ų 1�� ����ȭ
    {
        firstPersonController.m_PhotonView.RPC("Bonds", RpcTarget.All);
    }
    [PunRPC]
    public void RPCUseSkill6()//��� ��ų 1�� ����ȭ
    {
        firstPersonController.m_PhotonView.RPC("Spoons", RpcTarget.All);
    }
}

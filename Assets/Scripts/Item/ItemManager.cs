using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemManager : MonoBehaviourPunCallbacks
{
    public static ItemManager instane;
    public FirstPersonController firstPersonController; // 스킬을 사용할 캐릭터
    public Trash2 trash2;
    private void Awake()
    {
        instane = this;
    }
    // 스킬 사용 메서드
    public void UseSkill()
    {
        if (firstPersonController != null && firstPersonController.photonView.IsMine) // 자신의 캐릭터일 때만
        {
            // 네트워크를 통해 스킬 사용 요청
            photonView.RPC("RPCUseSkill", RpcTarget.Others);
        }
    }

    [PunRPC]
    public void RPCUseSkill0()//연막탄
    {
        firstPersonController.m_PhotonView.RPC("ApplyObscuringEffects", RpcTarget.All);
    }
    [PunRPC]
    public void RPCUseSkill1()//섬광탄
    {
        firstPersonController.m_PhotonView.RPC("TaserGuns", RpcTarget.All);
    }
    [PunRPC]
    public void RPCUseSkill2()//상대 해킹
    {
        firstPersonController.m_PhotonView.RPC("Hackings", RpcTarget.All);
    }
    [PunRPC]
    public void RPCUseSkill3()//상대 스킬 1번 무력화
    {
        firstPersonController.m_PhotonView.RPC("SmartPhone", RpcTarget.All);
    }
    [PunRPC]
    public void RPCUseSkill4()
    {
        firstPersonController.m_PhotonView.RPC("HandCreams", RpcTarget.All);
    }
    [PunRPC]
    public void RPCUseSkill5()//상대 스킬 1번 무력화
    {
        firstPersonController.m_PhotonView.RPC("Bonds", RpcTarget.All);
    }
    [PunRPC]
    public void RPCUseSkill6()//상대 스킬 1번 무력화
    {
        firstPersonController.m_PhotonView.RPC("Spoons", RpcTarget.All);
    }
}

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

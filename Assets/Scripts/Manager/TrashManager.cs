using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TrashManager : MonoBehaviourPunCallbacks
{
    [Header("쓰레기 게이지")]
    public int Maxscary = 100; // 최대 공포
    public int scary; // 현재 공포
    public Image scaryBar; // 공포 이미지

    [Header("느려지는 속도")]
    public float slowedSpeedMultiplier = 0.5f; // 보내줄 값
    public bool isEffectActive = false; // DangerItem에서 변화시킬 값

    public AudioSource effectSound; // 소리 효과

    private FirstPersonController playerController;

    public void Start()
    {
        UpdateScaryBar();
        playerController = GetComponent<FirstPersonController>(); // 플레이어 컨트롤러 가져오기

        if (effectSound != null)
        {
            effectSound.Stop(); // 소리 끄기
        }
    }

    public void Update()
    {
        if (isEffectActive)
        {
            if (playerController != null)
            {
                playerController.SlowDown(slowedSpeedMultiplier); // 플레이어의 속도를 감소시키는 함수 호출
            }
            if (effectSound != null)
            {
                effectSound.Play(); // 소리 키기
            }
        }
        else
        {
            if (playerController != null)
            {
                playerController.RestoreSpeed(); // 원래 속도로 복구
            }
            if (effectSound != null)
            {
                effectSound.Stop(); // 소리 끄기
            }
        }
    }

    public void UpdateScaryBar()
    {
        scaryBar.fillAmount = (float)scary / Maxscary;
    }

    // PUN을 통한 데이터 동기화
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 이 객체의 상태를 네트워크에 전송
            stream.SendNext(isEffectActive);
        }
        else
        {
            // 네트워크에서 상태를 수신
            isEffectActive = (bool)stream.ReceiveNext();
            UpdateScaryBar(); // UI 업데이트
        }
    }

    // 공포 상태를 변경하는 메서드
    public void ChangeScary(int amount)
    {
        scary = Mathf.Clamp(scary + amount, 0, Maxscary);
        UpdateScaryBar();

        // 상태 변경 시 모든 클라이언트에 알림
        photonView.RPC("UpdateEffectState", RpcTarget.Others, isEffectActive);
    }

    [PunRPC]
    public void UpdateEffectState(bool newIsEffectActive)
    {
        isEffectActive = newIsEffectActive;
        UpdateScaryBar();
    }
}
/*
{
    [Header("쓰레기 게이지")]
    public int Maxscary = 100;//최대 공포
    public int scary;//현제 공포
    public Image scaryBar; // 공포 이미지

    [Header("느려지는 속도")]
    public float slowedSpeedMultiplier = 0.5f; //보내줄값
    public bool isEffectActive = false;//DangerItem에서 변화시킬값

    public AudioSource effectSound; // 소리 효과

    public GameObject Player;
    private FirstPersonController playerController;
    public void Start()
    {

        UpdateScaryBar();
        Player = GameObject.Find("Player(Clone)");
        playerController = Player.GetComponent<FirstPersonController>();
        if (effectSound != null)
        {
            effectSound.Stop();//소리끄기
        }
    }
    public void Update()
    {
        if (isEffectActive)
        {
            if (playerController != null)
            {
                playerController.SlowDown(slowedSpeedMultiplier);// 플레이어의 속도를 감소시키는 함수 호출
            }
            if (effectSound != null)
            {
                effectSound.Play();//소리키기
            }
        }
        else
        {
            playerController.RestoreSpeed();  // 원래 속도로 복구
            if (effectSound != null)
            {
                effectSound.Stop();//소리끄기
            }
        }
    }
    public void UpdateScaryBar()
    {
        scaryBar.fillAmount = (float)scary / Maxscary;
    }
}
*/
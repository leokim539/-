using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrashManager : MonoBehaviour
{
    [Header("쓰레기 게이지")]
    public int Maxscary = 100;//최대 공포
    public int scary;//현제 공포
    public Image scaryBar; // 공포 이미지

    [Header("느려지는 속도")]
    public float slowedSpeedMultiplier = 0.5f; //보내줄값
    public bool isEffectActive = false;//DangerItem에서 변화시킬값

    public AudioSource effectSound; // 소리 효과

    private GameObject Player;
    private FirstPersonController playerController;
    public void Start()
    {
        UpdateScaryBar();
        Player = GameObject.Find("Player");
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

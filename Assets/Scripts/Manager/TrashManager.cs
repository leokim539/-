using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrashManager : MonoBehaviour
{
    [Header("������ ������")]
    public int Maxscary = 100;//�ִ� ����
    public int scary;//���� ����
    public Image scaryBar; // ���� �̹���

    [Header("�������� �ӵ�")]
    public float slowedSpeedMultiplier = 0.5f; //�����ٰ�
    public bool isEffectActive = false;//DangerItem���� ��ȭ��ų��

    public AudioSource effectSound; // �Ҹ� ȿ��

    private GameObject Player;
    private FirstPersonController playerController;
    public void Start()
    {
        UpdateScaryBar();
        Player = GameObject.Find("Player");
        playerController = Player.GetComponent<FirstPersonController>();
        if (effectSound != null)
        {
            effectSound.Stop();//�Ҹ�����
        }
    }
    public void Update()
    {
        if (isEffectActive)
        {
            if (playerController != null)
            {
                playerController.SlowDown(slowedSpeedMultiplier);// �÷��̾��� �ӵ��� ���ҽ�Ű�� �Լ� ȣ��
            }
            if (effectSound != null)
            {
                effectSound.Play();//�Ҹ�Ű��
            }
        }
        else
        {
            playerController.RestoreSpeed();  // ���� �ӵ��� ����
            if (effectSound != null)
            {
                effectSound.Stop();//�Ҹ�����
            }
        }
    }
    public void UpdateScaryBar()
    {
        scaryBar.fillAmount = (float)scary / Maxscary;
    }
}

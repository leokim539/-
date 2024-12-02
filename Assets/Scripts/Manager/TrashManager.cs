using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TrashManager : MonoBehaviourPunCallbacks
{
    [Header("������ ������")]
    public int Maxscary = 100; // �ִ� ����
    public int scary; // ���� ����
    public Image scaryBar; // ���� �̹���

    [Header("�������� �ӵ�")]
    public float slowedSpeedMultiplier = 0.5f; // ������ ��
    public bool isEffectActive = false; // DangerItem���� ��ȭ��ų ��

    public AudioSource effectSound; // �Ҹ� ȿ��

    private FirstPersonController playerController;

    public void Start()
    {
        UpdateScaryBar();
        playerController = GetComponent<FirstPersonController>(); // �÷��̾� ��Ʈ�ѷ� ��������

        if (effectSound != null)
        {
            effectSound.Stop(); // �Ҹ� ����
        }
    }

    public void Update()
    {
        if (isEffectActive)
        {
            if (playerController != null)
            {
                playerController.SlowDown(slowedSpeedMultiplier); // �÷��̾��� �ӵ��� ���ҽ�Ű�� �Լ� ȣ��
            }
            if (effectSound != null)
            {
                effectSound.Play(); // �Ҹ� Ű��
            }
        }
        else
        {
            if (playerController != null)
            {
                playerController.RestoreSpeed(); // ���� �ӵ��� ����
            }
            if (effectSound != null)
            {
                effectSound.Stop(); // �Ҹ� ����
            }
        }
    }

    public void UpdateScaryBar()
    {
        scaryBar.fillAmount = (float)scary / Maxscary;
    }

    // PUN�� ���� ������ ����ȭ
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // �� ��ü�� ���¸� ��Ʈ��ũ�� ����
            stream.SendNext(isEffectActive);
        }
        else
        {
            // ��Ʈ��ũ���� ���¸� ����
            isEffectActive = (bool)stream.ReceiveNext();
            UpdateScaryBar(); // UI ������Ʈ
        }
    }

    // ���� ���¸� �����ϴ� �޼���
    public void ChangeScary(int amount)
    {
        scary = Mathf.Clamp(scary + amount, 0, Maxscary);
        UpdateScaryBar();

        // ���� ���� �� ��� Ŭ���̾�Ʈ�� �˸�
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
    [Header("������ ������")]
    public int Maxscary = 100;//�ִ� ����
    public int scary;//���� ����
    public Image scaryBar; // ���� �̹���

    [Header("�������� �ӵ�")]
    public float slowedSpeedMultiplier = 0.5f; //�����ٰ�
    public bool isEffectActive = false;//DangerItem���� ��ȭ��ų��

    public AudioSource effectSound; // �Ҹ� ȿ��

    public GameObject Player;
    private FirstPersonController playerController;
    public void Start()
    {

        UpdateScaryBar();
        Player = GameObject.Find("Player(Clone)");
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
*/
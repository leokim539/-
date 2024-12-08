using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class TaskUIManager : MonoBehaviourPunCallbacks
{
    [Header("������ �ֿ� ����")]
    public Text circleText;
    public Text cylinderText;
    public Text squareText;
    public Text BeerCanText;
    public Text PetBottleText;
    public Text TrashBagText;

    [Header("������ ����")]
    public Image circleImage;
    public Image cylinderImage;
    public Image squareImage;

    [Header("������ ������")]
    public GameObject circleStrikeThrough;
    public GameObject cylinderStrikeThrough;
    public GameObject squareStrikeThrough;
    public GameObject BeerCanStrikeThrough;
    public GameObject PetBottleStrikeThrough;
    public GameObject TrashBagStrikeThrough;

    private int circleCount = 0;
    private int cylinderCount = 0;
    private int squareCount = 0;
    private int BeerCanCount = 0;
    private int PetBottleCount = 0;
    private int TrashBagCount = 0;

    // ����� ī��Ʈ ���� �߰�
    private int storedCircleCount = 0;
    private int storedCylinderCount = 0;
    private int storedSquareCount = 0;
    private int storedBeerCanCount = 0;
    private int storedPetBottleCount = 0;
    private int storedTrashBagCount = 0;

    private int totalCircle = 5;
    private int totalCylinder = 5;
    private int totalSquare = 5;
    private int totalBeerCan = 5;
    private int totalPetBottle = 5;
    private int totalTrashBag = 5;


    public int GetTotalTrashCount()
    {
        return circleCount + cylinderCount + squareCount + BeerCanCount + PetBottleCount + TrashBagCount;
    }

    void Start()
    {
        UpdateUI();
    }

    public void StoreCircleCount()
    {
        if (PhotonNetwork.IsMessageQueueRunning || !PhotonNetwork.IsConnected)
        {
            storedCircleCount++; // ����� ī��Ʈ ����
            UpdateUI();

            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("RPC_StoreCircleCount", RpcTarget.Others);
            }
        }
    }


    [PunRPC]
    public void RPC_StoreCircleCount()
    {
        storedCircleCount++; // ����� ī��Ʈ ����
    }

    public void StoreCylinderCount()
    {
        if (PhotonNetwork.IsMessageQueueRunning || !PhotonNetwork.IsConnected)
        {
            storedCylinderCount++; // ����� ī��Ʈ ����
            UpdateUI();

            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("RPC_StoreCylinderCount", RpcTarget.Others);
            }
        }
    }

    [PunRPC]
    public void RPC_StoreCylinderCount()
    {
        storedCylinderCount++; // ����� ī��Ʈ ����
    }

    public void StoreSquareCount()
    {
        if (PhotonNetwork.IsMessageQueueRunning || !PhotonNetwork.IsConnected)
        {
            storedSquareCount++; // ����� ī��Ʈ ����
            UpdateUI();

            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("RPC_StoreSquareCount", RpcTarget.Others);
            }
        }
    }

    [PunRPC]
    public void RPC_StoreSquareCount()
    {
        storedSquareCount++; // ����� ī��Ʈ ����
    }

    public void StoreBeerCanCount()
    {
        if (PhotonNetwork.IsMessageQueueRunning || !PhotonNetwork.IsConnected)
        {
            storedBeerCanCount++; // ����� ī��Ʈ ����
            UpdateUI();

            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("RPC_StoreBeerCanCount", RpcTarget.Others);
            }
        }
    }

    [PunRPC]
    public void RPC_StoreBeerCanCount()
    {
        storedBeerCanCount++; // ����� ī��Ʈ ����
    }

    public void StorePetBottleCount()
    {
        if (PhotonNetwork.IsMessageQueueRunning || !PhotonNetwork.IsConnected)
        {
            storedPetBottleCount++; // ����� ī��Ʈ ����
            UpdateUI();

            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("RPC_StorePetBottleCount", RpcTarget.Others);
            }
        }
    }

    

    public void StoreTrashBagCount()
    {
        if (PhotonNetwork.IsMessageQueueRunning || !PhotonNetwork.IsConnected)
        {
            storedTrashBagCount++; // ����� ī��Ʈ ����
            UpdateUI();

            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("RPC_StoreTrashBagCount", RpcTarget.Others);
            }
        }
    }

    [PunRPC]
    public void RPC_StoreTrashBagCount()
    {
        storedTrashBagCount++; // ����� ī��Ʈ ����
        UpdateUI(); // UI ������Ʈ
    }

    // TrashCan�� ��ȣ�ۿ� �� ȣ���� �޼ҵ�
    public void ConfirmCollection()
    {
        // ����� ī��Ʈ�� ���� ī��Ʈ�� ������Ű��
        circleCount += storedCircleCount;
        cylinderCount += storedCylinderCount;
        squareCount += storedSquareCount;
        BeerCanCount += storedBeerCanCount;
        PetBottleCount += storedPetBottleCount;
        TrashBagCount += storedTrashBagCount;

        // ����� ī��Ʈ �ʱ�ȭ
        storedCircleCount = 0;
        storedCylinderCount = 0;
        storedSquareCount = 0;
        storedBeerCanCount = 0;
        storedPetBottleCount = 0;
        storedTrashBagCount = 0;

        UpdateUI();
    }



    public void UpdateUI()
    {
        if (circleText != null) circleText.text = $"{circleCount}/{totalCircle}";
        if (cylinderText != null) cylinderText.text = $"{cylinderCount}/{totalCylinder}";
        if (squareText != null) squareText.text = $"{squareCount}/{totalSquare}";
        if (BeerCanText != null) BeerCanText.text = $"{BeerCanCount}/{totalBeerCan}";
        if (PetBottleText != null) PetBottleText.text = $"{PetBottleCount}/{totalPetBottle}";
        if (TrashBagText != null) TrashBagText.text = $"{TrashBagCount}/{totalTrashBag}";

    }

    
}
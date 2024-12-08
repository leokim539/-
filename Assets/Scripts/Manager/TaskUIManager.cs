using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class TaskUIManager : MonoBehaviourPunCallbacks
{
    [Header("쓰레기 주운 갯수")]
    public Text circleText;
    public Text cylinderText;
    public Text squareText;
    public Text BeerCanText;
    public Text PetBottleText;
    public Text TrashBagText;

    [Header("쓰레기 사진")]
    public Image circleImage;
    public Image cylinderImage;
    public Image squareImage;

    [Header("쓰레기 빨간줄")]
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

    // 저장된 카운트 변수 추가
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
            storedCircleCount++; // 저장된 카운트 증가
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
        storedCircleCount++; // 저장된 카운트 증가
    }

    public void StoreCylinderCount()
    {
        if (PhotonNetwork.IsMessageQueueRunning || !PhotonNetwork.IsConnected)
        {
            storedCylinderCount++; // 저장된 카운트 증가
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
        storedCylinderCount++; // 저장된 카운트 증가
    }

    public void StoreSquareCount()
    {
        if (PhotonNetwork.IsMessageQueueRunning || !PhotonNetwork.IsConnected)
        {
            storedSquareCount++; // 저장된 카운트 증가
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
        storedSquareCount++; // 저장된 카운트 증가
    }

    public void StoreBeerCanCount()
    {
        if (PhotonNetwork.IsMessageQueueRunning || !PhotonNetwork.IsConnected)
        {
            storedBeerCanCount++; // 저장된 카운트 증가
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
        storedBeerCanCount++; // 저장된 카운트 증가
    }

    public void StorePetBottleCount()
    {
        if (PhotonNetwork.IsMessageQueueRunning || !PhotonNetwork.IsConnected)
        {
            storedPetBottleCount++; // 저장된 카운트 증가
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
            storedTrashBagCount++; // 저장된 카운트 증가
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
        storedTrashBagCount++; // 저장된 카운트 증가
        UpdateUI(); // UI 업데이트
    }

    // TrashCan과 상호작용 시 호출할 메소드
    public void ConfirmCollection()
    {
        // 저장된 카운트를 실제 카운트로 증가시키기
        circleCount += storedCircleCount;
        cylinderCount += storedCylinderCount;
        squareCount += storedSquareCount;
        BeerCanCount += storedBeerCanCount;
        PetBottleCount += storedPetBottleCount;
        TrashBagCount += storedTrashBagCount;

        // 저장된 카운트 초기화
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TaskUIManager : MonoBehaviourPunCallbacks
{
    [Header("������ �ֿ� ����")]
    public Text circleText;
    public Text cylinderText;
    public Text squareText;
    public Text BeerCanText;
    public Text PetBottleText;
    public Text TrashBagText;

    private int circleCount = 0;
    private int cylinderCount = 0;
    private int squareCount = 0;
    private int BeerCanCount = 0;
    private int PetBottleCount = 0;
    private int TrashBagCount = 0;

    private int storedCircleCount = 0;
    private int storedCylinderCount = 0;
    private int storedSquareCount = 0;
    private int storedBeerCanCount = 0;
    private int storedPetBottleCount = 0;
    private int storedTrashBagCount = 0;

    private PlayerInfo[] playerInfos = new PlayerInfo[2]; // �� ���� �÷��̾� ���� ����

    private TrashCount[] trashCounts; // TrashCount �迭 ����

    public override void OnJoinedRoom()
    {
        Start(); // �濡 �� �� �ʱ�ȭ
    }
    void Start()
    {
        int playerCount = PhotonNetwork.PlayerList.Length;
        playerInfos = new PlayerInfo[playerCount];
        trashCounts = new TrashCount[playerCount];

        Debug.Log($"Total Players: {playerCount}");

        for (int i = 0; i < playerCount; i++)
        {
            playerInfos[i] = new PlayerInfo(PhotonNetwork.PlayerList[i].NickName, 0);

            // �÷��̾� GameObject ��������
            GameObject playerObject = PhotonNetwork.PlayerList[i].TagObject as GameObject;
            if (playerObject != null)
            {
                Debug.Log($"Found Player Object: {playerObject.name}");

                // TrashCount ã��
                TrashCount trashCount = playerObject.GetComponent<TrashCount>();
                if (trashCount != null)
                {
                    trashCounts[i] = trashCount;
                    Debug.Log($"Player {i}: {playerInfos[i].playerName}, TrashCount Found: {trashCounts[i] != null}");
                }
                else
                {
                    Debug.LogError($"TrashCount not found for player: {playerInfos[i].playerName}");
                }
            }
            else
            {
                Debug.LogError($"Player Object is null for index: {i}");
            }
        }

        UpdateUI();
    }



    public void StoreCircleCount()
    {
        storedCircleCount++; // ����� ī��Ʈ ����
        UpdateUI(); // UI ������Ʈ
    }

    public void StoreCylinderCount()
    {
        storedCylinderCount++; // ����� ī��Ʈ ����
        UpdateUI(); // UI ������Ʈ
    }

    public void StoreSquareCount()
    {
        storedSquareCount++; // ����� ī��Ʈ ����
        UpdateUI(); // UI ������Ʈ
    }

    public void StoreBeerCanCount()
    {
        storedBeerCanCount++; // ����� ī��Ʈ ����
        UpdateUI(); // UI ������Ʈ
    }

    public void StorePetBottleCount()
    {
        storedPetBottleCount++; // ����� ī��Ʈ ����
        UpdateUI(); // UI ������Ʈ
    }

    public void StoreTrashBagCount()
    {
        storedTrashBagCount++; // ����� ī��Ʈ ����
        UpdateUI(); // UI ������Ʈ
    }

    public void ConfirmCollection()
    {
        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;

        // ������ �ε��� Ȯ��
        if (playerIndex < 0 || playerIndex >= trashCounts.Length)
        {
            Debug.LogError($"Invalid player index: {playerIndex}");
            return;
        }

        // ���� ī��Ʈ�� ������Ű��
        int totalToAdd = (storedCircleCount + storedCylinderCount + storedSquareCount + storedBeerCanCount + storedPetBottleCount + storedTrashBagCount);

        // �÷��̾� ���� ������Ʈ
        playerInfos[playerIndex].totalTrashCount += totalToAdd;

        // TrashCount ������Ʈ
        if (trashCounts[playerIndex] != null)
        {
            trashCounts[playerIndex].AddTrash(totalToAdd); // TrashCount ������Ʈ
        }
        else
        {
            Debug.LogError($"TrashCount not found for player index: {playerIndex}");
        }

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

        // UI ������Ʈ
        UpdateUI(); // UI ������Ʈ

        // �÷��̾� ���� ����ȭ
        photonView.RPC("SyncPlayerInfo", RpcTarget.All, playerIndex, playerInfos[playerIndex].totalTrashCount);
    }


    [PunRPC]
    public void SyncPlayerInfo(int playerIndex, int totalTrashCount)
    {
        playerInfos[playerIndex].totalTrashCount = totalTrashCount;
        UpdateUI(); // UI ������Ʈ
    }



    public void UpdateUI()
    {
        if (circleText != null) circleText.text = $"{circleCount}/5"; // ���÷� �ִ� ���� 5
        if (cylinderText != null) cylinderText.text = $"{cylinderCount}/5";
        if (squareText != null) squareText.text = $"{squareCount}/5";
        if (BeerCanText != null) BeerCanText.text = $"{BeerCanCount}/5";
        if (PetBottleText != null) PetBottleText.text = $"{PetBottleCount}/5";
        if (TrashBagText != null) TrashBagText.text = $"{TrashBagCount}/5";

        // �÷��̾� ���� ���
        foreach (var playerInfo in playerInfos)
        {
            Debug.Log($"Player Name: {playerInfo.playerName}, Total Trash Collected: {playerInfo.totalTrashCount}");
        }
    }

    public PlayerInfo[] GetPlayerInfos()
    {
        return playerInfos; // �÷��̾� ���� ��ȯ
    }

    public void EndGame()
    {
        for (int i = 0; i < playerInfos.Length; i++)
        {
            // PlayerPrefs�� �� ������ ���� ����
            PlayerPrefs.SetInt("Player" + i + "TrashCount", playerInfos[i].totalTrashCount);
            PlayerPrefs.SetString("Player" + i + "Name", playerInfos[i].playerName);
        }

        // ResultUIManager �ν��Ͻ� ã��
        ResultUIManager resultUIManager = FindObjectOfType<ResultUIManager>();
        if (resultUIManager != null)
        {
            resultUIManager.UpdateResult(playerInfos); // �÷��̾� ���� ����
        }

        // ���⼭�� �� ��ȯ�� ���� ����
        Debug.Log("Game ended and data saved."); // ����� �α� �߰�
    }


}

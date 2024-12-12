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
    public Text beerCanText;
    public Text petBottleText;
    public Text trashBagText;

    private int circleCount = 0;
    private int cylinderCount = 0;
    private int squareCount = 0;
    private int beerCanCount = 0;
    private int petBottleCount = 0;
    private int trashBagCount = 0;

    private int storedCircleCount = 0;
    private int storedCylinderCount = 0;
    private int storedSquareCount = 0;
    private int storedBeerCanCount = 0;
    private int storedPetBottleCount = 0;
    private int storedTrashBagCount = 0;

    private PlayerInfo[] playerInfos = new PlayerInfo[2];

    public override void OnJoinedRoom()
    {
        Start();
    }

    void Start()
    {
        int playerCount = PhotonNetwork.PlayerList.Length;
        playerInfos = new PlayerInfo[playerCount];

        for (int i = 0; i < playerCount; i++)
        {
            playerInfos[i] = new PlayerInfo(PhotonNetwork.PlayerList[i].NickName, 0);
        }

        UpdateUI();
    }

    public void StoreCircleCount()
    {
        storedCircleCount++;
        UpdateUI();
        ActivateUIIfNeeded();
    }

    public void StoreCylinderCount()
    {
        storedCylinderCount++;
        UpdateUI();
        ActivateUIIfNeeded();
    }

    public void StoreSquareCount()
    {
        storedSquareCount++;
        UpdateUI();
        ActivateUIIfNeeded();
    }

    public void StoreBeerCanCount()
    {
        storedBeerCanCount++;
        UpdateUI();
        ActivateUIIfNeeded();
    }

    public void StorePetBottleCount()
    {
        storedPetBottleCount++;
        UpdateUI();
        ActivateUIIfNeeded();
    }

    public void StoreTrashBagCount()
    {
        storedTrashBagCount++;
        UpdateUI();
        ActivateUIIfNeeded();
    }

    public void ConfirmCollection()
    {
        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;

        if (playerIndex < 0 || playerIndex >= playerInfos.Length)
        {
            Debug.LogError($"Invalid player index: {playerIndex}");
            return;
        }

        // ����� ī��Ʈ�� ������ ���
        int totalToAdd = (storedCircleCount + storedCylinderCount + storedSquareCount + storedBeerCanCount + storedPetBottleCount + storedTrashBagCount);

        // �÷��̾��� �� ������ ���� ������Ʈ
        playerInfos[playerIndex].totalTrashCount += totalToAdd;

        // Photon Custom Properties ������Ʈ
        ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable
    {
        { "TotalTrashCount", playerInfos[playerIndex].totalTrashCount }
    };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);

        // ���� ī��Ʈ�� ������Ʈ
        circleCount += storedCircleCount;
        cylinderCount += storedCylinderCount;
        squareCount += storedSquareCount;
        beerCanCount += storedBeerCanCount;
        petBottleCount += storedPetBottleCount;
        trashBagCount += storedTrashBagCount;

        // ����� ī��Ʈ �ʱ�ȭ
        storedCircleCount = 0;
        storedCylinderCount = 0;
        storedSquareCount = 0;
        storedBeerCanCount = 0;
        storedPetBottleCount = 0;
        storedTrashBagCount = 0;

        // UI ������Ʈ
        UpdateUI(); // ���⿡�� UI�� ������Ʈ
    }


    public void UpdateUI()
    {
        if (circleText != null) circleText.text = $"{circleCount}/5";
        if (cylinderText != null) cylinderText.text = $"{cylinderCount}/5";
        if (squareText != null) squareText.text = $"{squareCount}/5";
        if (beerCanText != null) beerCanText.text = $"{beerCanCount}/5";
        if (petBottleText != null) petBottleText.text = $"{petBottleCount}/5";
        if (trashBagText != null) trashBagText.text = $"{trashBagCount}/5";

        foreach (var playerInfo in playerInfos)
        {
            Debug.Log($"Player Name: {playerInfo.playerName}, Total Trash Collected: {playerInfo.totalTrashCount}");
        }
    }

    private void ActivateUIIfNeeded()
    {
        // UI�� Ȱ��ȭ�ϴ� ������ �߰��մϴ�.
        if (storedCircleCount > 0 || storedCylinderCount > 0 || storedSquareCount > 0 ||
            storedBeerCanCount > 0 || storedPetBottleCount > 0 || storedTrashBagCount > 0)
        {
            // UI Ȱ��ȭ ���� (��: Ư�� UI ��� Ȱ��ȭ)
            // ���� ���, � UI ������Ʈ�� Ȱ��ȭ�Ϸ���:
            // someUIElement.SetActive(true);
            Debug.Log("UI Ȱ��ȭ: �����Ⱑ ����Ǿ����ϴ�.");
        }
    }

    public PlayerInfo[] GetPlayerInfos()
    {
        return playerInfos;
    }

    public void EndGame()
    {
        for (int i = 0; i < playerInfos.Length; i++)
        {
            PlayerPrefs.SetInt("Player" + i + "TrashCount", playerInfos[i].totalTrashCount);
            PlayerPrefs.SetString("Player" + i + "Name", playerInfos[i].playerName);
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("ResultScene");
    }
}

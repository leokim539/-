using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TaskUIManager : MonoBehaviourPunCallbacks
{
    [Header("쓰레기 주운 갯수")]
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

    private PlayerInfo[] playerInfos = new PlayerInfo[2]; // 두 명의 플레이어 정보 저장

    private TrashCount[] trashCounts; // TrashCount 배열 선언

    public override void OnJoinedRoom()
    {
        Start(); // 방에 들어간 후 초기화
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

            // 플레이어 GameObject 가져오기
            GameObject playerObject = PhotonNetwork.PlayerList[i].TagObject as GameObject;
            if (playerObject != null)
            {
                Debug.Log($"Found Player Object: {playerObject.name}");

                // TrashCount 찾기
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
        storedCircleCount++; // 저장된 카운트 증가
        UpdateUI(); // UI 업데이트
    }

    public void StoreCylinderCount()
    {
        storedCylinderCount++; // 저장된 카운트 증가
        UpdateUI(); // UI 업데이트
    }

    public void StoreSquareCount()
    {
        storedSquareCount++; // 저장된 카운트 증가
        UpdateUI(); // UI 업데이트
    }

    public void StoreBeerCanCount()
    {
        storedBeerCanCount++; // 저장된 카운트 증가
        UpdateUI(); // UI 업데이트
    }

    public void StorePetBottleCount()
    {
        storedPetBottleCount++; // 저장된 카운트 증가
        UpdateUI(); // UI 업데이트
    }

    public void StoreTrashBagCount()
    {
        storedTrashBagCount++; // 저장된 카운트 증가
        UpdateUI(); // UI 업데이트
    }

    public void ConfirmCollection()
    {
        int playerIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;

        // 안전한 인덱스 확인
        if (playerIndex < 0 || playerIndex >= trashCounts.Length)
        {
            Debug.LogError($"Invalid player index: {playerIndex}");
            return;
        }

        // 실제 카운트로 증가시키기
        int totalToAdd = (storedCircleCount + storedCylinderCount + storedSquareCount + storedBeerCanCount + storedPetBottleCount + storedTrashBagCount);

        // 플레이어 정보 업데이트
        playerInfos[playerIndex].totalTrashCount += totalToAdd;

        // TrashCount 업데이트
        if (trashCounts[playerIndex] != null)
        {
            trashCounts[playerIndex].AddTrash(totalToAdd); // TrashCount 업데이트
        }
        else
        {
            Debug.LogError($"TrashCount not found for player index: {playerIndex}");
        }

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

        // UI 업데이트
        UpdateUI(); // UI 업데이트

        // 플레이어 정보 동기화
        photonView.RPC("SyncPlayerInfo", RpcTarget.All, playerIndex, playerInfos[playerIndex].totalTrashCount);
    }


    [PunRPC]
    public void SyncPlayerInfo(int playerIndex, int totalTrashCount)
    {
        playerInfos[playerIndex].totalTrashCount = totalTrashCount;
        UpdateUI(); // UI 업데이트
    }



    public void UpdateUI()
    {
        if (circleText != null) circleText.text = $"{circleCount}/5"; // 예시로 최대 개수 5
        if (cylinderText != null) cylinderText.text = $"{cylinderCount}/5";
        if (squareText != null) squareText.text = $"{squareCount}/5";
        if (BeerCanText != null) BeerCanText.text = $"{BeerCanCount}/5";
        if (PetBottleText != null) PetBottleText.text = $"{PetBottleCount}/5";
        if (TrashBagText != null) TrashBagText.text = $"{TrashBagCount}/5";

        // 플레이어 정보 출력
        foreach (var playerInfo in playerInfos)
        {
            Debug.Log($"Player Name: {playerInfo.playerName}, Total Trash Collected: {playerInfo.totalTrashCount}");
        }
    }

    public PlayerInfo[] GetPlayerInfos()
    {
        return playerInfos; // 플레이어 정보 반환
    }

    public void EndGame()
    {
        for (int i = 0; i < playerInfos.Length; i++)
        {
            // PlayerPrefs에 총 쓰레기 개수 저장
            PlayerPrefs.SetInt("Player" + i + "TrashCount", playerInfos[i].totalTrashCount);
            PlayerPrefs.SetString("Player" + i + "Name", playerInfos[i].playerName);
        }

        // ResultUIManager 인스턴스 찾기
        ResultUIManager resultUIManager = FindObjectOfType<ResultUIManager>();
        if (resultUIManager != null)
        {
            resultUIManager.UpdateResult(playerInfos); // 플레이어 정보 전달
        }

        // 여기서는 씬 전환을 하지 않음
        Debug.Log("Game ended and data saved."); // 디버그 로그 추가
    }


}

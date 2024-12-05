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

    private int totalCircle = 2;
    private int totalCylinder = 2;
    private int totalSquare = 2;
    private int totalBeerCan = 2;
    private int totalPetBottle = 2;
    private int totalTrashBag = 2;

    public GameObject oneObject; // "One" 오브젝트를 참조할 변수
    private EndLoad endLoadScript; // EndLoad 스크립트를 참조할 변수

    void Start()
    {
        UpdateUI();
        oneObject = GameObject.Find("One");
        if (oneObject != null)
        {
            oneObject.SetActive(false);
            endLoadScript = oneObject.GetComponent<EndLoad>(); // EndLoad 스크립트 참조
        }
    }

    void Update()
    {

    }

    public void UpdateCircleCount()
    {
        circleCount++;
        UpdateUI();
    }

    public void UpdateCylinderCount()
    {
        cylinderCount++;
        UpdateUI();
    }

    public void UpdateSquareCount()
    {
        squareCount++;
        UpdateUI();
    }
    public void UpdateBeerCanCount()
    {
        BeerCanCount++;
        UpdateUI();
    }
    public void UpdatePetBottleCount()
    {
        PetBottleCount++;
        UpdateUI();
    }

    public void UpdateTrashBagCount()
    {
        TrashBagCount++;
        UpdateUI();
    }


    public void UpdateUI()
    {
        if (circleText != null) circleText.text = $"{circleCount}/{totalCircle}";
        if (cylinderText != null) cylinderText.text = $"{cylinderCount}/{totalCylinder}";
        if (squareText != null) squareText.text = $"{squareCount}/{totalSquare}";
        if (BeerCanText != null) BeerCanText.text = $"{BeerCanCount}/{totalBeerCan}"; // 수정된 부분
        if (PetBottleText != null) PetBottleText.text = $"{PetBottleCount}/{totalPetBottle}"; // 수정된 부분
        if (TrashBagText != null) TrashBagText.text = $"{TrashBagCount}/{totalTrashBag}";
        CheckCompletion(circleCount, totalCircle, circleText, circleStrikeThrough);
        CheckCompletion(cylinderCount, totalCylinder, cylinderText, cylinderStrikeThrough);
        CheckCompletion(squareCount, totalSquare, squareText, squareStrikeThrough);
        CheckCompletion(BeerCanCount, totalBeerCan, BeerCanText, BeerCanStrikeThrough);
        CheckCompletion(PetBottleCount, totalPetBottle, PetBottleText, PetBottleStrikeThrough);
        CheckCompletion(TrashBagCount, totalTrashBag, TrashBagText, TrashBagStrikeThrough);
    }


    void CheckCompletion(int count, int total, Text text, GameObject strikeThrough)
    {
        if (text == null || strikeThrough == null) return;

        // 카운트가 목표 수에 도달했는지 확인
        if (count >= total)
        {
            text.color = Color.red;
            strikeThrough.SetActive(true);
        }
        else
        {
            text.color = Color.black;
            strikeThrough.SetActive(false);
        }

        // 모든 쓰레기 종류의 StrikeThrough가 활성화되었는지 확인
        if (circleStrikeThrough.activeSelf && cylinderStrikeThrough.activeSelf && squareStrikeThrough.activeSelf && BeerCanStrikeThrough && PetBottleStrikeThrough && TrashBagStrikeThrough)
        {
            ActivateOneObject();
        }
    }



    void ActivateOneObject()
    {
        if (oneObject != null)
        {
            oneObject.SetActive(true);
            if (endLoadScript != null)
            {
                endLoadScript.OnTriggerEnter(null); // OnCollisionEnter 호출
            }
        }
    }
}


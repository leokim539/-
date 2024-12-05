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

    [Header("쓰레기 사진")]
    public Image circleImage;
    public Image cylinderImage;
    public Image squareImage;

    [Header("쓰레기 빨간줄")]
    public GameObject circleStrikeThrough;
    public GameObject cylinderStrikeThrough;
    public GameObject squareStrikeThrough;

    private int circleCount = 0;
    private int cylinderCount = 0;
    private int squareCount = 0;

    private int totalCircle = 2;
    private int totalCylinder = 2;
    private int totalSquare = 2;

    void Start()
    {
        UpdateUI();
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

    public void UpdateUI()
    {
        // 현재 갯수 알림이
        if (circleText != null) circleText.text = $"{circleCount}/{totalCircle}";
        if (cylinderText != null) cylinderText.text = $"{cylinderCount}/{totalCylinder}";
        if (squareText != null) squareText.text = $"{squareCount}/{totalSquare}";

        // 초과했는지 체크
        CheckCompletion(circleCount, totalCircle, circleText, circleStrikeThrough);
        CheckCompletion(cylinderCount, totalCylinder, cylinderText, cylinderStrikeThrough);
        CheckCompletion(squareCount, totalSquare, squareText, squareStrikeThrough);
    }

    void CheckCompletion(int count, int total, Text text, GameObject strikeThrough)
    {
        if (text == null || strikeThrough == null) return;

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
    }
}

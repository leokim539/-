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

    [Header("������ ����")]
    public Image circleImage;
    public Image cylinderImage;
    public Image squareImage;

    [Header("������ ������")]
    public GameObject circleStrikeThrough;
    public GameObject cylinderStrikeThrough;
    public GameObject squareStrikeThrough;

    private int circleCount = 0;
    private int cylinderCount = 0;
    private int squareCount = 0;

    private int totalCircle = 2;
    private int totalCylinder = 2;
    private int totalSquare = 2;

    public GameObject oneObject; // "One" ������Ʈ�� ������ ����
    private EndLoad endLoadScript; // EndLoad ��ũ��Ʈ�� ������ ����

    void Start()
    {
        UpdateUI();
        oneObject = GameObject.Find("One");
        if (oneObject != null)
        {
            oneObject.SetActive(false);
            endLoadScript = oneObject.GetComponent<EndLoad>(); // EndLoad ��ũ��Ʈ ����
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

    public void UpdateUI()
    {
        if (circleText != null) circleText.text = $"{circleCount}/{totalCircle}";
        if (cylinderText != null) cylinderText.text = $"{cylinderCount}/{totalCylinder}";
        if (squareText != null) squareText.text = $"{squareCount}/{totalSquare}";

        CheckCompletion(circleCount, totalCircle, circleText, circleStrikeThrough);
        CheckCompletion(cylinderCount, totalCylinder, cylinderText, cylinderStrikeThrough);
        CheckCompletion(squareCount, totalSquare, squareText, squareStrikeThrough);
    }

    void CheckCompletion(int count, int total, Text text, GameObject strikeThrough)
    {
        if (text == null || strikeThrough == null) return;

        // ī��Ʈ�� ��ǥ ���� �����ߴ��� Ȯ��
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

        // ��� ������ ������ StrikeThrough�� Ȱ��ȭ�Ǿ����� Ȯ��
        if (circleStrikeThrough.activeSelf && cylinderStrikeThrough.activeSelf && squareStrikeThrough.activeSelf)
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
                endLoadScript.OnTriggerEnter(null); // OnCollisionEnter ȣ��
            }
        }
    }
}


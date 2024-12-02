using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject tutorialPanel;
    public Image tutorialImage; // 가운데 이미지
    public TMP_Text tutorialText;  // 튜토리얼 텍스트
    public Button nextButton;  // 오른쪽 버튼
    public Button backButton; // 왼쪽 버튼

    [Header("Tutorial Data")]
    public Sprite[] tutorialImages; // 튜토리얼 이미지 배열
    public string[] tutorialTexts;  // 튜토리얼 텍스트 배열

    private int currentIndex = 0; // 현재 페이지 인덱스

    void Start()
    {
        // 초기화
        tutorialPanel.SetActive(false);
        nextButton.onClick.AddListener(NextPage);
        backButton.onClick.AddListener(PreviousPage);

        UpdateTutorialUI();
    }

    public void OpenTutorialPanel()
    {
        currentIndex = 0; // 첫 번째 페이지로 초기화
        tutorialPanel.SetActive(true);
        UpdateTutorialUI();
    }

    public void CloseTutorialPanel()
    {
        tutorialPanel.SetActive(false);
    }

    private void UpdateTutorialUI()
    {
        // 이미지와 텍스트 업데이트
        if (currentIndex >= 0 && currentIndex < tutorialImages.Length)
        {
            tutorialImage.sprite = tutorialImages[currentIndex];
            tutorialText.text = tutorialTexts[currentIndex];
        }

        // 버튼 활성화/비활성화 처리
        backButton.gameObject.SetActive(currentIndex > 0); // 첫 번째 페이지에서는 비활성화
        nextButton.gameObject.SetActive(currentIndex < tutorialImages.Length - 1); // 마지막 페이지에서는 비활성화
    }

    private void NextPage()
    {
        if (currentIndex < tutorialImages.Length - 1)
        {
            currentIndex++;
            UpdateTutorialUI();
        }
    }

    private void PreviousPage()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateTutorialUI();
        }
    }
}
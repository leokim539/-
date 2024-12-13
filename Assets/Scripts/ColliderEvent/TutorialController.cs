using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject tutorialPanel; // 튜토리얼 패널
    public Image tutorialImage; // 튜토리얼 이미지
    public Button nextButton; // 다음 버튼
    public Button backButton; // 이전 버튼

    [Header("Tutorial Data")]
    public Sprite[] tutorialImages; // 튜토리얼 이미지 배열

    private int currentIndex = 0; // 현재 페이지 인덱스

    void Start()
    {
        // 필드 유효성 검사 및 초기화
        if (!ValidateFields())
        {
            return;
        }

        tutorialPanel.SetActive(false); // 튜토리얼 패널 초기 비활성화
        nextButton.onClick.AddListener(NextPage);
        backButton.onClick.AddListener(PreviousPage);
    }

    public void OpenTutorialPanel()
    {
        if (!ValidateFields()) return; // 필드 유효성 검사
        if (!ValidateData()) return; // 데이터 유효성 검사

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
        if (!ValidateFields()) return; // 필드 유효성 검사
        if (!ValidateData()) return; // 데이터 유효성 검사

        // 이미지 업데이트
        tutorialImage.sprite = tutorialImages[currentIndex];

        // 버튼 활성화/비활성화 처리
        backButton.gameObject.SetActive(currentIndex > 0); // 첫 번째 페이지에서는 이전 버튼 비활성화
        nextButton.gameObject.SetActive(currentIndex < tutorialImages.Length - 1); // 마지막 페이지에서는 다음 버튼 비활성화
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

    // 필드 유효성 검사 함수
    private bool ValidateFields()
    {
        if (tutorialPanel == null || tutorialImage == null || nextButton == null || backButton == null)
        {
            return false;
        }
        return true;
    }

    // 데이터 유효성 검사 함수
    private bool ValidateData()
    {
        if (tutorialImages == null || tutorialImages.Length == 0)
        {
            return false;
        }
        return true;
    }
}

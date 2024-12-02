using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject tutorialPanel;
    public Image tutorialImage; // ��� �̹���
    public TMP_Text tutorialText;  // Ʃ�丮�� �ؽ�Ʈ
    public Button nextButton;  // ������ ��ư
    public Button backButton; // ���� ��ư

    [Header("Tutorial Data")]
    public Sprite[] tutorialImages; // Ʃ�丮�� �̹��� �迭
    public string[] tutorialTexts;  // Ʃ�丮�� �ؽ�Ʈ �迭

    private int currentIndex = 0; // ���� ������ �ε���

    void Start()
    {
        // �ʱ�ȭ
        tutorialPanel.SetActive(false);
        nextButton.onClick.AddListener(NextPage);
        backButton.onClick.AddListener(PreviousPage);

        UpdateTutorialUI();
    }

    public void OpenTutorialPanel()
    {
        currentIndex = 0; // ù ��° �������� �ʱ�ȭ
        tutorialPanel.SetActive(true);
        UpdateTutorialUI();
    }

    public void CloseTutorialPanel()
    {
        tutorialPanel.SetActive(false);
    }

    private void UpdateTutorialUI()
    {
        // �̹����� �ؽ�Ʈ ������Ʈ
        if (currentIndex >= 0 && currentIndex < tutorialImages.Length)
        {
            tutorialImage.sprite = tutorialImages[currentIndex];
            tutorialText.text = tutorialTexts[currentIndex];
        }

        // ��ư Ȱ��ȭ/��Ȱ��ȭ ó��
        backButton.gameObject.SetActive(currentIndex > 0); // ù ��° ������������ ��Ȱ��ȭ
        nextButton.gameObject.SetActive(currentIndex < tutorialImages.Length - 1); // ������ ������������ ��Ȱ��ȭ
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
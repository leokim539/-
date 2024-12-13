using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject tutorialPanel; // Ʃ�丮�� �г�
    public Image tutorialImage; // Ʃ�丮�� �̹���
    public Button nextButton; // ���� ��ư
    public Button backButton; // ���� ��ư

    [Header("Tutorial Data")]
    public Sprite[] tutorialImages; // Ʃ�丮�� �̹��� �迭

    private int currentIndex = 0; // ���� ������ �ε���

    void Start()
    {
        // �ʵ� ��ȿ�� �˻� �� �ʱ�ȭ
        if (!ValidateFields())
        {
            return;
        }

        tutorialPanel.SetActive(false); // Ʃ�丮�� �г� �ʱ� ��Ȱ��ȭ
        nextButton.onClick.AddListener(NextPage);
        backButton.onClick.AddListener(PreviousPage);
    }

    public void OpenTutorialPanel()
    {
        if (!ValidateFields()) return; // �ʵ� ��ȿ�� �˻�
        if (!ValidateData()) return; // ������ ��ȿ�� �˻�

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
        if (!ValidateFields()) return; // �ʵ� ��ȿ�� �˻�
        if (!ValidateData()) return; // ������ ��ȿ�� �˻�

        // �̹��� ������Ʈ
        tutorialImage.sprite = tutorialImages[currentIndex];

        // ��ư Ȱ��ȭ/��Ȱ��ȭ ó��
        backButton.gameObject.SetActive(currentIndex > 0); // ù ��° ������������ ���� ��ư ��Ȱ��ȭ
        nextButton.gameObject.SetActive(currentIndex < tutorialImages.Length - 1); // ������ ������������ ���� ��ư ��Ȱ��ȭ
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

    // �ʵ� ��ȿ�� �˻� �Լ�
    private bool ValidateFields()
    {
        if (tutorialPanel == null || tutorialImage == null || nextButton == null || backButton == null)
        {
            return false;
        }
        return true;
    }

    // ������ ��ȿ�� �˻� �Լ�
    private bool ValidateData()
    {
        if (tutorialImages == null || tutorialImages.Length == 0)
        {
            return false;
        }
        return true;
    }
}

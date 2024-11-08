using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuController : MonoBehaviour
{
    public GameObject optionsPanel; // OptionsPanel�� ����
    public Button openOptionsButton; // �ɼ� ���� ��ư
    public Button closeButton; // �ݱ� ��ư
    public Button exitButton; // Exit ��ư

    void Start()
    {
        // �ɼ� â ��Ȱ��ȭ
        optionsPanel.SetActive(false);

        // �� ��ư�� �̺�Ʈ ����
        openOptionsButton.onClick.AddListener(OpenOptionsPanel);
        closeButton.onClick.AddListener(CloseOptionsPanel);
        exitButton.onClick.AddListener(ExitGame); // Exit ��ư�� �̺�Ʈ ����
    }

    void OpenOptionsPanel()
    {
        optionsPanel.SetActive(true); // �ɼ� â Ȱ��ȭ
    }

    void CloseOptionsPanel()
    {
        optionsPanel.SetActive(false); // �ɼ� â ��Ȱ��ȭ
    }

    void ExitGame()
    {
        Application.Quit(); // ���� ����

       
    }
}
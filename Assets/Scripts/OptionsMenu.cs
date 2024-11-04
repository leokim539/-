using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuController : MonoBehaviour
{
    public GameObject optionsPanel; // OptionsPanel을 연결
    public Button openOptionsButton; // 옵션 열기 버튼
    public Button closeButton; // 닫기 버튼
    public Button exitButton; // Exit 버튼

    void Start()
    {
        // 옵션 창 비활성화
        optionsPanel.SetActive(false);

        // 각 버튼에 이벤트 연결
        openOptionsButton.onClick.AddListener(OpenOptionsPanel);
        closeButton.onClick.AddListener(CloseOptionsPanel);
        exitButton.onClick.AddListener(ExitGame); // Exit 버튼에 이벤트 연결
    }

    void OpenOptionsPanel()
    {
        optionsPanel.SetActive(true); // 옵션 창 활성화
    }

    void CloseOptionsPanel()
    {
        optionsPanel.SetActive(false); // 옵션 창 비활성화
    }

    void ExitGame()
    {
        Application.Quit(); // 게임 종료

       
    }
}
using UnityEngine;
using UnityEngine.UI;

<<<<<<< Updated upstream
public class OptionsMenu : MonoBehaviour
=======
public class MenuController : MonoBehaviour
>>>>>>> Stashed changes
{
    [Header("Panels")]
    public GameObject optionsPanel;
    public GameObject tutorialPanel;

    [Header("Buttons")]
    public Button openOptionsButton;
    public Button closeOptionsButton;
    public Button exitButton;
    public Button openTutorialButton;
    public Button closeTutorialButton;

    void Start()
    {
        // �г� ��Ȱ��ȭ
        SetPanelActive(optionsPanel, false);
        SetPanelActive(tutorialPanel, false);

        // ��ư �̺�Ʈ ����
        AddButtonListener(openOptionsButton, () => SetPanelActive(optionsPanel, true));
        AddButtonListener(closeOptionsButton, () => SetPanelActive(optionsPanel, false));
        AddButtonListener(openTutorialButton, () => SetPanelActive(tutorialPanel, true));
        AddButtonListener(closeTutorialButton, () => SetPanelActive(tutorialPanel, false));
        AddButtonListener(exitButton, ExitGame);
    }

    void SetPanelActive(GameObject panel, bool isActive)
    {
        if (panel != null)
            panel.SetActive(isActive);
    }

    void AddButtonListener(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button != null)
            button.onClick.AddListener(action);
    }

    void ExitGame()
    {
        Application.Quit();
    }
}
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic; // List�� ����ϱ� ���� �ʿ��� ���ӽ����̽� �߰�

public class SettingsMenu : MonoBehaviour
{
    public Dropdown resolutionDropdown; // �ػ� ��Ӵٿ�
    public Toggle fullscreenToggle;     // ��üȭ�� ���

    private Resolution[] resolutions;   // �ý��ۿ��� ����� �� �ִ� �ػ� ��� ����

    void Start()
    {
        // �ػ� ��� ��������
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        // �ػ� ����� ���ڿ��� ��ȯ�Ͽ� ��Ӵٿ� �ɼ����� �߰�
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            // ���� �ػ󵵿� ��ġ�ϴ� �ػ� ã��
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // ���� â��� ���� �ݿ�
        fullscreenToggle.isOn = Screen.fullScreen;

        // �̺�Ʈ ������ �߰�
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    // �ػ� ���� �޼���
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    // ��üȭ�� ���� �޼���
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
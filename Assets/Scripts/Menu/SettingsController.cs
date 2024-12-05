using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    public Slider volumeSlider;          // ��ü ���� �����̴�
    public Slider sensitivitySlider;     // ���콺 ���� �����̴�
    public FirstPersonLook lookScript;   // ���콺 ���� ������ ���� FirstPersonLook ��ũ��Ʈ ����

    void Start()
    {
        // �ʱ� �����̴� �� ����
        volumeSlider.value = AudioListener.volume;           // ���� ��ü ������ �����̴� �ʱ� ������ ����
        sensitivitySlider.value = lookScript.sensitivity;    // ���� ���콺 ������ �����̴� �ʱ� ������ ����

        // �����̴� �� ���� �̺�Ʈ ����
        volumeSlider.onValueChanged.AddListener(SetVolume);
        sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
    }

    // ��ü ���� ���� �޼���
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    // ���콺 ���� ���� �޼���
    public void SetSensitivity(float sensitivity)
    {
        lookScript.sensitivity = sensitivity;
    }
}
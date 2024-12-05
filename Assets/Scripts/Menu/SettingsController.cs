using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class SettingsController : MonoBehaviourPunCallbacks
{
    public Slider volumeSlider;          // ��ü ���� �����̴�
    public Slider sensitivitySlider;     // ���콺 ���� �����̴�
    public FirstPersonController lookScript;   // ���콺 ���� ������ ���� FirstPersonLook ��ũ��Ʈ ����

    private float cameraMove = 2f;
    void Start()
    {
        if (sensitivitySlider == null)
        {
            Debug.LogError("sensitivitySlider is not assigned in the inspector.");
            return;
        }

        if (lookScript == null)
        {
            Debug.LogError("lookScript is not assigned in the inspector.");
            return;
        }
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
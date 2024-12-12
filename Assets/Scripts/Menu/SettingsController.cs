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

        // �����̴� �� ���� �̺�Ʈ ����
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    // ��ü ���� ���� �޼���
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
public class SettingsController : MonoBehaviourPunCallbacks
{
    public Slider volumeSlider;          // 전체 볼륨 슬라이더
    public Slider sensitivitySlider;     // 마우스 감도 슬라이더
    public FirstPersonController lookScript;   // 마우스 감도 조절을 위한 FirstPersonLook 스크립트 참조

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
        // 초기 슬라이더 값 설정
        volumeSlider.value = AudioListener.volume;           // 현재 전체 볼륨을 슬라이더 초기 값으로 설정
        sensitivitySlider.value = lookScript.sensitivity;    // 현재 마우스 감도를 슬라이더 초기 값으로 설정

        // 슬라이더 값 변경 이벤트 연결
        volumeSlider.onValueChanged.AddListener(SetVolume);
        sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
    }

    // 전체 볼륨 조절 메서드
    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    // 마우스 감도 조절 메서드
    public void SetSensitivity(float sensitivity)
    {
        lookScript.sensitivity = sensitivity;
    }
}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public Image[] images; // 사용할 이미지 배열
    public AudioClip[] audioClips; // 사용할 오디오 클립 배열
    public AudioSource audioSource; // AudioSource 컴포넌트

    public Text timerText; // 시계
    public float totalTime; // 총 시간 (초 단위)
    private float remainingTime;//줄어들 시간
    void Start()
    {
        remainingTime = totalTime;//시간 초기화
    }

    void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime; // 시간 계산
            UpdateTimerDisplay(); // 시계 표시
            if (remainingTime <= 100f && remainingTime > 100f - Time.deltaTime)
            {
                StartCoroutine(TriggerEvent()); // 이벤트 생성
                Debug.Log("100");
            }
            if (remainingTime <= 90f && remainingTime > 90f - Time.deltaTime)
            {
                StartCoroutine(TriggerEvent()); // 이벤트 생성
                Debug.Log("90");
            }
            if (remainingTime <= 80f && remainingTime > 80f - Time.deltaTime)
            {
                StartCoroutine(TriggerEvent()); // 이벤트 생성
                Debug.Log("80");
            }
        }
        else
        {
            remainingTime = 0;
            timerText.color = Color.red;// 시간이 끝났을 때 색상 변경
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60); // 분 계산
        int seconds = Mathf.FloorToInt(remainingTime % 60); // 초 계산
        timerText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds); // 표시
    }
    IEnumerator TriggerEvent()
    {
        int randomImageIndex = Random.Range(0, images.Length);// 랜덤 이미지 설정
        foreach (var img in images)
        {
            img.gameObject.SetActive(false); // 모든 이미지 비활성화
        }
        images[randomImageIndex].gameObject.SetActive(true); // 랜덤 이미지 활성화

        // 랜덤 소리 선택
        int randomSoundIndex = Random.Range(0, audioClips.Length);
        audioSource.clip = audioClips[randomSoundIndex]; // 랜덤 소리 설정
        audioSource.Play(); // 소리 재생

        if (images.Length == 0 || audioClips.Length == 0)
        {
            yield break;
        }

        yield return new WaitForSeconds(5);

        // 비활성화
        images[randomImageIndex].gameObject.SetActive(false); // 이미지 비활성화
        audioSource.Stop(); // 소리 정지
    }
}
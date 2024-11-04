using UnityEngine;
using System.Collections;

public class RainSoundManager : MonoBehaviour
{
    public AudioSource rainSound;       // 하나의 빗소리 오디오 소스
    public float nearVolume = 1.0f;     // 창문 근처에서의 최대 볼륨
    public float farVolume = 0.05f;      // 기본 상태의 낮은 볼륨
    public float fadeDuration = 1.0f;   // 볼륨 변화에 걸리는 시간 (초 단위)

    private Coroutine fadeCoroutine;    // 현재 실행 중인 볼륨 조절 코루틴

    private void Start()
    {
        // 빗소리의 초기 볼륨을 낮게 설정
        if (rainSound != null)
        {
            rainSound.volume = farVolume;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어가 "RainZone" 태그의 콜라이더에 들어갔을 때 볼륨을 서서히 높임
        if (other.CompareTag("RainZone") && rainSound != null)
        {
            // 실행 중인 코루틴이 있으면 중지
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

            // 코루틴을 통해 볼륨을 점진적으로 nearVolume으로 증가
            fadeCoroutine = StartCoroutine(FadeVolume(nearVolume));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 플레이어가 "RainZone" 태그의 콜라이더에서 나갔을 때 볼륨을 서서히 낮춤
        if (other.CompareTag("RainZone") && rainSound != null)
        {
            // 실행 중인 코루틴이 있으면 중지
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

            // 코루틴을 통해 볼륨을 점진적으로 farVolume으로 감소
            fadeCoroutine = StartCoroutine(FadeVolume(farVolume));
        }
    }

    private IEnumerator FadeVolume(float targetVolume)
    {
        float startVolume = rainSound.volume;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            // 시간에 따라 볼륨을 점진적으로 변경
            rainSound.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 최종 볼륨을 목표 볼륨에 맞춤
        rainSound.volume = targetVolume;
    }
}
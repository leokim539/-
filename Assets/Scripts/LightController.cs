using System.Collections;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public Light[] lights;            // 여러 라이트를 담을 배열
    public float delayTime = 5.0f;     // 불을 끄기 전 대기 시간
    public int blinkCount = 3;         // 깜빡거릴 횟수
    public float blinkInterval = 0.2f; // 깜빡거림 간격 (초 단위)

    private void Start()
    {
        // 일정 시간이 지난 후 라이트를 끄는 코루틴 실행
        StartCoroutine(TurnOffLightsAfterBlink());
    }

    private IEnumerator TurnOffLightsAfterBlink()
    {
        // 지정된 시간만큼 대기
        yield return new WaitForSeconds(delayTime);

        // 설정한 횟수만큼 깜빡거림
        for (int i = 0; i < blinkCount; i++)
        {
            foreach (Light light in lights)
            {
                light.enabled = !light.enabled; // 라이트 상태 반전
            }
            yield return new WaitForSeconds(blinkInterval); // 깜빡거림 간격 대기
        }

        // 깜빡거림이 끝난 후 모든 라이트 끄기
        foreach (Light light in lights)
        {
            light.enabled = false;
        }
    }
}
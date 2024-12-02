using UnityEngine;
using System.Collections;

public class EffectManager : MonoBehaviour
{
    private float effectDuration = 8f; // 효과 지속 시간
    private GameObject activeVolume; // 현재 활성화된 GameObject 추적
    private Coroutine effectCoroutine; // 효과 코루틴 추적

    // EffectTrash에서 호출될 메서드 (GameObject을 파라미터로 받음)
    public void ActivatePostProcessEffect(GameObject postProcessObject)
    {
        if (postProcessObject != null)
        {
            // 직접 전달받은 GameObject 활성화
            activeVolume = postProcessObject;
            activeVolume.SetActive(true); // 효과 즉시 활성화
            effectCoroutine = StartCoroutine(ActivateEffectTemporarily(postProcessObject));
        }
        if (postProcessObject != null)
        {
            // 자식 오브젝트 중 GameObject 찾아서 활성화
            GameObject[] childVolumes = GetComponentsInChildren<GameObject>();
            if (childVolumes.Length > 0)
            {
                activeVolume = childVolumes[0];
                activeVolume.SetActive(true); // 효과 즉시 활성화
                effectCoroutine = StartCoroutine(ActivateEffectTemporarily(childVolumes[0]));
            }
        }
    }

    private IEnumerator ActivateEffectTemporarily(GameObject volume)
    {
        // 지정된 시간 동안 대기
        yield return new WaitForSeconds(effectDuration);

        // GameObject 비활성화
        volume.SetActive(false);
        activeVolume = null;
        effectCoroutine = null;
    }

    public void DeactivatePostProcessEffect()
    {
        if (activeVolume != null)
        {
            activeVolume.SetActive(false); // GameObject 비활성화
            activeVolume = null; // 활성화된 포스트 프로세스 초기화
        }
    }
}

using UnityEngine;
using System.Collections;

public class FlashLightPickUp : MonoBehaviour
{
    public GameObject interactionUI; // 상호작용 UI
    public AudioClip flashlightPickupSound; // 손전등 수집 소리
    private bool isFlashlightCollected = false; // 손전등 수집 상태
    private AudioSource audioSource;
    public float interactionDistance = 3f; // 상호작용 거리

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        if (interactionUI != null)
        {
            interactionUI.SetActive(false); // 시작 시 UI 비활성화
        }
    }

    private void Update()
    {
        if (isFlashlightCollected) return; // 이미 수집된 손전등이라면 다른 동작을 하지 않음

        RaycastHit hit;

        // 레이캐스트를 통해 감지된 오브젝트를 확인합니다.
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactionDistance))
        {
            // 레이캐스트에 감지된 오브젝트가 "FlashLight" 태그를 가지고 있을 때
            if (hit.transform.CompareTag("FlashLight"))
            {
                // 해당 오브젝트의 FlashLightPickUp 스크립트를 가져옵니다.
                FlashLightPickUp flashlightEvent = hit.transform.GetComponent<FlashLightPickUp>();
                if (flashlightEvent != null && flashlightEvent.interactionUI != null)
                {
                    // FlashLight 태그를 가진 오브젝트의 interactionUI를 활성화합니다.
                    flashlightEvent.interactionUI.SetActive(true);

                    // F 키를 눌렀을 때만 수집
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        StartCoroutine(flashlightEvent.CollectFlashlight()); // 손전등 수집 메서드 호출
                    }
                }
            }
            else
            {
                // FlashLight 태그가 아닌 오브젝트를 바라보면 UI 비활성화
                FlashLightPickUp[] flashlights = FindObjectsOfType<FlashLightPickUp>();
                foreach (FlashLightPickUp flashlight in flashlights)
                {
                    if (flashlight.interactionUI != null)
                    {
                        flashlight.interactionUI.SetActive(false);
                    }
                }
            }
        }
        else
        {
            // 레이캐스트로 감지된 오브젝트가 없으면 모든 UI 비활성화
            FlashLightPickUp[] flashlights = FindObjectsOfType<FlashLightPickUp>();
            foreach (FlashLightPickUp flashlight in flashlights)
            {
                if (flashlight.interactionUI != null)
                {
                    flashlight.interactionUI.SetActive(false);
                }
            }
        }
    }

    private IEnumerator CollectFlashlight()
    {
        isFlashlightCollected = true; // 손전등 수집 상태로 변경
        // UI 비활성화
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
        if (flashlightPickupSound != null)
        {
            audioSource.PlayOneShot(flashlightPickupSound); // 손전등 수집 소리 재생
        }

        // 손전등 수집 애니메이션 시작 (필요시 추가)
        yield return StartCoroutine(CollectFlashlightAnimation()); // 애니메이션 코루틴 호출

        // 손전등 오브젝트 삭제
        Destroy(gameObject); // 손전등 오브젝트 삭제
    }

    private IEnumerator CollectFlashlightAnimation()
    {
        // 손전등 오브젝트가 제거되는 애니메이션을 구현할 수 있습니다.
        // 예: 크기 감소 및 이동 애니메이션 등

        // 애니메이션 구현 예시
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = Vector3.zero; // 최종 크기
        float duration = 1f; // 이동 및 축소 시간
        float elapsedTime = 0f;

        // 애니메이션 수행
        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}

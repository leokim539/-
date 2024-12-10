using UnityEngine;
using System.Collections;

public class KeyEvent : MonoBehaviour
{
    public GameObject interactionUI;
    public DoorController doorToUnlock; // 특정 문 오브젝트를 할당하기 위한 변수
    public float interactionDistance = 3f;
    public AudioClip keyPickupSound;
    private bool isKeyCollected = false;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
        // 시작 시 지정된 문의 DoorController 비활성화
        if (doorToUnlock != null)
        {
            doorToUnlock.enabled = false;
            doorToUnlock.requiresUnlocking = true;
            doorToUnlock.firstInteraction = true;
        }
    }

    private void Update()
    {
        if (isKeyCollected) return; // 이미 수집된 키라면 다른 동작을 하지 않음
        RaycastHit hit;

        // 레이캐스트를 통해 감지된 오브젝트를 확인합니다.
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactionDistance))
        {
            // 레이캐스트에 감지된 오브젝트가 "Key" 태그를 가지고 있을 때
            if (hit.transform.CompareTag("Key"))
            {
                // 해당 오브젝트의 KeyEvent 스크립트를 가져옵니다.
                KeyEvent keyEvent = hit.transform.GetComponent<KeyEvent>();
                if (keyEvent != null && keyEvent.interactionUI != null)
                {
                    // Key 태그를 가진 오브젝트의 interactionUI를 활성화합니다.
                    keyEvent.interactionUI.SetActive(true);

                    // F 키를 눌렀을 때만 수집
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        keyEvent.CollectKey(); // 키 수집 메서드 호출
                    }
                }
            }
            else
            {
                // Key 태그가 아닌 오브젝트를 바라보면 UI 비활성화
                KeyEvent[] keys = FindObjectsOfType<KeyEvent>();
                foreach (KeyEvent key in keys)
                {
                    if (key.interactionUI != null)
                    {
                        key.interactionUI.SetActive(false);
                    }
                }
            }
        }
        else
        {
            // 레이캐스트로 감지된 오브젝트가 없으면 모든 UI 비활성화
            KeyEvent[] keys = FindObjectsOfType<KeyEvent>();
            foreach (KeyEvent key in keys)
            {
                if (key.interactionUI != null)
                {
                    key.interactionUI.SetActive(false);
                }
            }
        }
    }

    private void CollectKey()
    {
        isKeyCollected = true; // 키 수집 상태로 변경
        // UI 비활성화
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
        if (keyPickupSound != null)
        {
            audioSource.PlayOneShot(keyPickupSound);
        }
        // 특정 문만 활성화
        if (doorToUnlock != null)
        {
            doorToUnlock.enabled = true;
            doorToUnlock.requiresUnlocking = true;
            doorToUnlock.firstInteraction = true;

            // lockOpenUI 비활성화
            if (doorToUnlock.GetComponent<LockDoor>().lockOpenUI != null)
            {
                doorToUnlock.GetComponent<LockDoor>().lockOpenUI.SetActive(false);
            }
        }

        // 키 수집 애니메이션 시작
        StartCoroutine(CollectKeyAnimation());
    }

    private IEnumerator CollectKeyAnimation()
    {
        // 콜라이더 비활성화
        Collider keyCollider = GetComponent<Collider>();
        if (keyCollider != null)
        {
            keyCollider.enabled = false; // 콜라이더 비활성화
        }

        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = Vector3.zero; // 최종 크기
        float duration = 1f; // 이동 및 축소 시간
        float elapsedTime = 0f;

        // 플레이어 위치 가져오기 (여기서는 카메라 위치로 가정)
        Transform playerTransform = Camera.main.transform;

        while (elapsedTime < duration)
        {
            // 플레이어 방향으로 이동
            Vector3 dir = playerTransform.position - transform.position;
            dir.y = 0; // Y축을 0으로 설정하여 수평 이동만 하도록 함
            dir.Normalize(); // 방향 벡터 정규화

            // 아이템을 플레이어 쪽으로 이동
            transform.position += dir * (3f * Time.deltaTime); // 속도 조정

            // 크기 줄이기
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / (duration / 3));
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // 아이템 비활성화
        Destroy(gameObject); // 아이템을 삭제
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.UI; // UI 관련 네임스페이스 추가

public class DrawerEventTrigger : MonoBehaviour
{
    public float moveDistance = 1f; // 이동할 거리
    public float moveDuration = 2f; // 이동에 걸리는 시간
    private bool isMoving = false; // 이동 상태
    private Transform currentDrawer; // 현재 열려 있는 서랍의 Transform

    // 오디오 클립
    public AudioClip openClip; // 서랍 열기 소리
    public AudioClip closeClip; // 서랍 닫기 소리

    private AudioSource audioSource; // 오디오 소스

    // UI 관련 변수
    public GameObject interactionUI; // 상호작용 UI

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>(); // 오디오 소스 추가
        interactionUI.SetActive(false); // 시작 시 UI 비활성화
    }

    void Update()
    {
        // E 키 입력 체크
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;

            // 플레이어 위치에서 정면으로 레이캐스트
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 3f))
            {
                Transform hitTransform = hit.transform;

                // 레이캐스트로 감지한 오브젝트의 태그에 따라 서랍 열기 또는 닫기
                if (hitTransform.CompareTag("Drawer") || hitTransform.CompareTag("Drawer2") || hitTransform.CompareTag("Drawer3"))
                {
                    if (!isMoving) // 이동 중이 아닐 때만 실행
                    {
                        // 현재 열려 있는 서랍이 있다면
                        if (currentDrawer != null)
                        {
                            // 열린 서랍을 닫기
                            if (currentDrawer == hitTransform) // 같은 서랍이면 닫기
                            {
                                StartCoroutine(MoveDrawer(currentDrawer, -moveDistance)); // 서랍 닫기
                            }
                        }
                        else
                        {
                            // 새로운 서랍으로 설정하고 열기
                            currentDrawer = hitTransform;
                            StartCoroutine(MoveDrawer(currentDrawer, moveDistance)); // 서랍 열기
                        }
                    }
                }
            }
        }

        // 레이캐스트를 통해 UI 활성화/비활성화
        RaycastHit hitInteraction;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInteraction, 3f))
        {
            if (hitInteraction.transform.CompareTag("Drawer") || hitInteraction.transform.CompareTag("Drawer2") || hitInteraction.transform.CompareTag("Drawer3"))
            {
                interactionUI.SetActive(true); // UI 활성화
            }
            else
            {
                interactionUI.SetActive(false); // UI 비활성화
            }
        }
        else
        {
            interactionUI.SetActive(false); // UI 비활성화
        }
    }

    public IEnumerator MoveDrawer(Transform drawer, float direction) // IEnumerator로 정의
    {
        isMoving = true; // 이동 시작 상태로 변경

        // 오디오 클립 선택
        audioSource.clip = direction > 0 ? openClip : closeClip;
        if (audioSource.clip != null)
        {
            audioSource.Play(); // 서랍 열기 또는 닫기 오디오 재생
        }

        Vector3 startPosition = drawer.position; // 시작 위치
        Vector3 targetPosition = startPosition + new Vector3(0, 0, direction); // 목표 위치

        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            drawer.position = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null; // 다음 프레임까지 대기
        }

        drawer.position = targetPosition; // 최종 위치 설정
        isMoving = false; // 이동 완료 상태로 변경

        // 서랍이 닫히면 currentDrawer를 null로 설정
        if (direction < 0)
        {
            currentDrawer = null; // 서랍이 닫혔으므로 현재 서랍 초기화
        }
        else
        {
            // 서랍이 열리면 currentDrawer를 해당 서랍으로 설정
            currentDrawer = drawer;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && currentDrawer == null) // 열려있는 서랍이 없을 때만 실행
        {
            // 첫 번째 서랍으로 설정
            Transform firstDrawer = FindDrawer();
            if (firstDrawer != null && !isMoving)
            {
                StartCoroutine(MoveDrawer(firstDrawer, moveDistance)); // 서랍 열기
                currentDrawer = firstDrawer; // 현재 서랍 설정
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        // 서랍을 닫지 않도록 로직을 변경
    }

    private Transform FindDrawer()
    {
        // Drawer 태그를 가진 자식 오브젝트 찾기
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Drawer"))
            {
                return child; // 첫 번째 서랍만 저장
            }
        }
        return null; // 서랍을 찾지 못한 경우 null 반환
    }
}

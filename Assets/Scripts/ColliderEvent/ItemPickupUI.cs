using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickupUI : MonoBehaviour
{
    [Header("슬라이드 애니메이션 관련")]
    public RectTransform itemImageRect; // 아이템 이미지의 RectTransform
    public Image itemImage; // UI에 표시될 아이템 이미지
    public float slideDuration = 0.5f; // 슬라이드 애니메이션 시간
    public float stayDuration = 1f; // 화면에 머무는 시간
    public Vector2 startPosition = new Vector2(-200, 0); // 시작 위치
    public Vector2 endPosition = new Vector2(95, 0); // 끝 위치

    [Header("게임 오브젝트 및 이미지 매핑")]
    public GameObject[] gameObjects; // 게임 오브젝트 배열
    public Sprite[] objectImages; // 게임 오브젝트에 매칭될 이미지 배열

    [Header("상호작용 거리")]
    public float interactionDistance = 5f; // 상호작용 거리

    private Transform playerTransform; // 플레이어 Transform
    private bool isSliding = false; // 현재 슬라이드 애니메이션 진행 중인지 여부

    void Start()
    {
        // Player 태그를 가진 오브젝트의 Transform을 찾음
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player 태그를 가진 오브젝트를 찾을 수 없습니다. Player 태그를 확인해주세요.");
        }

        itemImageRect.anchoredPosition = startPosition;
        itemImageRect.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && playerTransform != null)
        {
            HandleInteraction();
        }
    }

    private void HandleInteraction()
    {
        // 배열에서 유효한 첫 번째 오브젝트 찾기
        for (int i = 0; i < gameObjects.Length; i++)
        {
            if (gameObjects[i] != null)
            {
                // 오브젝트와의 거리 계산
                float distance = Vector3.Distance(playerTransform.position, gameObjects[i].transform.position);

                // 거리 조건 확인
                if (distance <= interactionDistance)
                {
                    // UI 표시 및 오브젝트 제거
                    ShowItemUI(objectImages[i]);
                    Destroy(gameObjects[i]);

                    // 배열 정리
                    gameObjects[i] = null;
                    return;
                }
                else
                {
                    Debug.Log($"오브젝트가 너무 멀어서 상호작용할 수 없습니다. 현재 거리: {distance}");
                }
            }
        }

        Debug.Log("상호작용 가능한 오브젝트가 없습니다.");
    }

    public void ShowItemUI(Sprite sprite)
    {
        if (!isSliding)
        {
            StartCoroutine(SlideUI(sprite));
        }
    }

    IEnumerator SlideUI(Sprite sprite)
    {
        isSliding = true;

        // 이미지 설정
        itemImage.sprite = sprite;
        itemImageRect.gameObject.SetActive(true);

        // 왼쪽에서 오른쪽으로 슬라이드
        yield return StartCoroutine(Slide(startPosition, endPosition, slideDuration));

        // 일정 시간 동안 머무름
        yield return new WaitForSeconds(stayDuration);

        // 오른쪽에서 왼쪽으로 슬라이드
        yield return StartCoroutine(Slide(endPosition, startPosition, slideDuration));

        // UI 비활성화
        itemImageRect.gameObject.SetActive(false);

        isSliding = false;
    }

    IEnumerator Slide(Vector2 from, Vector2 to, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            itemImageRect.anchoredPosition = Vector2.Lerp(from, to, elapsedTime / duration);
            yield return null;
        }

        itemImageRect.anchoredPosition = to;
    }
}

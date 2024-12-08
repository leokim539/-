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

    [Header("Trash2 참조")]
    public Trash2 trash2; // Trash2 컴포넌트를 참조

    private bool isSliding = false; // 현재 슬라이드 애니메이션 진행 중인지 여부

    void Start()
    {
        if (trash2 == null)
        {
            Debug.LogWarning("Trash2 컴포넌트가 연결되지 않았습니다. Manager에서 Trash2를 연결해주세요.");
        }

        itemImageRect.anchoredPosition = startPosition;
        itemImageRect.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && trash2 != null)
        {
            HandleInteraction();
        }
    }

    private void HandleInteraction()
    {
        // Trash2의 첫 번째 gameObjects 오브젝트와 상호작용
        if (gameObjects.Length > 0 && gameObjects[0] != null)
        {
            // UI 표시 및 첫 번째 오브젝트 제거
            ShowItemUI(objectImages[0]);
            Destroy(gameObjects[0]);

            // 배열 정리 (필요시)
            gameObjects[0] = null; // 첫 번째 오브젝트를 null 처리
        }
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

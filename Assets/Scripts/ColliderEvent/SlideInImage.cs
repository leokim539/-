using System;
using UnityEngine;

public class SlideInImage : MonoBehaviour
{
    public RectTransform imageRectTransform; // 이동할 이미지의 RectTransform
    public Vector2 targetPosition = new Vector2(0, 0); // 이미지가 도착할 위치
    public float moveSpeed = 500f; // 이동 속도

    private Vector2 startPosition; // 이미지의 초기 위치
    private bool isMoving = false;
    private Vector2 currentTargetPosition; // 현재 목표 위치

    // 콜백 이벤트
    public event Action OnSlideComplete;

    void Start()
    {
        // 시작 위치를 현재 위치로 설정
        startPosition = imageRectTransform.anchoredPosition;

        // 이미지를 초기 위치로 설정 (아래로 숨김)
        imageRectTransform.anchoredPosition = new Vector2(startPosition.x, startPosition.y - 1000);

        // 초기 목표 위치 설정
        currentTargetPosition = targetPosition;
        isMoving = true;
    }

    void Update()
    {
        if (isMoving)
        {
            // 현재 위치를 목표 위치로 점진적으로 이동
            imageRectTransform.anchoredPosition = Vector2.MoveTowards(
                imageRectTransform.anchoredPosition,
                currentTargetPosition,
                moveSpeed * Time.deltaTime
            );

            // 목표 위치에 도달하면 이동 중지 및 이벤트 호출
            if (Vector2.Distance(imageRectTransform.anchoredPosition, currentTargetPosition) < 0.1f)
            {
                isMoving = false;
                OnSlideComplete?.Invoke(); // 콜백 호출
            }
        }
    }

    public void SlideDown(Vector2 customTargetPosition)
    {
        currentTargetPosition = customTargetPosition;
        isMoving = true;
    }
}
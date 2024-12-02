using System;
using UnityEngine;

public class SlideInImage : MonoBehaviour
{
    public RectTransform imageRectTransform; // �̵��� �̹����� RectTransform
    public Vector2 targetPosition = new Vector2(0, 0); // �̹����� ������ ��ġ
    public float moveSpeed = 500f; // �̵� �ӵ�

    private Vector2 startPosition; // �̹����� �ʱ� ��ġ
    private bool isMoving = false;
    private Vector2 currentTargetPosition; // ���� ��ǥ ��ġ

    // �ݹ� �̺�Ʈ
    public event Action OnSlideComplete;

    void Start()
    {
        // ���� ��ġ�� ���� ��ġ�� ����
        startPosition = imageRectTransform.anchoredPosition;

        // �̹����� �ʱ� ��ġ�� ���� (�Ʒ��� ����)
        imageRectTransform.anchoredPosition = new Vector2(startPosition.x, startPosition.y - 1000);

        // �ʱ� ��ǥ ��ġ ����
        currentTargetPosition = targetPosition;
        isMoving = true;
    }

    void Update()
    {
        if (isMoving)
        {
            // ���� ��ġ�� ��ǥ ��ġ�� ���������� �̵�
            imageRectTransform.anchoredPosition = Vector2.MoveTowards(
                imageRectTransform.anchoredPosition,
                currentTargetPosition,
                moveSpeed * Time.deltaTime
            );

            // ��ǥ ��ġ�� �����ϸ� �̵� ���� �� �̺�Ʈ ȣ��
            if (Vector2.Distance(imageRectTransform.anchoredPosition, currentTargetPosition) < 0.1f)
            {
                isMoving = false;
                OnSlideComplete?.Invoke(); // �ݹ� ȣ��
            }
        }
    }

    public void SlideDown(Vector2 customTargetPosition)
    {
        currentTargetPosition = customTargetPosition;
        isMoving = true;
    }
}
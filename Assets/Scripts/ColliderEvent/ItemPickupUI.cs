using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickupUI : MonoBehaviour
{
    [Header("�����̵� �ִϸ��̼� ����")]
    public RectTransform itemImageRect; // ������ �̹����� RectTransform
    public Image itemImage; // UI�� ǥ�õ� ������ �̹���
    public float slideDuration = 0.5f; // �����̵� �ִϸ��̼� �ð�
    public float stayDuration = 1f; // ȭ�鿡 �ӹ��� �ð�
    public Vector2 startPosition = new Vector2(-200, 0); // ���� ��ġ
    public Vector2 endPosition = new Vector2(95, 0); // �� ��ġ

    [Header("���� ������Ʈ �� �̹��� ����")]
    public GameObject[] gameObjects; // ���� ������Ʈ �迭
    public Sprite[] objectImages; // ���� ������Ʈ�� ��Ī�� �̹��� �迭

    [Header("��ȣ�ۿ� �Ÿ�")]
    public float interactionDistance = 5f; // ��ȣ�ۿ� �Ÿ�

    private Transform playerTransform; // �÷��̾� Transform
    private bool isSliding = false; // ���� �����̵� �ִϸ��̼� ���� ������ ����

    void Start()
    {
        // Player �±׸� ���� ������Ʈ�� Transform�� ã��
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player �±׸� ���� ������Ʈ�� ã�� �� �����ϴ�. Player �±׸� Ȯ�����ּ���.");
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
        // �迭���� ��ȿ�� ù ��° ������Ʈ ã��
        for (int i = 0; i < gameObjects.Length; i++)
        {
            if (gameObjects[i] != null)
            {
                // ������Ʈ���� �Ÿ� ���
                float distance = Vector3.Distance(playerTransform.position, gameObjects[i].transform.position);

                // �Ÿ� ���� Ȯ��
                if (distance <= interactionDistance)
                {
                    // UI ǥ�� �� ������Ʈ ����
                    ShowItemUI(objectImages[i]);
                    Destroy(gameObjects[i]);

                    // �迭 ����
                    gameObjects[i] = null;
                    return;
                }
                else
                {
                    Debug.Log($"������Ʈ�� �ʹ� �־ ��ȣ�ۿ��� �� �����ϴ�. ���� �Ÿ�: {distance}");
                }
            }
        }

        Debug.Log("��ȣ�ۿ� ������ ������Ʈ�� �����ϴ�.");
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

        // �̹��� ����
        itemImage.sprite = sprite;
        itemImageRect.gameObject.SetActive(true);

        // ���ʿ��� ���������� �����̵�
        yield return StartCoroutine(Slide(startPosition, endPosition, slideDuration));

        // ���� �ð� ���� �ӹ���
        yield return new WaitForSeconds(stayDuration);

        // �����ʿ��� �������� �����̵�
        yield return StartCoroutine(Slide(endPosition, startPosition, slideDuration));

        // UI ��Ȱ��ȭ
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

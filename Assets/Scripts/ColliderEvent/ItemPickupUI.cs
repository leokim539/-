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

    [Header("Trash2 ����")]
    public Trash2 trash2; // Trash2 ������Ʈ�� ����

    private bool isSliding = false; // ���� �����̵� �ִϸ��̼� ���� ������ ����

    void Start()
    {
        if (trash2 == null)
        {
            Debug.LogWarning("Trash2 ������Ʈ�� ������� �ʾҽ��ϴ�. Manager���� Trash2�� �������ּ���.");
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
        // Trash2�� ù ��° gameObjects ������Ʈ�� ��ȣ�ۿ�
        if (gameObjects.Length > 0 && gameObjects[0] != null)
        {
            // UI ǥ�� �� ù ��° ������Ʈ ����
            ShowItemUI(objectImages[0]);
            Destroy(gameObjects[0]);

            // �迭 ���� (�ʿ��)
            gameObjects[0] = null; // ù ��° ������Ʈ�� null ó��
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

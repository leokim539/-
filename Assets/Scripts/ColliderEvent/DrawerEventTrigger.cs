using System.Collections;
using UnityEngine;
using UnityEngine.UI; // UI ���� ���ӽ����̽� �߰�

public class DrawerEventTrigger : MonoBehaviour
{
    public float moveDistance = 1f; // �̵��� �Ÿ�
    public float moveDuration = 2f; // �̵��� �ɸ��� �ð�
    private bool isMoving = false; // �̵� ����
    private Transform currentDrawer; // ���� ���� �ִ� ������ Transform

    // ����� Ŭ��
    public AudioClip openClip; // ���� ���� �Ҹ�
    public AudioClip closeClip; // ���� �ݱ� �Ҹ�

    private AudioSource audioSource; // ����� �ҽ�

    // UI ���� ����
    public GameObject interactionUI; // ��ȣ�ۿ� UI

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>(); // ����� �ҽ� �߰�
        interactionUI.SetActive(false); // ���� �� UI ��Ȱ��ȭ
    }

    void Update()
    {
        // E Ű �Է� üũ
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;

            // �÷��̾� ��ġ���� �������� ����ĳ��Ʈ
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 3f))
            {
                Transform hitTransform = hit.transform;

                // ����ĳ��Ʈ�� ������ ������Ʈ�� �±׿� ���� ���� ���� �Ǵ� �ݱ�
                if (hitTransform.CompareTag("Drawer") || hitTransform.CompareTag("Drawer2") || hitTransform.CompareTag("Drawer3"))
                {
                    if (!isMoving) // �̵� ���� �ƴ� ���� ����
                    {
                        // ���� ���� �ִ� ������ �ִٸ�
                        if (currentDrawer != null)
                        {
                            // ���� ������ �ݱ�
                            if (currentDrawer == hitTransform) // ���� �����̸� �ݱ�
                            {
                                StartCoroutine(MoveDrawer(currentDrawer, -moveDistance)); // ���� �ݱ�
                            }
                        }
                        else
                        {
                            // ���ο� �������� �����ϰ� ����
                            currentDrawer = hitTransform;
                            StartCoroutine(MoveDrawer(currentDrawer, moveDistance)); // ���� ����
                        }
                    }
                }
            }
        }

        // ����ĳ��Ʈ�� ���� UI Ȱ��ȭ/��Ȱ��ȭ
        RaycastHit hitInteraction;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInteraction, 3f))
        {
            if (hitInteraction.transform.CompareTag("Drawer") || hitInteraction.transform.CompareTag("Drawer2") || hitInteraction.transform.CompareTag("Drawer3"))
            {
                interactionUI.SetActive(true); // UI Ȱ��ȭ
            }
            else
            {
                interactionUI.SetActive(false); // UI ��Ȱ��ȭ
            }
        }
        else
        {
            interactionUI.SetActive(false); // UI ��Ȱ��ȭ
        }
    }

    public IEnumerator MoveDrawer(Transform drawer, float direction) // IEnumerator�� ����
    {
        isMoving = true; // �̵� ���� ���·� ����

        // ����� Ŭ�� ����
        audioSource.clip = direction > 0 ? openClip : closeClip;
        if (audioSource.clip != null)
        {
            audioSource.Play(); // ���� ���� �Ǵ� �ݱ� ����� ���
        }

        Vector3 startPosition = drawer.position; // ���� ��ġ
        Vector3 targetPosition = startPosition + new Vector3(0, 0, direction); // ��ǥ ��ġ

        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            drawer.position = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null; // ���� �����ӱ��� ���
        }

        drawer.position = targetPosition; // ���� ��ġ ����
        isMoving = false; // �̵� �Ϸ� ���·� ����

        // ������ ������ currentDrawer�� null�� ����
        if (direction < 0)
        {
            currentDrawer = null; // ������ �������Ƿ� ���� ���� �ʱ�ȭ
        }
        else
        {
            // ������ ������ currentDrawer�� �ش� �������� ����
            currentDrawer = drawer;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && currentDrawer == null) // �����ִ� ������ ���� ���� ����
        {
            // ù ��° �������� ����
            Transform firstDrawer = FindDrawer();
            if (firstDrawer != null && !isMoving)
            {
                StartCoroutine(MoveDrawer(firstDrawer, moveDistance)); // ���� ����
                currentDrawer = firstDrawer; // ���� ���� ����
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        // ������ ���� �ʵ��� ������ ����
    }

    private Transform FindDrawer()
    {
        // Drawer �±׸� ���� �ڽ� ������Ʈ ã��
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Drawer"))
            {
                return child; // ù ��° ������ ����
            }
        }
        return null; // ������ ã�� ���� ��� null ��ȯ
    }
}

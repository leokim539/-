using UnityEngine;
using System.Collections;

public class KeyEvent : MonoBehaviour
{
    public GameObject interactionUI;
    public DoorController doorToUnlock; // Ư�� �� ������Ʈ�� �Ҵ��ϱ� ���� ����
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
        // ���� �� ������ ���� DoorController ��Ȱ��ȭ
        if (doorToUnlock != null)
        {
            doorToUnlock.enabled = false;
            doorToUnlock.requiresUnlocking = true;
            doorToUnlock.firstInteraction = true;
        }
    }

    private void Update()
    {
        if (isKeyCollected) return; // �̹� ������ Ű��� �ٸ� ������ ���� ����
        RaycastHit hit;

        // ����ĳ��Ʈ�� ���� ������ ������Ʈ�� Ȯ���մϴ�.
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactionDistance))
        {
            // ����ĳ��Ʈ�� ������ ������Ʈ�� "Key" �±׸� ������ ���� ��
            if (hit.transform.CompareTag("Key"))
            {
                // �ش� ������Ʈ�� KeyEvent ��ũ��Ʈ�� �����ɴϴ�.
                KeyEvent keyEvent = hit.transform.GetComponent<KeyEvent>();
                if (keyEvent != null && keyEvent.interactionUI != null)
                {
                    // Key �±׸� ���� ������Ʈ�� interactionUI�� Ȱ��ȭ�մϴ�.
                    keyEvent.interactionUI.SetActive(true);

                    // F Ű�� ������ ���� ����
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        keyEvent.CollectKey(); // Ű ���� �޼��� ȣ��
                    }
                }
            }
            else
            {
                // Key �±װ� �ƴ� ������Ʈ�� �ٶ󺸸� UI ��Ȱ��ȭ
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
            // ����ĳ��Ʈ�� ������ ������Ʈ�� ������ ��� UI ��Ȱ��ȭ
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
        isKeyCollected = true; // Ű ���� ���·� ����
        // UI ��Ȱ��ȭ
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
        if (keyPickupSound != null)
        {
            audioSource.PlayOneShot(keyPickupSound);
        }
        // Ư�� ���� Ȱ��ȭ
        if (doorToUnlock != null)
        {
            doorToUnlock.enabled = true;
            doorToUnlock.requiresUnlocking = true;
            doorToUnlock.firstInteraction = true;

            // lockOpenUI ��Ȱ��ȭ
            if (doorToUnlock.GetComponent<LockDoor>().lockOpenUI != null)
            {
                doorToUnlock.GetComponent<LockDoor>().lockOpenUI.SetActive(false);
            }
        }

        // Ű ���� �ִϸ��̼� ����
        StartCoroutine(CollectKeyAnimation());
    }

    private IEnumerator CollectKeyAnimation()
    {
        // �ݶ��̴� ��Ȱ��ȭ
        Collider keyCollider = GetComponent<Collider>();
        if (keyCollider != null)
        {
            keyCollider.enabled = false; // �ݶ��̴� ��Ȱ��ȭ
        }

        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = Vector3.zero; // ���� ũ��
        float duration = 1f; // �̵� �� ��� �ð�
        float elapsedTime = 0f;

        // �÷��̾� ��ġ �������� (���⼭�� ī�޶� ��ġ�� ����)
        Transform playerTransform = Camera.main.transform;

        while (elapsedTime < duration)
        {
            // �÷��̾� �������� �̵�
            Vector3 dir = playerTransform.position - transform.position;
            dir.y = 0; // Y���� 0���� �����Ͽ� ���� �̵��� �ϵ��� ��
            dir.Normalize(); // ���� ���� ����ȭ

            // �������� �÷��̾� ������ �̵�
            transform.position += dir * (3f * Time.deltaTime); // �ӵ� ����

            // ũ�� ���̱�
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / (duration / 3));
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // ������ ��Ȱ��ȭ
        Destroy(gameObject); // �������� ����
    }
}
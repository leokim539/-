using UnityEngine;
using System.Collections;

public class FlashLightPickUp : MonoBehaviour
{
    public GameObject interactionUI; // ��ȣ�ۿ� UI
    public AudioClip flashlightPickupSound; // ������ ���� �Ҹ�
    private bool isFlashlightCollected = false; // ������ ���� ����
    private AudioSource audioSource;
    public float interactionDistance = 3f; // ��ȣ�ۿ� �Ÿ�

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        if (interactionUI != null)
        {
            interactionUI.SetActive(false); // ���� �� UI ��Ȱ��ȭ
        }
    }

    private void Update()
    {
        if (isFlashlightCollected) return; // �̹� ������ �������̶�� �ٸ� ������ ���� ����

        RaycastHit hit;

        // ����ĳ��Ʈ�� ���� ������ ������Ʈ�� Ȯ���մϴ�.
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactionDistance))
        {
            // ����ĳ��Ʈ�� ������ ������Ʈ�� "FlashLight" �±׸� ������ ���� ��
            if (hit.transform.CompareTag("FlashLight"))
            {
                // �ش� ������Ʈ�� FlashLightPickUp ��ũ��Ʈ�� �����ɴϴ�.
                FlashLightPickUp flashlightEvent = hit.transform.GetComponent<FlashLightPickUp>();
                if (flashlightEvent != null && flashlightEvent.interactionUI != null)
                {
                    // FlashLight �±׸� ���� ������Ʈ�� interactionUI�� Ȱ��ȭ�մϴ�.
                    flashlightEvent.interactionUI.SetActive(true);

                    // F Ű�� ������ ���� ����
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        StartCoroutine(flashlightEvent.CollectFlashlight()); // ������ ���� �޼��� ȣ��
                    }
                }
            }
            else
            {
                // FlashLight �±װ� �ƴ� ������Ʈ�� �ٶ󺸸� UI ��Ȱ��ȭ
                FlashLightPickUp[] flashlights = FindObjectsOfType<FlashLightPickUp>();
                foreach (FlashLightPickUp flashlight in flashlights)
                {
                    if (flashlight.interactionUI != null)
                    {
                        flashlight.interactionUI.SetActive(false);
                    }
                }
            }
        }
        else
        {
            // ����ĳ��Ʈ�� ������ ������Ʈ�� ������ ��� UI ��Ȱ��ȭ
            FlashLightPickUp[] flashlights = FindObjectsOfType<FlashLightPickUp>();
            foreach (FlashLightPickUp flashlight in flashlights)
            {
                if (flashlight.interactionUI != null)
                {
                    flashlight.interactionUI.SetActive(false);
                }
            }
        }
    }

    private IEnumerator CollectFlashlight()
    {
        isFlashlightCollected = true; // ������ ���� ���·� ����
        // UI ��Ȱ��ȭ
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
        if (flashlightPickupSound != null)
        {
            audioSource.PlayOneShot(flashlightPickupSound); // ������ ���� �Ҹ� ���
        }

        // ������ ���� �ִϸ��̼� ���� (�ʿ�� �߰�)
        yield return StartCoroutine(CollectFlashlightAnimation()); // �ִϸ��̼� �ڷ�ƾ ȣ��

        // ������ ������Ʈ ����
        Destroy(gameObject); // ������ ������Ʈ ����
    }

    private IEnumerator CollectFlashlightAnimation()
    {
        // ������ ������Ʈ�� ���ŵǴ� �ִϸ��̼��� ������ �� �ֽ��ϴ�.
        // ��: ũ�� ���� �� �̵� �ִϸ��̼� ��

        // �ִϸ��̼� ���� ����
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = Vector3.zero; // ���� ũ��
        float duration = 1f; // �̵� �� ��� �ð�
        float elapsedTime = 0f;

        // �ִϸ��̼� ����
        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}

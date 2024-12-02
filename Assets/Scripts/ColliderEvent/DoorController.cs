using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public GameObject openUI;  // ���� ������ �� ǥ���� UI
    public GameObject closeUI;
    private bool isOpen = false;
    private bool isMoving = false;
    public AudioClip openSound;
    public AudioClip closeSound;
    private AudioSource audioSource;

    [HideInInspector]
    public bool isUnlocking = false;  // ���� ������� ������ ����
    [HideInInspector]
    public bool requiresUnlocking = false;  // ��������� �ʿ��� ������ ����
    [HideInInspector]
    public bool firstInteraction = true;    // ����� ó�� ���� ������ ����

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (!enabled || isUnlocking) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 1.5f))
            {
                DoorController door = hit.transform.GetComponent<DoorController>();
                if (door != null && door.enabled && !door.isMoving)
                {
                    if (door.requiresUnlocking && door.firstInteraction)
                    {
                        LockDoor lockDoor = door.GetComponent<LockDoor>();
                        if (lockDoor != null)
                        {
                            lockDoor.TryUnlock();
                        }
                    }
                    else
                    {
                        StartCoroutine(door.ToggleDoor());
                    }
                }
            }
        }

        // UI ǥ�� ���� ����ĳ��Ʈ
        RaycastHit hitInteraction;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInteraction, 1.5f))
        {
            if (hitInteraction.transform.CompareTag("Door"))
            {
                DoorController door = hitInteraction.transform.GetComponent<DoorController>();
                if (door != null && door.enabled)
                {
                    // �� ���¿� ���� UI ����
                    if (!door.isOpen) // ���� ���� ���� ��
                    {
                        // OpenUI Ȱ��ȭ ���� ����
                        openUI.SetActive(!door.requiresUnlocking || !door.firstInteraction); // ���谡 �ʿ� ���ų� ù ��ȣ�ۿ��� false�� �� Ȱ��ȭ
                        closeUI.SetActive(false); // CloseUI ��Ȱ��ȭ

                        // lockOpenUI Ȱ��ȭ ���� �߰�
                        LockDoor lockDoor = door.GetComponent<LockDoor>();
                        if (lockDoor != null && lockDoor.lockOpenUI != null)
                        {
                            lockDoor.lockOpenUI.SetActive(door.requiresUnlocking && door.firstInteraction); // ��� ������ �ʿ��� ���̰� ù ��ȣ�ۿ��� �� Ȱ��ȭ
                        }
                    }
                    else // ���� ���� ���� ��
                    {
                        openUI.SetActive(false);  // OpenUI ��Ȱ��ȭ
                        closeUI.SetActive(true);   // CloseUI Ȱ��ȭ
                    }
                }
            }
            else
            {
                // ���� �ƴ� ��� CloseUI ��Ȱ��ȭ
                closeUI.SetActive(false);
                openUI.SetActive(false); // OpenUI ��Ȱ��ȭ
            }
        }
        else
        {
            // ����ĳ��Ʈ�� �������� ���� ��� ��� UI ��Ȱ��ȭ
            openUI.SetActive(false);
            closeUI.SetActive(false);

            // lockOpenUI ��Ȱ��ȭ
            LockDoor lockDoor = GetComponent<LockDoor>();
            if (lockDoor != null && lockDoor.lockOpenUI != null)
            {
                lockDoor.lockOpenUI.SetActive(false); // lockOpenUI ��Ȱ��ȭ
            }
        }
    }

    public IEnumerator ToggleDoor()
    {
        if (!enabled || audioSource == null || isUnlocking) yield break;

        isMoving = true;
        float currentAngle = transform.localEulerAngles.y;
        float targetAngle = !isOpen ? currentAngle + 90f : currentAngle - 90f;

        if (audioSource != null)
        {
            AudioClip currentClip = isOpen ? closeSound : openSound;
            if (currentClip != null)
            {
                audioSource.clip = currentClip;
                audioSource.loop = false;
                audioSource.Play();
            }
        }

        while (Mathf.Abs(targetAngle - currentAngle) > 0.1f)
        {
            float step = Time.deltaTime * 150f;
            currentAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, step);
            transform.localEulerAngles = new Vector3(0, currentAngle, 90);
            yield return null;
        }

        transform.localEulerAngles = new Vector3(0, targetAngle, 90);
        if (audioSource != null)
        {
            audioSource.Stop();
        }

        isOpen = !isOpen;
        isMoving = false;
    }
}

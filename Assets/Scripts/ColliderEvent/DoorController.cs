using System.Collections;
using UnityEngine;
using Photon.Pun;

public class DoorController : MonoBehaviourPunCallbacks  // MonoBehaviourPunCallbacks�� ����
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
                        // ���� ���� RPC ȣ��
                        door.photonView.RPC("ToggleDoorRPC", RpcTarget.All);
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

    [PunRPC]  // RPC �޼���� ����
    public void ToggleDoorRPC()
    {
        StartCoroutine(ToggleDoor());
    }

    public IEnumerator ToggleDoor()
    {
        if (!enabled || audioSource == null || isUnlocking) yield break;

        isMoving = true;
        float startAngle = transform.localEulerAngles.z;
        float targetAngle;

        // ���� �����ִٸ� ���� (0 �� 90)
        if (!isOpen)
        {
            startAngle = 0f;
            targetAngle = 90f;
        }
        else  // ���� �����ִٸ� �ݱ� (90 �� 0)
        {
            startAngle = 90f;
            targetAngle = 0f;
        }

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

        float elapsedTime = 0f;
        float totalRotationTime = 1f; // ȸ���� �ɸ��� �� �ð�

        while (elapsedTime < totalRotationTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / totalRotationTime;
            // Smoothstep�� ����Ͽ� �� �ڿ������� ���� ȿ��
            t = t * t * (3f - 2f * t);
            float currentAngle = Mathf.Lerp(startAngle, targetAngle, t);
            transform.localEulerAngles = new Vector3(-90f, 0f, currentAngle);
            yield return null;
        }

        // ���� ���� ��Ȯ�� ����
        transform.localEulerAngles = new Vector3(-90f, 0f, targetAngle);

        if (audioSource != null)
        {
            audioSource.Stop();
        }

        isOpen = !isOpen;
        isMoving = false;
    }
}

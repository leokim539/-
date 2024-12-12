using System.Collections;
using UnityEngine;
using Photon.Pun;

public class DoorController2 : MonoBehaviourPunCallbacks
{
    public GameObject openUI;
    public GameObject closeUI;
    private bool isOpen = false;
    private bool isMoving = false;
    public AudioClip openSound;
    public AudioClip closeSound;
    private AudioSource audioSource;

    [HideInInspector]
    public bool isUnlocking = false;
    [HideInInspector]
    public bool requiresUnlocking = false;
    [HideInInspector]
    public bool firstInteraction = true;

    // 인스펙터에서 설정할 수 있는 닫힌 상태와 열린 상태의 회전값
    public Vector3 closedRotation;  // 닫힌 상태의 로테이션 값
    public Vector3 openRotation;    // 열린 상태의 로테이션 값

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
                DoorController2 door = hit.transform.GetComponent<DoorController2>();
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
                        door.photonView.RPC("ToggleDoorRPC", RpcTarget.All);
                    }
                }
            }
        }

        // UI 활성화 관리
        RaycastHit hitInteraction;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInteraction, 1.5f))
        {
            if (hitInteraction.transform.CompareTag("Door"))
            {
                DoorController2 door = hitInteraction.transform.GetComponent<DoorController2>();
                if (door != null && door.enabled)
                {
                    if (!door.isOpen)
                    {
                        openUI.SetActive(!door.requiresUnlocking || !door.firstInteraction);
                        closeUI.SetActive(false);

                        LockDoor lockDoor = door.GetComponent<LockDoor>();
                        if (lockDoor != null && lockDoor.lockOpenUI != null)
                        {
                            lockDoor.lockOpenUI.SetActive(door.requiresUnlocking && door.firstInteraction);
                        }
                    }
                    else
                    {
                        openUI.SetActive(false);
                        closeUI.SetActive(true);
                    }
                }
            }
            else
            {
                closeUI.SetActive(false);
                openUI.SetActive(false);
            }
        }
        else
        {
            openUI.SetActive(false);
            closeUI.SetActive(false);

            LockDoor lockDoor = GetComponent<LockDoor>();
            if (lockDoor != null && lockDoor.lockOpenUI != null)
            {
                lockDoor.lockOpenUI.SetActive(false);
            }
        }
    }

    [PunRPC]
    public void ToggleDoorRPC()
    {
        StartCoroutine(ToggleDoor());
    }

    public IEnumerator ToggleDoor()
    {
        if (!enabled || audioSource == null || isUnlocking) yield break;

        isMoving = true;

        // 목표 회전 각도 설정
        Vector3 targetRotation = isOpen ? closedRotation : openRotation;

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
        float totalRotationTime = 1f;

        while (elapsedTime < totalRotationTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / totalRotationTime;
            t = t * t * (3f - 2f * t);
            Vector3 currentRotation = Vector3.Lerp(isOpen ? openRotation : closedRotation, targetRotation, t);
            transform.localEulerAngles = currentRotation;
            yield return null;
        }

        transform.localEulerAngles = targetRotation;

        if (audioSource != null)
        {
            audioSource.Stop();
        }

        isOpen = !isOpen;
        isMoving = false;
    }
}

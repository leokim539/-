using System.Collections;
using UnityEngine;
using Photon.Pun;

public class DoorController : MonoBehaviourPunCallbacks  // MonoBehaviourPunCallbacks로 변경
{
    public GameObject openUI;  // 문이 열렸을 때 표시할 UI
    public GameObject closeUI;
    private bool isOpen = false;
    private bool isMoving = false;
    public AudioClip openSound;
    public AudioClip closeSound;
    private AudioSource audioSource;

    [HideInInspector]
    public bool isUnlocking = false;  // 현재 잠금해제 중인지 여부
    [HideInInspector]
    public bool requiresUnlocking = false;  // 잠금해제가 필요한 문인지 여부
    [HideInInspector]
    public bool firstInteraction = true;    // 열쇠로 처음 여는 것인지 여부

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
                        // 문을 여는 RPC 호출
                        door.photonView.RPC("ToggleDoorRPC", RpcTarget.All);
                    }
                }
            }
        }

        // UI 표시 관련 레이캐스트
        RaycastHit hitInteraction;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInteraction, 1.5f))
        {
            if (hitInteraction.transform.CompareTag("Door"))
            {
                DoorController door = hitInteraction.transform.GetComponent<DoorController>();
                if (door != null && door.enabled)
                {
                    // 문 상태에 따라 UI 변경
                    if (!door.isOpen) // 문이 닫혀 있을 때
                    {
                        // OpenUI 활성화 조건 수정
                        openUI.SetActive(!door.requiresUnlocking || !door.firstInteraction); // 열쇠가 필요 없거나 첫 상호작용이 false일 때 활성화
                        closeUI.SetActive(false); // CloseUI 비활성화

                        // lockOpenUI 활성화 조건 추가
                        LockDoor lockDoor = door.GetComponent<LockDoor>();
                        if (lockDoor != null && lockDoor.lockOpenUI != null)
                        {
                            lockDoor.lockOpenUI.SetActive(door.requiresUnlocking && door.firstInteraction); // 잠금 해제가 필요한 문이고 첫 상호작용일 때 활성화
                        }
                    }
                    else // 문이 열려 있을 때
                    {
                        openUI.SetActive(false);  // OpenUI 비활성화
                        closeUI.SetActive(true);   // CloseUI 활성화
                    }
                }
            }
            else
            {
                // 문이 아닐 경우 CloseUI 비활성화
                closeUI.SetActive(false);
                openUI.SetActive(false); // OpenUI 비활성화
            }
        }
        else
        {
            // 레이캐스트가 감지되지 않을 경우 모든 UI 비활성화
            openUI.SetActive(false);
            closeUI.SetActive(false);

            // lockOpenUI 비활성화
            LockDoor lockDoor = GetComponent<LockDoor>();
            if (lockDoor != null && lockDoor.lockOpenUI != null)
            {
                lockDoor.lockOpenUI.SetActive(false); // lockOpenUI 비활성화
            }
        }
    }

    [PunRPC]  // RPC 메서드로 설정
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

        // 문이 닫혀있다면 열기 (0 → 90)
        if (!isOpen)
        {
            startAngle = 0f;
            targetAngle = 90f;
        }
        else  // 문이 열려있다면 닫기 (90 → 0)
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
        float totalRotationTime = 1f; // 회전에 걸리는 총 시간

        while (elapsedTime < totalRotationTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / totalRotationTime;
            // Smoothstep을 사용하여 더 자연스러운 보간 효과
            t = t * t * (3f - 2f * t);
            float currentAngle = Mathf.Lerp(startAngle, targetAngle, t);
            transform.localEulerAngles = new Vector3(-90f, 0f, currentAngle);
            yield return null;
        }

        // 최종 각도 명확히 설정
        transform.localEulerAngles = new Vector3(-90f, 0f, targetAngle);

        if (audioSource != null)
        {
            audioSource.Stop();
        }

        isOpen = !isOpen;
        isMoving = false;
    }
}

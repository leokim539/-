using System.Collections;
using UnityEngine;

public class LockDoor : MonoBehaviour
{
    public GameObject NeedKeyUI;
    public GameObject lockOpenUI;
    public AudioClip lockSound;
    public AudioClip unlockSound;
    public float interactionDistance = 3f;
    public float soundDistance = 1.5f;
    public float unlockDuration = 1.5f;
    private AudioSource audioSource;
    private float lastLockSoundTime = 0f;
    private const float LOCK_SOUND_COOLDOWN = 0.5f;
    private DoorController doorController;
    private bool isUnlocking = false;


    public bool IsUnlocking { get { return isUnlocking; } }

    public GameObject keyObject;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        doorController = GetComponent<DoorController>();
        if (doorController != null)
        {
            doorController.requiresUnlocking = true;
            doorController.firstInteraction = true;
        }
        if (NeedKeyUI != null)
        {
            NeedKeyUI.SetActive(false);
        }
        if (lockOpenUI != null)
        {
            lockOpenUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (doorController == null || (!doorController.enabled))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactionDistance))
            {
                LockDoor targetLockDoor = hit.transform.GetComponent<LockDoor>();
                if (targetLockDoor != null)
                {
                    if (targetLockDoor.keyObject == null || !targetLockDoor.keyObject.activeInHierarchy)
                    {
                        if (NeedKeyUI != null)
                        {
                            NeedKeyUI.SetActive(false);
                        }
                        if (lockOpenUI != null)
                        {
                            lockOpenUI.SetActive(!doorController.firstInteraction);
                        }
                    }
                    else
                    {
                        if (NeedKeyUI != null)
                        {
                            NeedKeyUI.SetActive(true);
                        }
                    }


                    if (Input.GetKeyDown(KeyCode.E) && hit.distance <= soundDistance)
                    {
                        if (targetLockDoor.keyObject == null || !targetLockDoor.keyObject.activeInHierarchy)
                        {
                            TryUnlock();
                        }
                        else
                        {
                            PlayLockSound();
                        }
                    }
                }
                else
                {
                    if (NeedKeyUI != null)
                    {
                        NeedKeyUI.SetActive(false);
                    }
                }
            }
            else
            {
                if (NeedKeyUI != null)
                {
                    NeedKeyUI.SetActive(false);
                }
                if (lockOpenUI != null)
                {
                    lockOpenUI.SetActive(false);
                }
            }
        }
    }

    private void PlayLockSound()
    {
        if (Time.time - lastLockSoundTime >= LOCK_SOUND_COOLDOWN)
        {
            if (lockSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(lockSound);
                lastLockSoundTime = Time.time;
            }
        }
    }

    public void TryUnlock()
    {
        if (doorController != null && doorController.enabled && doorController.firstInteraction)
        {
            // 잠금 해제 UI 활성화
            if (lockOpenUI != null)
            {
                lockOpenUI.SetActive(true);
            }
            StartCoroutine(UnlockDoor());
        }
    }

    private IEnumerator UnlockDoor()
    {
        if (doorController == null || isUnlocking) yield break;

        isUnlocking = true;
        doorController.isUnlocking = true;

        if (unlockSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(unlockSound);
            yield return new WaitForSeconds(unlockDuration);
        }

        // 잠금 해제 후 문 열기
        doorController.firstInteraction = false;

        // lockOpenUI 비활성화
        if (lockOpenUI != null)
        {
            lockOpenUI.SetActive(false);
        }

        isUnlocking = false;
        doorController.isUnlocking = false;
    }


    private void OnDisable()
    {
        if (NeedKeyUI != null)
        {
            NeedKeyUI.SetActive(false);
        }
        if (lockOpenUI != null)
        {
            lockOpenUI.SetActive(false);
        }
    }
}
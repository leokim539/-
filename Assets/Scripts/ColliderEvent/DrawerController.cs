using System.Collections;
using UnityEngine;
using Photon.Pun;

public class DrawerController : MonoBehaviourPunCallbacks
{
    public GameObject openUI;  // 서랍이 열렸을 때 표시할 UI
    public GameObject closeUI;  // 서랍이 닫혔을 때 표시할 UI
    private bool isOpen = false;
    private bool isMoving = false;

    public AudioClip openSound;  // 서랍 열 때 소리
    public AudioClip closeSound;  // 서랍 닫을 때 소리
    private AudioSource audioSource;

    [SerializeField]
    private Vector3 openPosition;    // 서랍이 열렸을 때 위치
    [SerializeField]
    private Vector3 closedPosition;  // 서랍이 닫혔을 때 위치
    [SerializeField]
    private float moveDuration = 0.5f;   // 서랍이 열리는 시간

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // UI 초기 상태 설정
        openUI.SetActive(false);
        closeUI.SetActive(false);
    }

    private void Update()
    {
        // E 키 입력 체크
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 2f))
            {
                DrawerController drawer = hit.transform.GetComponent<DrawerController>();
                if (drawer != null && drawer.enabled && !drawer.isMoving)
                {
                    // RPC 호출로 드로어를 여는 작업
                    drawer.photonView.RPC("ToggleDrawerRPC", RpcTarget.All);
                }
            }
        }

        // UI 표시 관련 레이캐스트
        RaycastHit hitInteraction;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInteraction, 2f))
        {
            if (hitInteraction.transform.CompareTag("Drawer"))
            {
                DrawerController drawer = hitInteraction.transform.GetComponent<DrawerController>();
                if (drawer != null && drawer.enabled)
                {
                    // 서랍 상태에 따라 UI 변경
                    if (!drawer.isOpen) // 서랍이 닫혀 있을 때
                    {
                        openUI.SetActive(true);  // OpenUI 활성화
                        closeUI.SetActive(false); // CloseUI 비활성화
                    }
                    else // 서랍이 열려 있을 때
                    {
                        openUI.SetActive(false);  // OpenUI 비활성화
                        closeUI.SetActive(true);   // CloseUI 활성화
                    }
                }
            }
            else
            {
                // 서랍이 아닐 경우 UI 비활성화
                closeUI.SetActive(false);
                openUI.SetActive(false);
            }
        }
        else
        {
            // 레이캐스트가 감지되지 않을 경우 모든 UI 비활성화
            openUI.SetActive(false);
            closeUI.SetActive(false);
        }
    }

    [PunRPC] // RPC 메서드로 설정
    public void ToggleDrawerRPC()
    {
        StartCoroutine(ToggleDrawer());
    }

    public IEnumerator ToggleDrawer()
    {
        if (!enabled || audioSource == null) yield break;

        isMoving = true;
        Vector3 targetPosition = isOpen ? closedPosition : openPosition; // 열림/닫힘 위치 결정

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

        float elapsedTime = 0;
        Vector3 initialPosition = transform.localPosition;

        while (elapsedTime < moveDuration)
        {
            transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = targetPosition; // 최종 위치 보정

        if (audioSource != null)
        {
            audioSource.Stop();
        }

        isOpen = !isOpen; // 상태 업데이트
        isMoving = false;
    }
}

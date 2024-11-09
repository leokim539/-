using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public GameObject interactionUI; // 상호작용 UI

    private bool isOpen = false; // 문이 열려있는지 여부
    private bool isMoving = false; // 문이 이동 중인지 여부

    // 오디오 클립 변수
    public AudioClip openSound; // 문이 열릴 때 재생할 소리
    public AudioClip closeSound; // 문이 닫힐 때 재생할 소리
    private AudioSource audioSource; // 오디오 소스

    private void Start()
    {
        // 초기 상태에서 X축을 -90도로 설정
      //  transform.localEulerAngles = new Vector3(-90, transform.localEulerAngles.y, 0);

        // AudioSource 컴포넌트 가져오기
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        // E 키 입력 체크
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;

            // 플레이어 위치에서 정면으로 레이캐스트
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 1.5f))
            {
                // 레이캐스트로 감지한 오브젝트의 DoorController 컴포넌트 확인
                DoorController door = hit.transform.GetComponent<DoorController>();
                if (door != null && !door.isMoving) // DoorController가 존재하고 이동 중이 아닐 때
                {
                    StartCoroutine(door.ToggleDoor()); // 해당 문만 열기
                }
            }
        }

        // 레이캐스트를 통해 UI 활성화/비활성화
        RaycastHit hitInteraction;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInteraction, 3f))
        {
            if (hitInteraction.transform.CompareTag("Door"))
            {
                interactionUI.SetActive(true); // UI 활성화
            }
            else
            {
                interactionUI.SetActive(false); // UI 비활성화
            }
        }
        else
        {
            interactionUI.SetActive(false); // UI 비활성화
        }
    }

    public IEnumerator ToggleDoor() // public으로 변경하여 외부에서 호출 가능하도록
    {
        isMoving = true; // 이동 중 상태 설정

        // 문이 열릴 때와 닫힐 때의 현재 각도를 저장할 변수
        float currentAngle = transform.localEulerAngles.y;
        float targetAngle;

        // 문이 닫혀 있는 상태일 때
        if (!isOpen)
        {
            targetAngle = currentAngle + 90f; // 현재 각도에서 90도 더하기
        }
        else // 문이 열려 있는 상태일 때
        {
            targetAngle = currentAngle - 90f; // 현재 각도에서 90도 빼기
        }

        // 오디오 클립 설정
        AudioClip currentClip = isOpen ? closeSound : openSound;
        audioSource.clip = currentClip;
        audioSource.loop = false; // 루프 설정 해제
        audioSource.Play(); // 소리 재생 시작

        // 문을 회전시키는 애니메이션
        while (Mathf.Abs(targetAngle - currentAngle) > 0.1f)
        {
            float step = Time.deltaTime * 150f; // 회전 속도
            currentAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, step); // 현재 각도를 목표 각도로 이동
            transform.localEulerAngles = new Vector3(0, currentAngle, 90); // X축은 0, Y축만 회전
            yield return null; // 다음 프레임까지 대기
        }

        // 최종 각도로 설정
        transform.localEulerAngles = new Vector3(0, targetAngle, 90); // X축은 0, Y축만 설정

        // 소리 재생 중지
        audioSource.Stop();

        isOpen = !isOpen; // 문 상태 반전
        isMoving = false; // 이동 완료 상태로 변경
    }

}

using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class FirstPersonController : MonoBehaviourPunCallbacks, IPunObservable
{
    private Animator animator; // 애니메이터
    private AudioSource audioSource; // 오디오 소스
    private Rigidbody rd;
    private float originalMoveSpeed;  // 원래 이동 속도 저장
    private bool isSettingsOpen = false;

    public float moveSpeed = 5f;  // 캐릭터 이동 속도
    public float sensitivity = 2;

    private float xRotation = 0f; 
    public Transform character;

    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    [Header("오디오")]
    public AudioClip walkSound; // 걷기 소리 

    public Camera mainCamera; // MainCamera 오브젝트
    public Camera crouchCamera; // MainCamera2 오브젝트

    [Header("UI 버튼")]
    public GameObject panel; // 패널을 드래그하여 연결합니다.
    public KeyCode keyToPress; // 상태창을 활성화하는 키
    public GameObject settingsPanel; // 설정 창 패널을 연결

    void Awake()
    {
        character = GetComponent<FirstPersonController>().transform;
        animator = transform.Find("Idel").GetComponent<Animator>(); // 애니메이터 가져옴
    }

    void Start()
    {
        Debug.Log("IsMine: " + photonView.IsMine);
        settingsPanel = GameObject.Find("ESC");
        panel = GameObject.Find("Tap");
        settingsPanel.SetActive(false);
        if (photonView.IsMine)
        {
            Debug.Log("이 플레이어는 나의 것입니다.");
            audioSource = GetComponent<AudioSource>(); // AudioSource 컴포넌트 가져오기
            rd = GetComponent<Rigidbody>();
            rd.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            originalMoveSpeed = moveSpeed; // 속도 저장

            // 카메라를 초기 상태로 설정
            if (mainCamera != null && crouchCamera != null)
            {
                mainCamera.enabled = true; // MainCamera 활성화
                crouchCamera.enabled = false; // MainCamera2 비활성화
            }
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            mainCamera.gameObject.SetActive(true);
        }
        else
        {
            mainCamera.gameObject.SetActive(false);

        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleSettingsPanel();
            }
            if(!isSettingsOpen)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                MovePlayer(); // 이동 처리
                CameraRotation(); // 카메라 처리
            }
            if (Input.GetKey(keyToPress))
            {
                panel.SetActive(true);
            }
            else
            {
                panel.SetActive(false);
            }
        }
    }
    private void ToggleSettingsPanel()
    {
        isSettingsOpen = !isSettingsOpen;
        settingsPanel.SetActive(isSettingsOpen);

        if (isSettingsOpen) // 설정 창이 열렸을 때
        {
            Cursor.visible = true; // 마우스 커서 표시
            Cursor.lockState = CursorLockMode.None; // 마우스 이동 가능
            audioSource.Stop();
        }

    }
   // void MovePlayer()
   // {
     //   float x = Input.GetAxis("Horizontal");
      //  float z = Input.GetAxis("Vertical");
       // Vector3 move = transform.right * x + transform.forward * z;
       // transform.position += move * moveSpeed * Time.deltaTime;

       // if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) // 걷기 소리 재생
       // {
       //     PlaySound(walkSound);
      //  }
      //  else
       // {
       //     audioSource.Stop(); // 소리 정지
      //  }



      //  UpdateAnimator(move);
  //  }

    void MovePlayer()// 변경된 메서드(애니메이션 속도 관련)
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // 이동 벡터 계산
        Vector2 targetVelocity = new Vector2(x * moveSpeed, z * moveSpeed);

        // 캐릭터 이동
        rd.velocity = transform.rotation * new Vector3(targetVelocity.x, rd.velocity.y, targetVelocity.y); 

        // 걷기 소리 재생
        if (x != 0 || z != 0) // 이동 중일 때
        {
            PlaySound(walkSound);
        }
        else
        {
            audioSource.Stop(); // 소리 정지
        }

        UpdateAnimator(targetVelocity); // 애니메이션 상태 업데이트
    }


    private void UpdateAnimator(Vector2 targetVelocity)
    {
        // 이동 벡터의 크기를 계산하여 Forward와 Strafe 파라미터에 설정
        float forward = targetVelocity.y / moveSpeed; // moveSpeed를 기준으로 전방 비율 계산
        float strafe = targetVelocity.x / moveSpeed;  // moveSpeed를 기준으로 측면 비율 계산

        animator.SetFloat("Forward", forward); // Forward 파라미터 설정
        animator.SetFloat("Strafe", strafe);   // Strafe 파라미터 설정

        // 걷기 애니메이션 상태 처리
        // isWalk 파라미터 대신 Forward와 Strafe 값을 사용하여 걷기 상태를 판단
        if (Mathf.Abs(forward) > 0.01f || Mathf.Abs(strafe) > 0.01f) // 이동 중이면
        {
            animator.SetFloat("isWalking", 1f); // 걷기 상태 설정 (1)
        }
        else
        {
            animator.SetFloat("isWalking", 0f); // 걷기 상태 해제 (0)
        }
    }
    void CameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity*2 * Time.deltaTime * 100; // 마우스 X 이동
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity*2 * Time.deltaTime * 100; // 마우스 Y 이동

        xRotation -= mouseY; // 카메라 상하 회전값 조정
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // 상하 회전 제한

        mainCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // 카메라 상하 회전
        character.Rotate(Vector3.up * mouseX); // 캐릭터 좌우 회전

    }
    public void PickingUp()
    {
        if (mainCamera != null && crouchCamera != null)
        {
            mainCamera.enabled = false; // MainCamera 비활성화
            crouchCamera.enabled = true; // MainCamera2 활성화
        }

        // 딜레이를 두기 위해 코루틴을 호출
        StartCoroutine(DelayCrouch());
    }

    private IEnumerator DelayCrouch()
    {
        yield return new WaitForSeconds(0.5f); // 0.5초 딜레이
        animator.SetBool("isCrouched", false); // 쪼그려 앉는 애니메이션 상태를 해제

        yield return new WaitForSeconds(1f);
        // 카메라를 원래 상태로 되돌림
        if (mainCamera != null && crouchCamera != null)
        {
            mainCamera.enabled = true; // MainCamera 활성화
            crouchCamera.enabled = false; // MainCamera2 비활성화
        }
    }
    

    private void PlaySound(AudioClip clip)
    {
        if (audioSource.clip != clip || !audioSource.isPlaying) // 현재 재생 중인 소리와 다를 경우
        {
            audioSource.clip = clip;
            audioSource.loop = true; // 소리 반복 재생
            audioSource.Play();
        }
    }

    public void SlowDown(float speedMultiplier)
    {
        moveSpeed *= speedMultiplier; // 이동 속도를 줄이는 배율 (절반 속도)

        moveSpeed = Mathf.Max(moveSpeed, originalMoveSpeed * 0.5f); // 원래 속도의 50% 이하로 떨어지지 않도록
    }

    public void RestoreSpeed()
    {
        moveSpeed = originalMoveSpeed;
    }

    // 추가한 메서드
    public void StopMovement()
    {
        if (!photonView.IsMine) return; // 로컬 플레이어만 처리

        if (audioSource != null)
        {
            audioSource.Stop(); // 소리 정지
        }

        if (animator != null)
        {
            animator.SetBool("isWalk", false); // 걷기 애니메이션 중단
            animator.SetBool("isCrouched", false); // 쪼그리기 애니메이션 중단
        }

        if (rd != null)
        {
            rd.velocity = Vector3.zero; // 캐릭터 속도 초기화
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 내 위치와 회전 정보를 전송
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // 다른 플레이어의 위치와 회전 정보를 수신
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
        }
    }
    // 추가한 메서드
    /*public void ResumeMovement()
    {
        // 특별한 동작 필요 없음 (Update에서 처리)
    }

    /* 주석 코드: 대체 MovePlayer 로직 (이전 방식)
        void MovePlayer()
        {
            float moveX = Input.GetAxis("Horizontal");  // 좌우 이동 입력
            float moveZ = Input.GetAxis("Vertical");    // 앞뒤 이동 입력

            // 스프린트 여부 체크
            bool isSprinting = Input.GetKey(KeyCode.LeftShift) && stamina > 0;
            float currentSpeed = isSprinting ? runSpeed : moveSpeed;

            // 이동 벡터 계산
            Vector3 move = transform.right * moveX + transform.forward * moveZ;

            // 캐릭터 이동
            characterController.Move(move * currentSpeed * Time.deltaTime);

            // 스프린트 시 스태미나 소모
            if (isSprinting)
            {
                stamina -= staminaDrainRate * Time.deltaTime;
                stamina = Mathf.Clamp(stamina, 0, maxStamina);
            }
        }
    */
}
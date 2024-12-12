using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;

public class FirstPersonController : MonoBehaviourPunCallbacks, IPunObservable
{
    private float forward; // Forward 애니메이션 상태
    private float strafe;  // Strafe 애니메이션 상태
    private float isWalking; // 걷기 상태 (0 또는 1)
    private bool isRunning = false; // 뛰는 애니메이션 상태

    private Vector3 networkPosition; // 네트워크에서 수신한 위치
    private Quaternion networkRotation; // 네트워크에서 수신한 회전
    private float lerpRate = 10f; // 보간 속도

    // 애니메이션 상태 변수
    private bool networkIsRunning; // 네트워크에서 수신한 달리기 상태
    private float networkForward; // 네트워크에서 수신한 Forward 애니메이션 값
    private float networkStrafe; // 네트워크에서 수신한 Strafe 애니메이션 값
    private float networkIsWalking; // 네트워크에서 수신한 걷기 상태

    public Trash2 trash2;
    private Animator animator; // 애니메이터
    private AudioSource audioSource; // 오디오 소스
    private Rigidbody rd;
    private float originalMoveSpeed;  // 원래 이동 속도 저장
    private float originalRunSpeed;   // 원래 달리기 속도 저장
    private bool isSettingsOpen = false;

    private bool isExhausted = false; // 스태미나 소진 상태 확인

    public float moveSpeed = 5f;  // 캐릭터 이동 속도
    public float sensitivity = 2;
    public float runSpeed = 7f; // 달리기 속도
    public KeyCode runningKey = KeyCode.LeftShift; //달리기 키
    public float maxStamina = 5f;  // 최대 스태미나
    public float stopStamina = 2.5f; // 멈췄다가 다시 움직일 수 있는 스태미나 임계값
    public float stamina = 5f;    // 현재 스태미나
    public float staminaDrainRate = 1f;  // 스태미나 소모율
    public float staminaRecoveryRate = 1f; // 스태미나 회복율
    public Slider staminaSlider; // 스태미나 슬라이더

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
    public PhotonView m_PhotonView;

    public bool canMove; 
    public bool bond = true;
    public bool coffee = true;
    [Header("숟가락")]
    public bool spoon = true;
    void Awake()
    {
        ItemManager.instane.firstPersonController = this;
        m_PhotonView = GetComponent<PhotonView>();
        character = GetComponent<FirstPersonController>().transform;
        animator = transform.Find("Idel").GetComponent<Animator>(); // 애니메이터 가져옴
    }
    void Start()
    {
        // 스태미나를 최대값으로 초기화
        stamina = maxStamina;

        if (staminaSlider == null)
        {
            GameObject sliderObject = GameObject.Find("StaminaSlider");
            if (sliderObject != null)
            {
                staminaSlider = sliderObject.GetComponent<Slider>();
            }
        }


        // 슬라이더 설정
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina; // 슬라이더 최대값 동기화
            staminaSlider.value = stamina;       // 슬라이더 초기값 동기화
        }

        Debug.Log("IsMine: " + photonView.IsMine);
        settingsPanel = GameObject.Find("ESC");
        panel = GameObject.Find("Tap");

        if (photonView.IsMine)
        {
            settingsPanel.SetActive(false);
            Debug.Log("이 플레이어는 나의 것입니다.");
            audioSource = GetComponent<AudioSource>(); // AudioSource 컴포넌트 가져오기
            rd = GetComponent<Rigidbody>();
            rd.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            originalMoveSpeed = moveSpeed; // 속도 저장
            originalRunSpeed = runSpeed;

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
        Debug.Log(canMove);
        if (photonView.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleSettingsPanel();
            }
            if (!isSettingsOpen)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                CameraRotation(); // 카메라 처리
                if (canMove)
                {
                    MovePlayer(); // 이동 처리
                }
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
        else
        {
            InterpolateMovement();
            UpdateAnimatorFromNetwork();

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

            if (animator != null)
            {
                animator.SetFloat("Forward", 0f);
                animator.SetFloat("Strafe", 0f);
                animator.SetFloat("isWalking", 0f);
                animator.SetBool("isCrouched", false);

            }

            if (rd != null)
            {
                rd.velocity = Vector3.zero;
            }
        }

    }
    void MovePlayer()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (isExhausted) // 스태미나 소진 상태일 경우
        {
            RecoverStamina(); // 스태미나 회복
            UpdateStaminaBar(); // UI 업데이트

            if (stamina >= stopStamina) // stopStamina 이상 회복되면 소진 상태 해제
            {
                isExhausted = false;
            }
            else
            {
                StopMovement(); // 계속 정지 상태 유지
                isRunning = false; // 소진 상태에서 뛰기 상태 해제
                return;
            }
        }

        // 항상 스태미나 회복
        if (!isExhausted && stamina < maxStamina)
        {
            RecoverStamina();
            UpdateStaminaBar();
        }

        // 이동 여부 및 스프린트 여부 확인
        bool isMoving = Mathf.Abs(x) > 0 || Mathf.Abs(z) > 0; // 이동 여부
        if (bond)
            isRunning = isMoving && Input.GetKey(runningKey) && stamina > 0; // 스프린트 여부 설정

        // 스프린트 시 속도 및 스태미나 소모 처리
        float targetMovingSpeed = isRunning ? runSpeed : moveSpeed;

        if (isRunning) // 스프린트 시 스태미나 소모
        {
            if (coffee)
            {
                stamina -= staminaDrainRate * Time.deltaTime; // 초당 소모
                stamina = Mathf.Clamp(stamina, 0, maxStamina); // 스태미나 범위 제한

                UpdateStaminaBar(); // 슬라이더 업데이트

                if (stamina <= 0) // 스태미나가 소진되면
                {
                    isExhausted = true; // 소진 상태 활성화
                    isRunning = false; // 스프린트 중지
                    Debug.Log("Stamina is exhausted, stopping player. isRunning: " + isRunning);
                    StopMovement(); // 캐릭터 멈춤
                    return;
                }
            }
            
        }


        // 이동 처리
        Vector2 targetVelocity = new Vector2(x * targetMovingSpeed, z * targetMovingSpeed);
        rd.velocity = transform.rotation * new Vector3(targetVelocity.x, rd.velocity.y, targetVelocity.y);

        // 걷기/달리기 사운드 재생
        if (isMoving)
        {
            PlaySound(walkSound);
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop(); // 사운드 중단
            }
        }

        UpdateAnimator(targetVelocity); // 애니메이션 상태 업데이트
    }
    public void StopMovement()
    {
        // Photon 네트워크 처리 (필요한 경우)
        if (photonView != null && !photonView.IsMine) return;

        // 오디오 정지
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        // 애니메이터 처리 (두 스크립트의 방식 모두 포함)
        if (animator != null)
        {
            // Photon 스크립트 스타일
            animator.SetBool("isWalk", false);
            animator.SetBool("isCrouched", false);
            animator.SetBool("isRunning", false);

            // 기존 스크립트 스타일 추가
            animator.SetFloat("Forward", 0f);
            animator.SetFloat("Strafe", 0f);
            animator.SetFloat("isWalking", 0f);
        }

        // 속도 초기화
        if (rd != null)
        {
            rd.velocity = Vector3.zero;
        }

        // 선택적: isRunning 상태 설정
        isRunning = false;

        // 선택적: 디버그 로그
        Debug.Log("StopMovement called. isRunning set to: " + isRunning);
    }
    void RecoverStamina()
    {
        // 쉬프트를 누르지 않을 때 또는 이동 중이 아닐 때 스태미나 회복
        if (!Input.GetKey(runningKey) && stamina < maxStamina)
        {
            stamina += staminaRecoveryRate * Time.deltaTime; // 초당 1씩 회복
            stamina = Mathf.Clamp(stamina, 0, maxStamina); // 스태미나 범위 제한
        }
    }
    void UpdateStaminaBar()
    {
        if (staminaSlider != null)
        {
            staminaSlider.value = stamina; // 스태미나 값을 슬라이더에 반영
        }
    }
    private void UpdateAnimator(Vector2 targetVelocity)
    {
        // 이동 벡터의 크기를 계산하여 Forward와 Strafe 파라미터에 설정
        forward = targetVelocity.y / moveSpeed; // moveSpeed를 기준으로 전방 비율 계산
        strafe = targetVelocity.x / moveSpeed;  // moveSpeed를 기준으로 측면 비율 계산

        animator.SetFloat("Forward", forward); // Forward 파라미터 설정
        animator.SetFloat("Strafe", strafe);   // Strafe 파라미터 설정

        // 걷기 애니메이션 상태 처리
        if (Mathf.Abs(forward) > 0.01f || Mathf.Abs(strafe) > 0.01f) // 이동 중이면
        {
            isWalking = 1f; // 걷기 상태 설정 (1)
        }
        else
        {
            isWalking = 0f; // 걷기 상태 해제 (0)
        }


        animator.SetFloat("isWalking", isWalking); // 애니메이터에 설정

        // 스프린트 상태 애니메이션 설정
        animator.SetBool("isRunning", isRunning); // isRunning 파라미터 설정
    }

    private void InterpolateMovement()
    {
        // 네트워크 위치와 회전으로 보간
        transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * lerpRate);
        transform.rotation = Quaternion.Slerp(transform.rotation, networkRotation, Time.deltaTime * lerpRate);
    }

    private void UpdateAnimatorFromNetwork()
    {
        // 네트워크에서 수신한 애니메이션 상태 업데이트
        animator.SetFloat("Forward", networkForward);
        animator.SetFloat("Strafe", networkStrafe);
        animator.SetFloat("isWalking", networkIsWalking);
        animator.SetBool("isRunning", networkIsRunning);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 내 위치와 회전 정보를 전송
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(forward);
            stream.SendNext(strafe);
            stream.SendNext(isWalking);
            stream.SendNext(isRunning); // 달리기 상태도 전송
        }
        else
        {
            // 다른 플레이어의 위치와 회전 정보를 수신
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            networkForward = (float)stream.ReceiveNext(); // Forward 수신
            networkStrafe = (float)stream.ReceiveNext();  // Strafe 수신
            networkIsWalking = (float)stream.ReceiveNext(); // isWalking 수신
            networkIsRunning = (bool)stream.ReceiveNext(); // isRunning 수신

            // 애니메이터 업데이트
            UpdateAnimatorFromNetwork();
        }
    }
    void CameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * 2 * Time.deltaTime * 100; // 마우스 X 이동
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * 2 * Time.deltaTime * 100; // 마우스 Y 이동

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
        runSpeed *= speedMultiplier;

        moveSpeed = Mathf.Max(moveSpeed, originalMoveSpeed * 0.5f); // 원래 속도의 50% 이하로 떨어지지 않도록
        runSpeed = Mathf.Max(runSpeed, originalRunSpeed * 0.5f);
    }

    public void RestoreSpeed()
    {
        moveSpeed = originalMoveSpeed;
        runSpeed = originalRunSpeed;
    }
    [PunRPC]
    public void TeleportTo(Vector3 targetPosition)
    {
        transform.position = targetPosition;
        Debug.Log("Player teleported to: " + targetPosition);
    }
    [PunRPC]
    public void TaserGuns()
    {
        StartCoroutine(TaserGun()); // 타겟을 5초간 경직
    }
    public IEnumerator TaserGun()
    {
        canMove = false; // 상대방의 이동 비활성화
        yield return new WaitForSeconds(5f); // 5초 대기
        canMove = true; // 상대방의 이동 활성화
    }
    [PunRPC]
    public void ApplyObscuringEffects()
    {
        StartCoroutine(ApplyObscuringEffect()); // 상대 시야 5초간 안보임
    }

    
    public IEnumerator ApplyObscuringEffect()
    {
        GameObject canvasInstance = PhotonNetwork.Instantiate(trash2.canvasPrefab.name, transform.position, Quaternion.identity, 0);
        canvasInstance.transform.SetParent(transform); // 상대방 오브젝트의 자식으로 설정

        RectTransform rectTransform = canvasInstance.GetComponent<RectTransform>();
        rectTransform.localPosition = Vector3.zero;
        rectTransform.localScale = new Vector3(1, 1, 1);

        yield return new WaitForSeconds(5);

        PhotonNetwork.Destroy(canvasInstance);
    }
    [PunRPC]
    public void Hackings()
    {
        StartCoroutine(Hacking()); // 적 5초간 해킹
    }
    public IEnumerator Hacking()
    {
        trash2.hacking = false;
        yield return new WaitForSeconds(5f);
        trash2.hacking = true;               
    }
    [PunRPC]
    public void SmartPhone()
    {
         trash2.smartPhone = false;
    }
    [PunRPC]
    public void HandCreams()
    {
        StartCoroutine(HandCream()); // 적 5초간 해킹
    }
    public IEnumerator HandCream()
    {
        trash2.handCream = false;
        yield return new WaitForSeconds(10);
        trash2.handCream = true;

    }
    [PunRPC]
    public void Bonds()
    {
        StartCoroutine(Bond()); // 적 5초간 해킹
    }
    public IEnumerator Bond()
    {
        bond = false;
        yield return new WaitForSeconds(5);
        bond = true;
    }
    [PunRPC]
    public void Spoons()
    {
        StartCoroutine(Spoon()); // 적 5초간 해킹
    }
    public IEnumerator Spoon()
    {
        spoon = false;
        yield return new WaitForSeconds(10);
        spoon = true;
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
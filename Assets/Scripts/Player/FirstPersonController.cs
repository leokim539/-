using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstPersonController : MonoBehaviour
{
    public float moveSpeed = 5f;  // 캐릭터 이동 속도
    public float runSpeed = 7f; // 달리기 속도
    public KeyCode runningKey = KeyCode.LeftShift; //달리기 키
    public float maxStamina = 5f;  // 최대 스태미나
    public float stamina = 5f;    // 현재 스태미나
    public float staminaDrainRate = 1f;  // 스태미나 소모율
    public float staminaRecoveryRate = 1f; // 스태미나 회복율
    public Slider staminaSlider; // 스태미나 슬라이더

    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    private Rigidbody rigidbody;
    private AudioSource audioSource; // 오디오 소스

    private float originalMoveSpeed;  // 원래 이동 속도 저장
    private float originalRunSpeed;   // 원래 달리기 속도 저장
    private bool isWalking; // 걷고 있는지 여부

    public AudioClip walkSound; // 걷기 소리
    public AudioClip runSound; // 달리기 소리
    //public float mouseSensitivity = 100f;  // 마우스 감도
    //private float xRotation = 0f;  // 상하 카메라 회전을 위한 변수
    //private CharacterController characterController;
    void Awake()
    {
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // AudioSource 컴포넌트 가져오기
        rigidbody = GetComponent<Rigidbody>();

        originalMoveSpeed = moveSpeed;//속도 저장
        originalRunSpeed = runSpeed;

        // Character Controller 컴포넌트 가져오기
        //characterController = GetComponent<CharacterController>();
        //Cursor.lockState = CursorLockMode.Locked;// 마우스 커서 잠금
    }
    void Update()
    {
    }
    void FixedUpdate()
    {
        MovePlayer();// 이동 처리

        RecoverStamina();// 스태미나 회복

        UpdateStaminaBar();// UI 업데이트
    }
    void MovePlayer()
    {
        bool isSprinting = Input.GetKey(runningKey) && stamina > 0;// 스프린트 여부 체크
        float targetMovingSpeed = isSprinting ? runSpeed : moveSpeed;
        
        if (isSprinting)// 스프린트 시 스태미나 소모
        {
            stamina -= staminaDrainRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
            PlaySound(runSound); // 달리기 소리 재생
        }
        else if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) // 걷기 소리 재생
        {
            PlaySound(walkSound);
        }
        else
        {
            audioSource.Stop(); // 소리 정지
        }

        /*if (speedOverrides.Count > 0)
        {
            targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
        }  */    
        Vector2 targetVelocity = new Vector2(Input.GetAxis("Horizontal") * targetMovingSpeed, Input.GetAxis("Vertical") * targetMovingSpeed);// 이동 벡터 계산       
        rigidbody.velocity = transform.rotation * new Vector3(targetVelocity.x, rigidbody.velocity.y, targetVelocity.y);// 캐릭터 이동
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
    void RecoverStamina()// 스태미나 회복 함수
    {
        if (stamina < maxStamina && !Input.GetKey(runningKey))
        {
            stamina += staminaRecoveryRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
        }
    }
    void UpdateStaminaBar()// 슬라이더 업데이트 함수
    {
        if (staminaSlider != null)
        {
            staminaSlider.value = stamina; // 스태미나 값을 슬라이더의 값으로 설정
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
    /*    void MovePlayer()
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
    // 마우스 입력으로 캐릭터 및 카메라 회전 처리 함수
    void RotatePlayer()
    {
        // 마우스 좌우 움직임으로 캐릭터 회전
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);

        // 마우스 상하 움직임으로 카메라 회전
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;  // 상하 회전 값 조정
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // 카메라 상하 회전 범위 제한

        // 카메라 회전 적용
        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }*/
}
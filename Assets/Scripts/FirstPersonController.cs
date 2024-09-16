using UnityEngine;
using UnityEngine.UI;

public class FirstPersonController : MonoBehaviour
{
    public float moveSpeed = 5f;  // 캐릭터 이동 속도
    public float runSpeed = 7f; // 달리기 속도
    public float mouseSensitivity = 100f;  // 마우스 감도
    public float maxStamina = 5f;  // 최대 스태미나
    public float stamina = 5f;    // 현재 스태미나
    public float staminaDrainRate = 1f;  // 스태미나 소모율
    public float staminaRecoveryRate = 1f; // 스태미나 회복율

    public Slider staminaSlider; // 스태미나 슬라이더

    private CharacterController characterController;
    private float xRotation = 0f;  // 상하 카메라 회전을 위한 변수

    void Start()
    {
        // Character Controller 컴포넌트 가져오기
        characterController = GetComponent<CharacterController>();

        // 마우스 커서 잠금
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 이동 처리
        MovePlayer();

        // 마우스 입력 처리
        RotatePlayer();

        // 스태미나 회복
        RecoverStamina();

        // UI 업데이트
        UpdateStaminaBar();
    }

    // 캐릭터 이동 처리 함수
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
    }

    // 스태미나 회복 함수
    void RecoverStamina()
    {
        if (stamina < maxStamina && !Input.GetKey(KeyCode.LeftShift))
        {
            stamina += staminaRecoveryRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
        }
    }

    // 슬라이더 업데이트 함수
    void UpdateStaminaBar()
    {
        if (staminaSlider != null)
        {
            staminaSlider.value = stamina; // 스태미나 값을 슬라이더의 값으로 설정
        }
    }
}
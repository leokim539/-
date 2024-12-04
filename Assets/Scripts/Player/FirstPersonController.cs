using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class FirstPersonController : MonoBehaviourPunCallbacks
{
    public float moveSpeed = 5f;  // 캐릭터 이동 속도
    public float runSpeed = 7f;  // 달리기 속도
    public KeyCode runningKey = KeyCode.LeftShift; // 달리기 키
    public float maxStamina = 5f;  // 최대 스태미나
    public float stamina = 5f;    // 현재 스태미나
    public float staminaDrainRate = 1f;  // 스태미나 소모율
    public float staminaRecoveryRate = 1f; // 스태미나 회복율
    public Slider staminaSlider; // 스태미나 슬라이더

    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    private Rigidbody rd;
    private AudioSource audioSource; // 오디오 소스

    private float originalMoveSpeed;  // 원래 이동 속도 저장
    private float originalRunSpeed;   // 원래 달리기 속도 저장
    private bool isWalking; // 걷고 있는지 여부

    public AudioClip walkSound; // 걷기 소리
    public AudioClip runSound; // 달리기 소리

    private Animator animator; // 애니메이터 

    // 카메라 관련 변수
    public Camera mainCamera; // MainCamera 오브젝트
    public Camera crouchCamera; // MainCamera2 오브젝트

    void Awake()
    {
        animator = transform.Find("Idel").GetComponent<Animator>(); // 애니메이터 가져옴
    }

    void Start()
    {
        if (photonView.IsMine)
        {
            audioSource = GetComponent<AudioSource>(); // AudioSource 컴포넌트 가져오기
            rd = GetComponent<Rigidbody>();

            originalMoveSpeed = moveSpeed; // 속도 저장
            originalRunSpeed = runSpeed;

            // 카메라를 초기 상태로 설정
            if (mainCamera != null && crouchCamera != null)
            {
                mainCamera.enabled = true; // MainCamera 활성화
                crouchCamera.enabled = false; // MainCamera2 비활성화
            }
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            MovePlayer(); // 이동 처리
            RecoverStamina(); // 스태미나 회복
            UpdateStaminaBar(); // UI 업데이트        
        }
    }

    public void PickingUp()
    {
        Debug.Log("PickingUp 메서드가 호출되었습니다."); // 디버깅 메시지 추가
        animator.SetBool("isCrouched", true);

        // 카메라 전환
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

    void MovePlayer()
    {
        bool isSprinting = Input.GetKey(runningKey) && stamina > 0; // 스프린트 여부 체크
        float targetMovingSpeed = isSprinting ? runSpeed : moveSpeed;

        if (isSprinting) // 스프린트 시 스태미나 소모
        {
            stamina -= staminaDrainRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
            PlaySound(runSound); // 달리기 소리 재생
            animator.SetBool("isWalk", false); // 여기 원래 달리기 애니 재생인데 아직 달리기 애니메이션 없음
        }
        else if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) // 걷기 소리 재생
        {
            PlaySound(walkSound);
            animator.SetBool("isWalk", true); // 걷기 애니메이션 
        }
        else
        {
            audioSource.Stop(); // 소리 정지
            animator.SetBool("isWalk", false); // 걷기 중지 
        }

        Vector2 targetVelocity = new Vector2(Input.GetAxis("Horizontal") * targetMovingSpeed, Input.GetAxis("Vertical") * targetMovingSpeed); // 이동 벡터 계산       
        rd.velocity = transform.rotation * new Vector3(targetVelocity.x, rd.velocity.y, targetVelocity.y); // 캐릭터 이동
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

    void RecoverStamina() // 스태미나 회복 함수
    {
        if (stamina < maxStamina && !Input.GetKey(runningKey))
        {
            stamina += staminaRecoveryRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
        }
    }

    void UpdateStaminaBar() // 슬라이더 업데이트 함수
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

    // 추가한 메서드
    public void ResumeMovement()
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
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FirstPersonController : MonoBehaviour
{
    public float moveSpeed = 5f;  // ĳ���� �̵� �ӵ�
    public float runSpeed = 7f; // �޸��� �ӵ�
    public KeyCode runningKey = KeyCode.LeftShift; //�޸��� Ű
    public float maxStamina = 5f;  // �ִ� ���¹̳�
    public float stamina = 5f;    // ���� ���¹̳�
    public float staminaDrainRate = 1f;  // ���¹̳� �Ҹ���
    public float staminaRecoveryRate = 1f; // ���¹̳� ȸ����
    public Slider staminaSlider; // ���¹̳� �����̴�


    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    private Rigidbody rigidbody;
    private AudioSource audioSource; // ����� �ҽ�

    private float originalMoveSpeed;  // ���� �̵� �ӵ� ����
    private float originalRunSpeed;   // ���� �޸��� �ӵ� ����
    private bool isWalking; // �Ȱ� �ִ��� ����

    public AudioClip walkSound; // �ȱ� �Ҹ�
    public AudioClip runSound; // �޸��� �Ҹ�

    private Animator animator; // �ִϸ����� 

    // ī�޶� ���� ����
    public Camera mainCamera; // MainCamera ������Ʈ
    public Camera crouchCamera; // MainCamera2 ������Ʈ

    void Awake()
    {
        animator = transform.Find("Idel").GetComponent<Animator>(); // �ִϸ����� ������
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // AudioSource ������Ʈ ��������
        rigidbody = GetComponent<Rigidbody>();

        originalMoveSpeed = moveSpeed; // �ӵ� ����
        originalRunSpeed = runSpeed;

        // ī�޶� �ʱ� ���·� ����
        if (mainCamera != null && crouchCamera != null)
        {
            mainCamera.enabled = true; // MainCamera Ȱ��ȭ
            crouchCamera.enabled = false; // MainCamera2 ��Ȱ��ȭ
        }
    }

    void Update()
    {
    }

    void FixedUpdate()
    {
        MovePlayer(); // �̵� ó��
        RecoverStamina(); // ���¹̳� ȸ��
        UpdateStaminaBar(); // UI ������Ʈ
    }

    public void PickingUp()
    {
        Debug.Log("PickingUp �޼��尡 ȣ��Ǿ����ϴ�."); // ����� �޽��� �߰�
        animator.SetBool("isCrouched", true);

        // ī�޶� ��ȯ
        if (mainCamera != null && crouchCamera != null)
        {
            mainCamera.enabled = false; // MainCamera ��Ȱ��ȭ
            crouchCamera.enabled = true; // MainCamera2 Ȱ��ȭ
        }

        // �����̸� �α� ���� �ڷ�ƾ�� ȣ��
        StartCoroutine(DelayCrouch());
    }

    private IEnumerator DelayCrouch()
    {
        yield return new WaitForSeconds(0.5f); // 0.5�� ������
        animator.SetBool("isCrouched", false); // �ɱ׷� �ɴ� �ִϸ��̼� ���¸� ����


        yield return new WaitForSeconds(1f);
        // ī�޶� ���� ���·� �ǵ���
        if (mainCamera != null && crouchCamera != null)
        {
            mainCamera.enabled = true; // MainCamera Ȱ��ȭ
            crouchCamera.enabled = false; // MainCamera2 ��Ȱ��ȭ
        }
    }

    void MovePlayer()
    {
        bool isSprinting = Input.GetKey(runningKey) && stamina > 0;// ������Ʈ ���� üũ
        float targetMovingSpeed = isSprinting ? runSpeed : moveSpeed;

        if (isSprinting)// ������Ʈ �� ���¹̳� �Ҹ�
        {
            stamina -= staminaDrainRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
            PlaySound(runSound); // �޸��� �Ҹ� ���
            animator.SetBool("isWalk", false); // ���� ���� �޸��� �ִ� ����ε� ���� �޸��� �ִϸ��̼� ����
        }
        else if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) // �ȱ� �Ҹ� ���
        {
            PlaySound(walkSound);
            animator.SetBool("isWalk", true); // �ȱ� �ִϸ��̼� 
        }
        else
        {
            audioSource.Stop(); // �Ҹ� ����
            animator.SetBool("isWalk", false); // �ȱ� ���� 
        }

        Vector2 targetVelocity = new Vector2(Input.GetAxis("Horizontal") * targetMovingSpeed, Input.GetAxis("Vertical") * targetMovingSpeed); // �̵� ���� ���       
        rigidbody.velocity = transform.rotation * new Vector3(targetVelocity.x, rigidbody.velocity.y, targetVelocity.y); // ĳ���� �̵�
    }
    private void PlaySound(AudioClip clip)
    {
        if (audioSource.clip != clip || !audioSource.isPlaying) // ���� ��� ���� �Ҹ��� �ٸ� ���
        {
            audioSource.clip = clip;
            audioSource.loop = true; // �Ҹ� �ݺ� ���
            audioSource.Play();
        }
    }
    void RecoverStamina() // ���¹̳� ȸ�� �Լ�
    {
        if (stamina < maxStamina && !Input.GetKey(runningKey))
        {
            stamina += staminaRecoveryRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
        }
    }

    void UpdateStaminaBar() // �����̴� ������Ʈ �Լ�
    {
        if (staminaSlider != null)
        {
            staminaSlider.value = stamina; // ���¹̳� ���� �����̴��� ������ ����
        }
    }

    public void SlowDown(float speedMultiplier)
    {
        moveSpeed *= speedMultiplier; // �̵� �ӵ��� ���̴� ���� (���� �ӵ�)
        runSpeed *= speedMultiplier;

        moveSpeed = Mathf.Max(moveSpeed, originalMoveSpeed * 0.5f); // ���� �ӵ��� 50% ���Ϸ� �������� �ʵ���
        runSpeed = Mathf.Max(runSpeed, originalRunSpeed * 0.5f);
    }

    public void RestoreSpeed()
    {
        moveSpeed = originalMoveSpeed;
        runSpeed = originalRunSpeed;
    }
}
    /*    void MovePlayer()
        {
            float moveX = Input.GetAxis("Horizontal");  // �¿� �̵� �Է�
            float moveZ = Input.GetAxis("Vertical");    // �յ� �̵� �Է�

            // ������Ʈ ���� üũ
            bool isSprinting = Input.GetKey(KeyCode.LeftShift) && stamina > 0;
            float currentSpeed = isSprinting ? runSpeed : moveSpeed;

            // �̵� ���� ���
            Vector3 move = transform.right * moveX + transform.forward * moveZ;

            // ĳ���� �̵�
            characterController.Move(move * currentSpeed * Time.deltaTime);

            // ������Ʈ �� ���¹̳� �Ҹ�
            if (isSprinting)
            {
                stamina -= staminaDrainRate * Time.deltaTime;
                stamina = Mathf.Clamp(stamina, 0, maxStamina);
            }
        }
    // ���콺 �Է����� ĳ���� �� ī�޶� ȸ�� ó�� �Լ�
    void RotatePlayer()
    {
        // ���콺 �¿� ���������� ĳ���� ȸ��
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);

        // ���콺 ���� ���������� ī�޶� ȸ��
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;  // ���� ȸ�� �� ����
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);  // ī�޶� ���� ȸ�� ���� ����

        // ī�޶� ȸ�� ����
        Camera.main.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }*/

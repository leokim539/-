using System.Collections.Generic;
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

    Rigidbody rigidbody;

    private float originalMoveSpeed;  // ���� �̵� �ӵ� ����
    private float originalRunSpeed;   // ���� �޸��� �ӵ� ����

    //public float mouseSensitivity = 100f;  // ���콺 ����
    //private float xRotation = 0f;  // ���� ī�޶� ȸ���� ���� ����
    //private CharacterController characterController;
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        originalMoveSpeed = moveSpeed;//�ӵ� ����
        originalRunSpeed = runSpeed;

        // Character Controller ������Ʈ ��������
        //characterController = GetComponent<CharacterController>();
        //Cursor.lockState = CursorLockMode.Locked;// ���콺 Ŀ�� ���
    }
    void Update()
    {
        
    }
    void FixedUpdate()
    {
        MovePlayer();// �̵� ó��

        RecoverStamina();// ���¹̳� ȸ��

        UpdateStaminaBar();// UI ������Ʈ
    }
    void MovePlayer()
    { 
        bool isSprinting = Input.GetKey(runningKey) && stamina > 0;// ������Ʈ ���� üũ
        float targetMovingSpeed = isSprinting ? runSpeed : moveSpeed;

        if (isSprinting)// ������Ʈ �� ���¹̳� �Ҹ�
        {
            stamina -= staminaDrainRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
        }
        if (speedOverrides.Count > 0)
        {
            targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
        }      
        Vector2 targetVelocity = new Vector2(Input.GetAxis("Horizontal") * targetMovingSpeed, Input.GetAxis("Vertical") * targetMovingSpeed);// �̵� ���� ���       
        rigidbody.velocity = transform.rotation * new Vector3(targetVelocity.x, rigidbody.velocity.y, targetVelocity.y);// ĳ���� �̵�
    } 
    void RecoverStamina()// ���¹̳� ȸ�� �Լ�
    {
        if (stamina < maxStamina && !Input.GetKey(runningKey))
        {
            stamina += staminaRecoveryRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
        }
    }
    void UpdateStaminaBar()// �����̴� ������Ʈ �Լ�
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
}
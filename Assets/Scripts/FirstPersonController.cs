using UnityEngine;
using UnityEngine.UI;

public class FirstPersonController : MonoBehaviour
{
    public float moveSpeed = 5f;  // ĳ���� �̵� �ӵ�
    public float runSpeed = 7f; // �޸��� �ӵ�
    public float mouseSensitivity = 100f;  // ���콺 ����
    public float maxStamina = 5f;  // �ִ� ���¹̳�
    public float stamina = 5f;    // ���� ���¹̳�
    public float staminaDrainRate = 1f;  // ���¹̳� �Ҹ���
    public float staminaRecoveryRate = 1f; // ���¹̳� ȸ����

    public Slider staminaSlider; // ���¹̳� �����̴�

    private CharacterController characterController;
    private float xRotation = 0f;  // ���� ī�޶� ȸ���� ���� ����

    void Start()
    {
        // Character Controller ������Ʈ ��������
        characterController = GetComponent<CharacterController>();

        // ���콺 Ŀ�� ���
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // �̵� ó��
        MovePlayer();

        // ���콺 �Է� ó��
        RotatePlayer();

        // ���¹̳� ȸ��
        RecoverStamina();

        // UI ������Ʈ
        UpdateStaminaBar();
    }

    // ĳ���� �̵� ó�� �Լ�
    void MovePlayer()
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
    }

    // ���¹̳� ȸ�� �Լ�
    void RecoverStamina()
    {
        if (stamina < maxStamina && !Input.GetKey(KeyCode.LeftShift))
        {
            stamina += staminaRecoveryRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, maxStamina);
        }
    }

    // �����̴� ������Ʈ �Լ�
    void UpdateStaminaBar()
    {
        if (staminaSlider != null)
        {
            staminaSlider.value = stamina; // ���¹̳� ���� �����̴��� ������ ����
        }
    }
}
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class FirstPersonController : MonoBehaviourPunCallbacks, IPunObservable
{
    private Animator animator; // �ִϸ�����
    private AudioSource audioSource; // ����� �ҽ�
    private Rigidbody rd;
    private float originalMoveSpeed;  // ���� �̵� �ӵ� ����
    private bool isSettingsOpen = false;

    public float moveSpeed = 5f;  // ĳ���� �̵� �ӵ�
    public float sensitivity = 2;

    private float xRotation = 0f; 
    public Transform character;

    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    [Header("�����")]
    public AudioClip walkSound; // �ȱ� �Ҹ� 

    public Camera mainCamera; // MainCamera ������Ʈ
    public Camera crouchCamera; // MainCamera2 ������Ʈ

    [Header("UI ��ư")]
    public GameObject panel; // �г��� �巡���Ͽ� �����մϴ�.
    public KeyCode keyToPress; // ����â�� Ȱ��ȭ�ϴ� Ű
    public GameObject settingsPanel; // ���� â �г��� ����

    void Awake()
    {
        character = GetComponent<FirstPersonController>().transform;
        animator = transform.Find("Idel").GetComponent<Animator>(); // �ִϸ����� ������
    }

    void Start()
    {
        Debug.Log("IsMine: " + photonView.IsMine);
        settingsPanel = GameObject.Find("ESC");
        panel = GameObject.Find("Tap");
        settingsPanel.SetActive(false);
        if (photonView.IsMine)
        {
            Debug.Log("�� �÷��̾�� ���� ���Դϴ�.");
            audioSource = GetComponent<AudioSource>(); // AudioSource ������Ʈ ��������
            rd = GetComponent<Rigidbody>();
            rd.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            originalMoveSpeed = moveSpeed; // �ӵ� ����

            // ī�޶� �ʱ� ���·� ����
            if (mainCamera != null && crouchCamera != null)
            {
                mainCamera.enabled = true; // MainCamera Ȱ��ȭ
                crouchCamera.enabled = false; // MainCamera2 ��Ȱ��ȭ
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
                MovePlayer(); // �̵� ó��
                CameraRotation(); // ī�޶� ó��
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

        if (isSettingsOpen) // ���� â�� ������ ��
        {
            Cursor.visible = true; // ���콺 Ŀ�� ǥ��
            Cursor.lockState = CursorLockMode.None; // ���콺 �̵� ����
            audioSource.Stop();
        }

    }
   // void MovePlayer()
   // {
     //   float x = Input.GetAxis("Horizontal");
      //  float z = Input.GetAxis("Vertical");
       // Vector3 move = transform.right * x + transform.forward * z;
       // transform.position += move * moveSpeed * Time.deltaTime;

       // if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) // �ȱ� �Ҹ� ���
       // {
       //     PlaySound(walkSound);
      //  }
      //  else
       // {
       //     audioSource.Stop(); // �Ҹ� ����
      //  }



      //  UpdateAnimator(move);
  //  }

    void MovePlayer()// ����� �޼���(�ִϸ��̼� �ӵ� ����)
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // �̵� ���� ���
        Vector2 targetVelocity = new Vector2(x * moveSpeed, z * moveSpeed);

        // ĳ���� �̵�
        rd.velocity = transform.rotation * new Vector3(targetVelocity.x, rd.velocity.y, targetVelocity.y); 

        // �ȱ� �Ҹ� ���
        if (x != 0 || z != 0) // �̵� ���� ��
        {
            PlaySound(walkSound);
        }
        else
        {
            audioSource.Stop(); // �Ҹ� ����
        }

        UpdateAnimator(targetVelocity); // �ִϸ��̼� ���� ������Ʈ
    }


    private void UpdateAnimator(Vector2 targetVelocity)
    {
        // �̵� ������ ũ�⸦ ����Ͽ� Forward�� Strafe �Ķ���Ϳ� ����
        float forward = targetVelocity.y / moveSpeed; // moveSpeed�� �������� ���� ���� ���
        float strafe = targetVelocity.x / moveSpeed;  // moveSpeed�� �������� ���� ���� ���

        animator.SetFloat("Forward", forward); // Forward �Ķ���� ����
        animator.SetFloat("Strafe", strafe);   // Strafe �Ķ���� ����

        // �ȱ� �ִϸ��̼� ���� ó��
        // isWalk �Ķ���� ��� Forward�� Strafe ���� ����Ͽ� �ȱ� ���¸� �Ǵ�
        if (Mathf.Abs(forward) > 0.01f || Mathf.Abs(strafe) > 0.01f) // �̵� ���̸�
        {
            animator.SetFloat("isWalking", 1f); // �ȱ� ���� ���� (1)
        }
        else
        {
            animator.SetFloat("isWalking", 0f); // �ȱ� ���� ���� (0)
        }
    }
    void CameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity*2 * Time.deltaTime * 100; // ���콺 X �̵�
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity*2 * Time.deltaTime * 100; // ���콺 Y �̵�

        xRotation -= mouseY; // ī�޶� ���� ȸ���� ����
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // ���� ȸ�� ����

        mainCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f); // ī�޶� ���� ȸ��
        character.Rotate(Vector3.up * mouseX); // ĳ���� �¿� ȸ��

    }
    public void PickingUp()
    {
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
    

    private void PlaySound(AudioClip clip)
    {
        if (audioSource.clip != clip || !audioSource.isPlaying) // ���� ��� ���� �Ҹ��� �ٸ� ���
        {
            audioSource.clip = clip;
            audioSource.loop = true; // �Ҹ� �ݺ� ���
            audioSource.Play();
        }
    }

    public void SlowDown(float speedMultiplier)
    {
        moveSpeed *= speedMultiplier; // �̵� �ӵ��� ���̴� ���� (���� �ӵ�)

        moveSpeed = Mathf.Max(moveSpeed, originalMoveSpeed * 0.5f); // ���� �ӵ��� 50% ���Ϸ� �������� �ʵ���
    }

    public void RestoreSpeed()
    {
        moveSpeed = originalMoveSpeed;
    }

    // �߰��� �޼���
    public void StopMovement()
    {
        if (!photonView.IsMine) return; // ���� �÷��̾ ó��

        if (audioSource != null)
        {
            audioSource.Stop(); // �Ҹ� ����
        }

        if (animator != null)
        {
            animator.SetBool("isWalk", false); // �ȱ� �ִϸ��̼� �ߴ�
            animator.SetBool("isCrouched", false); // �ɱ׸��� �ִϸ��̼� �ߴ�
        }

        if (rd != null)
        {
            rd.velocity = Vector3.zero; // ĳ���� �ӵ� �ʱ�ȭ
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // �� ��ġ�� ȸ�� ������ ����
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // �ٸ� �÷��̾��� ��ġ�� ȸ�� ������ ����
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
        }
    }
    // �߰��� �޼���
    /*public void ResumeMovement()
    {
        // Ư���� ���� �ʿ� ���� (Update���� ó��)
    }

    /* �ּ� �ڵ�: ��ü MovePlayer ���� (���� ���)
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
    */
}
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;

public class FirstPersonController : MonoBehaviourPunCallbacks, IPunObservable
{
    private float forward; // Forward �ִϸ��̼� ����
    private float strafe;  // Strafe �ִϸ��̼� ����
    private float isWalking; // �ȱ� ���� (0 �Ǵ� 1)
    private bool isRunning = false; // �ٴ� �ִϸ��̼� ����

    private Vector3 networkPosition; // ��Ʈ��ũ���� ������ ��ġ
    private Quaternion networkRotation; // ��Ʈ��ũ���� ������ ȸ��
    private float lerpRate = 10f; // ���� �ӵ�

    // �ִϸ��̼� ���� ����
    private bool networkIsRunning; // ��Ʈ��ũ���� ������ �޸��� ����
    private float networkForward; // ��Ʈ��ũ���� ������ Forward �ִϸ��̼� ��
    private float networkStrafe; // ��Ʈ��ũ���� ������ Strafe �ִϸ��̼� ��
    private float networkIsWalking; // ��Ʈ��ũ���� ������ �ȱ� ����

    public Trash2 trash2;
    private Animator animator; // �ִϸ�����
    private AudioSource audioSource; // ����� �ҽ�
    private Rigidbody rd;
    private float originalMoveSpeed;  // ���� �̵� �ӵ� ����
    private float originalRunSpeed;   // ���� �޸��� �ӵ� ����
    private bool isSettingsOpen = false;

    private bool isExhausted = false; // ���¹̳� ���� ���� Ȯ��

    public float moveSpeed = 5f;  // ĳ���� �̵� �ӵ�
    public float sensitivity = 2;
    public float runSpeed = 7f; // �޸��� �ӵ�
    public KeyCode runningKey = KeyCode.LeftShift; //�޸��� Ű
    public float maxStamina = 5f;  // �ִ� ���¹̳�
    public float stopStamina = 2.5f; // ����ٰ� �ٽ� ������ �� �ִ� ���¹̳� �Ӱ谪
    public float stamina = 5f;    // ���� ���¹̳�
    public float staminaDrainRate = 1f;  // ���¹̳� �Ҹ���
    public float staminaRecoveryRate = 1f; // ���¹̳� ȸ����
    public Slider staminaSlider; // ���¹̳� �����̴�

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
    public PhotonView m_PhotonView;

    public bool canMove; 
    public bool bond = true;
    public bool coffee = true;
    [Header("������")]
    public bool spoon = true;
    void Awake()
    {
        ItemManager.instane.firstPersonController = this;
        m_PhotonView = GetComponent<PhotonView>();
        character = GetComponent<FirstPersonController>().transform;
        animator = transform.Find("Idel").GetComponent<Animator>(); // �ִϸ����� ������
    }
    void Start()
    {
        // ���¹̳��� �ִ밪���� �ʱ�ȭ
        stamina = maxStamina;

        if (staminaSlider == null)
        {
            GameObject sliderObject = GameObject.Find("StaminaSlider");
            if (sliderObject != null)
            {
                staminaSlider = sliderObject.GetComponent<Slider>();
            }
        }


        // �����̴� ����
        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina; // �����̴� �ִ밪 ����ȭ
            staminaSlider.value = stamina;       // �����̴� �ʱⰪ ����ȭ
        }

        Debug.Log("IsMine: " + photonView.IsMine);
        settingsPanel = GameObject.Find("ESC");
        panel = GameObject.Find("Tap");

        if (photonView.IsMine)
        {
            settingsPanel.SetActive(false);
            Debug.Log("�� �÷��̾�� ���� ���Դϴ�.");
            audioSource = GetComponent<AudioSource>(); // AudioSource ������Ʈ ��������
            rd = GetComponent<Rigidbody>();
            rd.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            originalMoveSpeed = moveSpeed; // �ӵ� ����
            originalRunSpeed = runSpeed;

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
                CameraRotation(); // ī�޶� ó��
                if (canMove)
                {
                    MovePlayer(); // �̵� ó��
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

        if (isSettingsOpen) // ���� â�� ������ ��
        {
            Cursor.visible = true; // ���콺 Ŀ�� ǥ��
            Cursor.lockState = CursorLockMode.None; // ���콺 �̵� ����
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

        if (isExhausted) // ���¹̳� ���� ������ ���
        {
            RecoverStamina(); // ���¹̳� ȸ��
            UpdateStaminaBar(); // UI ������Ʈ

            if (stamina >= stopStamina) // stopStamina �̻� ȸ���Ǹ� ���� ���� ����
            {
                isExhausted = false;
            }
            else
            {
                StopMovement(); // ��� ���� ���� ����
                isRunning = false; // ���� ���¿��� �ٱ� ���� ����
                return;
            }
        }

        // �׻� ���¹̳� ȸ��
        if (!isExhausted && stamina < maxStamina)
        {
            RecoverStamina();
            UpdateStaminaBar();
        }

        // �̵� ���� �� ������Ʈ ���� Ȯ��
        bool isMoving = Mathf.Abs(x) > 0 || Mathf.Abs(z) > 0; // �̵� ����
        if (bond)
            isRunning = isMoving && Input.GetKey(runningKey) && stamina > 0; // ������Ʈ ���� ����

        // ������Ʈ �� �ӵ� �� ���¹̳� �Ҹ� ó��
        float targetMovingSpeed = isRunning ? runSpeed : moveSpeed;

        if (isRunning) // ������Ʈ �� ���¹̳� �Ҹ�
        {
            if (coffee)
            {
                stamina -= staminaDrainRate * Time.deltaTime; // �ʴ� �Ҹ�
                stamina = Mathf.Clamp(stamina, 0, maxStamina); // ���¹̳� ���� ����

                UpdateStaminaBar(); // �����̴� ������Ʈ

                if (stamina <= 0) // ���¹̳��� �����Ǹ�
                {
                    isExhausted = true; // ���� ���� Ȱ��ȭ
                    isRunning = false; // ������Ʈ ����
                    Debug.Log("Stamina is exhausted, stopping player. isRunning: " + isRunning);
                    StopMovement(); // ĳ���� ����
                    return;
                }
            }
            
        }


        // �̵� ó��
        Vector2 targetVelocity = new Vector2(x * targetMovingSpeed, z * targetMovingSpeed);
        rd.velocity = transform.rotation * new Vector3(targetVelocity.x, rd.velocity.y, targetVelocity.y);

        // �ȱ�/�޸��� ���� ���
        if (isMoving)
        {
            PlaySound(walkSound);
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop(); // ���� �ߴ�
            }
        }

        UpdateAnimator(targetVelocity); // �ִϸ��̼� ���� ������Ʈ
    }
    public void StopMovement()
    {
        // Photon ��Ʈ��ũ ó�� (�ʿ��� ���)
        if (photonView != null && !photonView.IsMine) return;

        // ����� ����
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        // �ִϸ����� ó�� (�� ��ũ��Ʈ�� ��� ��� ����)
        if (animator != null)
        {
            // Photon ��ũ��Ʈ ��Ÿ��
            animator.SetBool("isWalk", false);
            animator.SetBool("isCrouched", false);
            animator.SetBool("isRunning", false);

            // ���� ��ũ��Ʈ ��Ÿ�� �߰�
            animator.SetFloat("Forward", 0f);
            animator.SetFloat("Strafe", 0f);
            animator.SetFloat("isWalking", 0f);
        }

        // �ӵ� �ʱ�ȭ
        if (rd != null)
        {
            rd.velocity = Vector3.zero;
        }

        // ������: isRunning ���� ����
        isRunning = false;

        // ������: ����� �α�
        Debug.Log("StopMovement called. isRunning set to: " + isRunning);
    }
    void RecoverStamina()
    {
        // ����Ʈ�� ������ ���� �� �Ǵ� �̵� ���� �ƴ� �� ���¹̳� ȸ��
        if (!Input.GetKey(runningKey) && stamina < maxStamina)
        {
            stamina += staminaRecoveryRate * Time.deltaTime; // �ʴ� 1�� ȸ��
            stamina = Mathf.Clamp(stamina, 0, maxStamina); // ���¹̳� ���� ����
        }
    }
    void UpdateStaminaBar()
    {
        if (staminaSlider != null)
        {
            staminaSlider.value = stamina; // ���¹̳� ���� �����̴��� �ݿ�
        }
    }
    private void UpdateAnimator(Vector2 targetVelocity)
    {
        // �̵� ������ ũ�⸦ ����Ͽ� Forward�� Strafe �Ķ���Ϳ� ����
        forward = targetVelocity.y / moveSpeed; // moveSpeed�� �������� ���� ���� ���
        strafe = targetVelocity.x / moveSpeed;  // moveSpeed�� �������� ���� ���� ���

        animator.SetFloat("Forward", forward); // Forward �Ķ���� ����
        animator.SetFloat("Strafe", strafe);   // Strafe �Ķ���� ����

        // �ȱ� �ִϸ��̼� ���� ó��
        if (Mathf.Abs(forward) > 0.01f || Mathf.Abs(strafe) > 0.01f) // �̵� ���̸�
        {
            isWalking = 1f; // �ȱ� ���� ���� (1)
        }
        else
        {
            isWalking = 0f; // �ȱ� ���� ���� (0)
        }


        animator.SetFloat("isWalking", isWalking); // �ִϸ����Ϳ� ����

        // ������Ʈ ���� �ִϸ��̼� ����
        animator.SetBool("isRunning", isRunning); // isRunning �Ķ���� ����
    }

    private void InterpolateMovement()
    {
        // ��Ʈ��ũ ��ġ�� ȸ������ ����
        transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * lerpRate);
        transform.rotation = Quaternion.Slerp(transform.rotation, networkRotation, Time.deltaTime * lerpRate);
    }

    private void UpdateAnimatorFromNetwork()
    {
        // ��Ʈ��ũ���� ������ �ִϸ��̼� ���� ������Ʈ
        animator.SetFloat("Forward", networkForward);
        animator.SetFloat("Strafe", networkStrafe);
        animator.SetFloat("isWalking", networkIsWalking);
        animator.SetBool("isRunning", networkIsRunning);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // �� ��ġ�� ȸ�� ������ ����
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(forward);
            stream.SendNext(strafe);
            stream.SendNext(isWalking);
            stream.SendNext(isRunning); // �޸��� ���µ� ����
        }
        else
        {
            // �ٸ� �÷��̾��� ��ġ�� ȸ�� ������ ����
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            networkForward = (float)stream.ReceiveNext(); // Forward ����
            networkStrafe = (float)stream.ReceiveNext();  // Strafe ����
            networkIsWalking = (float)stream.ReceiveNext(); // isWalking ����
            networkIsRunning = (bool)stream.ReceiveNext(); // isRunning ����

            // �ִϸ����� ������Ʈ
            UpdateAnimatorFromNetwork();
        }
    }
    void CameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * 2 * Time.deltaTime * 100; // ���콺 X �̵�
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * 2 * Time.deltaTime * 100; // ���콺 Y �̵�

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
        runSpeed *= speedMultiplier;

        moveSpeed = Mathf.Max(moveSpeed, originalMoveSpeed * 0.5f); // ���� �ӵ��� 50% ���Ϸ� �������� �ʵ���
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
        StartCoroutine(TaserGun()); // Ÿ���� 5�ʰ� ����
    }
    public IEnumerator TaserGun()
    {
        canMove = false; // ������ �̵� ��Ȱ��ȭ
        yield return new WaitForSeconds(5f); // 5�� ���
        canMove = true; // ������ �̵� Ȱ��ȭ
    }
    [PunRPC]
    public void ApplyObscuringEffects()
    {
        StartCoroutine(ApplyObscuringEffect()); // ��� �þ� 5�ʰ� �Ⱥ���
    }

    
    public IEnumerator ApplyObscuringEffect()
    {
        GameObject canvasInstance = PhotonNetwork.Instantiate(trash2.canvasPrefab.name, transform.position, Quaternion.identity, 0);
        canvasInstance.transform.SetParent(transform); // ���� ������Ʈ�� �ڽ����� ����

        RectTransform rectTransform = canvasInstance.GetComponent<RectTransform>();
        rectTransform.localPosition = Vector3.zero;
        rectTransform.localScale = new Vector3(1, 1, 1);

        yield return new WaitForSeconds(5);

        PhotonNetwork.Destroy(canvasInstance);
    }
    [PunRPC]
    public void Hackings()
    {
        StartCoroutine(Hacking()); // �� 5�ʰ� ��ŷ
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
        StartCoroutine(HandCream()); // �� 5�ʰ� ��ŷ
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
        StartCoroutine(Bond()); // �� 5�ʰ� ��ŷ
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
        StartCoroutine(Spoon()); // �� 5�ʰ� ��ŷ
    }
    public IEnumerator Spoon()
    {
        spoon = false;
        yield return new WaitForSeconds(10);
        spoon = true;
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
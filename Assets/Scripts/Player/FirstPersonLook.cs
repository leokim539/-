using UnityEngine;
using Photon.Pun;
public class FirstPersonLook : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform character;            // 캐릭터의 몸체를 가리킴 (Y축 회전용)
    public float sensitivity = 2;
    public float smoothing = 1.5f;
    public GameObject settingsPanel;        // 설정 창 패널을 연결

    private Vector2 velocity;
    private Vector2 frameVelocity;
    private FirstPersonController firstPersonController; // FirstPersonController 스크립트 참조
    private Rigidbody rb;                   // Rigidbody 참조
    private bool isSettingsOpen = false;    // 설정 창 열림 상태 확인 변수

    void Reset()
    {
        // 부모 오브젝트의 FirstPersonController와 Transform을 가져옴
        character = GetComponentInParent<FirstPersonController>().transform;
    }

    void Start()
    {
        // FirstPersonController 및 Rigidbody 캐싱
        firstPersonController = GetComponentInParent<FirstPersonController>();
        rb = GetComponentInParent<Rigidbody>();

        // Rigidbody가 있을 경우 물리 회전을 제한
        if (rb != null)
        {
            rb.freezeRotation = true; // Rigidbody의 회전을 고정
        }

        // 게임 시작 시 마우스 커서 고정 및 숨기기
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // ESC 키로 설정 창을 토글
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettingsPanel();
        }

        // 설정 창이 닫혀 있을 때만 캐릭터 회전
        if (!isSettingsOpen)
        {
            RotateCharacter();
        }
        else if (rb != null)
        {
            // 설정 창이 열렸을 때 Rigidbody의 각속도를 초기화하여 회전 방지
            rb.angularVelocity = Vector3.zero;
        }
    }

    // 캐릭터 회전 메서드
    private void RotateCharacter()
    {
        if (photonView.IsMine) // 이 오브젝트가 내 것일 때만 회전
        {
            Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
            frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
            velocity += frameVelocity;
            velocity.y = Mathf.Clamp(velocity.y, -90, 90);

            transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
            character.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);
        }
    }

    // 설정 창을 열고 닫는 메서드
    private void ToggleSettingsPanel()
    {
        isSettingsOpen = !isSettingsOpen;
        settingsPanel.SetActive(isSettingsOpen);

        if (isSettingsOpen) // 설정 창이 열렸을 때
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // FirstPersonController 비활성화
            if (firstPersonController != null)
            {
                firstPersonController.enabled = false;
            }
        }
        else // 설정 창이 닫혔을 때
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            // FirstPersonController 활성화
            if (firstPersonController != null)
            {
                firstPersonController.enabled = true;
            }
        }
    }
}
using UnityEngine;

public class FirstPersonLook : MonoBehaviour
{
    [SerializeField]
    Transform character;
    public float sensitivity = 2;
    public float smoothing = 1.5f;
    public GameObject settingsPanel; // 설정 창 패널을 연결

    private Vector2 velocity;
    private Vector2 frameVelocity;
    private FirstPersonController firstPersonController; // FirstPersonController 스크립트 참조

    void Reset()
    {
        character = GetComponentInParent<FirstPersonController>().transform;
    }

    void Start()
    {
        // 게임 시작 시 마우스 커서 고정 및 숨기기
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // FirstPersonController 스크립트 가져와서 캐싱
        firstPersonController = GetComponentInParent<FirstPersonController>();
    }

    void Update()
    {
        // ESC 키가 눌렸는지 확인하여 설정 창 토글
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool isActive = settingsPanel.activeSelf;
            settingsPanel.SetActive(!isActive);

            if (!isActive) // 설정 창이 활성화되면
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                // FirstPersonController 비활성화
                if (firstPersonController != null)
                {
                    firstPersonController.enabled = false;
                }
            }
            else // 설정 창이 비활성화되면
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

        // 설정 창이 비활성화된 경우에만 캐릭터 회전
        if (!settingsPanel.activeSelf)
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
}
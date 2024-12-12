using UnityEngine;

public class FloorTP : MonoBehaviour
{
    public GameObject triggerObject; // 트리거 역할을 할 빈 오브젝트
    public GameObject targetObject; // 순간이동할 빈 오브젝트
    public GameObject FUI; // 활성화할 UI 요소
    private bool isInTrigger = false; // 트리거 안에 있는지 여부

    void Start()
    {
        // FUI를 비활성화합니다.
        FUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 플레이어가 트리거에 들어갔는지 확인
        {
            isInTrigger = true;
            FUI.SetActive(true); // FUI 활성화
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // 플레이어가 트리거에서 나갔는지 확인
        {
            isInTrigger = false;
            FUI.SetActive(false); // FUI 비활성화
        }
    }

    void Update()
    {
        if (isInTrigger && Input.GetKeyDown(KeyCode.F)) // F 키가 눌렸을 때
        {
            // 순간이동
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = targetObject.transform.position; // 플레이어 위치를 타겟 오브젝트 위치로 변경
            }
        }
    }
}

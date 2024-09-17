using UnityEngine;

public class DangerItem : MonoBehaviour
{
    public float slowedSpeedMultiplier = 5f;  // 이동 속도를 줄이는 배율 (절반 속도)
    public float slowDuration = 5f;  // 속도 감소 지속 시간

    // 플레이어와 충돌했을 때 호출되는 함수
    void OnTriggerEnter(Collider other)
    {
        // 충돌한 오브젝트가 "Player" 태그를 가지고 있는지 확인
        if (other.CompareTag("Player"))
        {
            // FirstPersonController 스크립트를 찾아서 속도 감소 함수 호출
            FirstPersonController playerController = other.GetComponent<FirstPersonController>();
            if (playerController != null)
            {
                // 플레이어의 속도를 감소시키는 함수 호출
                playerController.SlowDown(slowedSpeedMultiplier, slowDuration);
            }

            // 아이템 제거 (필요 시)
            Destroy(gameObject);
        }
    }
}
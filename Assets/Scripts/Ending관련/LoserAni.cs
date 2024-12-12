using UnityEngine;

public class LoserAni : MonoBehaviour
{
    void Start()
    {
        // "Idle"이라는 이름의 자식 오브젝트에서 Animator 컴포넌트를 찾습니다.
        Transform idleTransform = transform.Find("Idle");

        if (idleTransform != null)
        {
            // Animator 컴포넌트를 가져와서 바로 패배 애니메이션 실행
            Animator animator = idleTransform.GetComponent<Animator>();
            if (animator != null)
            {
                animator.Play("defeat motion"); // "LoseAnimation"을 실제 애니메이션 이름으로 변경
            }
            else
            {
                Debug.LogWarning("Idle 오브젝트에 Animator 컴포넌트가 없습니다.");
            }
        }
        else
        {
            // Idle 오브젝트를 찾지 못했을 경우 경고 메시지 출력
            Debug.LogWarning("Idle 오브젝트를 찾을 수 없습니다.");
        }
    }
}

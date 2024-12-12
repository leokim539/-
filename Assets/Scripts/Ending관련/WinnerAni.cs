using UnityEngine;

public class WinnerAni : MonoBehaviour
{
    void Start()
    {
        // "Idle"�̶�� �̸��� �ڽ� ������Ʈ���� Animator ������Ʈ�� ã���ϴ�.
        Transform idleTransform = transform.Find("Idle");

        if (idleTransform != null)
        {
            // Animator ������Ʈ�� �����ͼ� �¸� �ִϸ��̼� ����
            Animator animator = idleTransform.GetComponent<Animator>();
            if (animator != null)
            {
                animator.Play("victory motion"); // "WinAnimation"�� ���� �ִϸ��̼� �̸����� ����
            }
            else
            {
                Debug.LogWarning("Idle ������Ʈ�� Animator ������Ʈ�� �����ϴ�.");
            }
        }
        else
        {
            // Idle ������Ʈ�� ã�� ������ ��� ��� �޽��� ���
            Debug.LogWarning("Idle ������Ʈ�� ã�� �� �����ϴ�.");
        }
    }
}

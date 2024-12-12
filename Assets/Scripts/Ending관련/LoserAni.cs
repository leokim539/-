using UnityEngine;

public class LoserAni : MonoBehaviour
{
    void Start()
    {
        // "Idle"�̶�� �̸��� �ڽ� ������Ʈ���� Animator ������Ʈ�� ã���ϴ�.
        Transform idleTransform = transform.Find("Idle");

        if (idleTransform != null)
        {
            // Animator ������Ʈ�� �����ͼ� �ٷ� �й� �ִϸ��̼� ����
            Animator animator = idleTransform.GetComponent<Animator>();
            if (animator != null)
            {
                animator.Play("defeat motion"); // "LoseAnimation"�� ���� �ִϸ��̼� �̸����� ����
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

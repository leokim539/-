using UnityEngine;

public class DangerItem : MonoBehaviour
{
    public float slowedSpeedMultiplier = 5f;  // �̵� �ӵ��� ���̴� ���� (���� �ӵ�)
    public float slowDuration = 5f;  // �ӵ� ���� ���� �ð�

    // �÷��̾�� �浹���� �� ȣ��Ǵ� �Լ�
    void OnTriggerEnter(Collider other)
    {
        // �浹�� ������Ʈ�� "Player" �±׸� ������ �ִ��� Ȯ��
        if (other.CompareTag("Player"))
        {
            // FirstPersonController ��ũ��Ʈ�� ã�Ƽ� �ӵ� ���� �Լ� ȣ��
            FirstPersonController playerController = other.GetComponent<FirstPersonController>();
            if (playerController != null)
            {
                // �÷��̾��� �ӵ��� ���ҽ�Ű�� �Լ� ȣ��
                playerController.SlowDown(slowedSpeedMultiplier, slowDuration);
            }

            // ������ ���� (�ʿ� ��)
            Destroy(gameObject);
        }
    }
}
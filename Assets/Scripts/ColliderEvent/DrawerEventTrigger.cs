using UnityEngine;

public class DrawerEventTrigger : MonoBehaviour
{
    private bool isTriggered = false; // Ʈ���Ű� ����Ǿ����� ���θ� ��Ÿ���� ����

    private void OnTriggerEnter(Collider other)
    {
        // �÷��̾�� �浹�ߴ��� Ȯ��
        if (!isTriggered && other.CompareTag("Player")) // �̹� Ʈ���Ű� ������� �ʾ��� ����
        {
            // �ڽ� ������Ʈ �� "OutoDrawer"��� �̸��� ������Ʈ ã��
            Transform outoDrawerTransform = transform.Find("OutoDrawer");

            if (outoDrawerTransform != null)
            {
                // DrawerController ������Ʈ�� ã��
                DrawerController drawerController = outoDrawerTransform.GetComponent<DrawerController>();

                if (drawerController != null)
                {
                    // ���� ���� ��� ����
                    StartCoroutine(drawerController.ToggleDrawer());
                    isTriggered = true; // Ʈ���Ű� ����Ǿ����� ǥ��
                }
                else
                {
                    Debug.LogWarning("DrawerController not found on OutoDrawer.");
                }
            }
            else
            {
                Debug.LogWarning("OutoDrawer not found as a child of " + gameObject.name);
            }
        }
    }
}

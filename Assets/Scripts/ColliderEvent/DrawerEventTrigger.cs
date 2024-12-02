using UnityEngine;

public class DrawerEventTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // �÷��̾�� �浹�ߴ��� Ȯ��
        if (other.CompareTag("Player"))
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

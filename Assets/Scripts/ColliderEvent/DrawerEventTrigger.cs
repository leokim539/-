using UnityEngine;

public class DrawerEventTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 플레이어와 충돌했는지 확인
        if (other.CompareTag("Player"))
        {
            // 자식 오브젝트 중 "OutoDrawer"라는 이름의 오브젝트 찾기
            Transform outoDrawerTransform = transform.Find("OutoDrawer");

            if (outoDrawerTransform != null)
            {
                // DrawerController 컴포넌트를 찾기
                DrawerController drawerController = outoDrawerTransform.GetComponent<DrawerController>();

                if (drawerController != null)
                {
                    // 서랍 열기 기능 실행
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

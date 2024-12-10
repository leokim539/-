using UnityEngine;

public class DrawerEventTrigger : MonoBehaviour
{
    private bool isTriggered = false; // 트리거가 실행되었는지 여부를 나타내는 변수

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어와 충돌했는지 확인
        if (!isTriggered && other.CompareTag("Player")) // 이미 트리거가 실행되지 않았을 때만
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
                    isTriggered = true; // 트리거가 실행되었음을 표시
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

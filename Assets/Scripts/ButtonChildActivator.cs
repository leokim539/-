using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonChildActivator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject childImage; // 자식 오브젝트를 저장

    private void Start()
    {
        // 자식 오브젝트를 가져옴
        if (transform.childCount > 0)
        {
            childImage = transform.GetChild(0).gameObject; // 첫 번째 자식을 가져옴
            childImage.SetActive(false); // 기본적으로 비활성화
        }
    }

    // 마우스를 버튼 위에 올릴 때 실행
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (childImage != null)
        {
            childImage.SetActive(true); // 자식 오브젝트 활성화
        }
    }

    // 마우스를 버튼에서 벗어날 때 실행
    public void OnPointerExit(PointerEventData eventData)
    {
        if (childImage != null)
        {
            childImage.SetActive(false); // 자식 오브젝트 비활성화
        }
    }
}

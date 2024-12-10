using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonChildActivator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject childImage; // �ڽ� ������Ʈ�� ����

    private void Start()
    {
        // �ڽ� ������Ʈ�� ������
        if (transform.childCount > 0)
        {
            childImage = transform.GetChild(0).gameObject; // ù ��° �ڽ��� ������
            childImage.SetActive(false); // �⺻������ ��Ȱ��ȭ
        }
    }

    // ���콺�� ��ư ���� �ø� �� ����
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (childImage != null)
        {
            childImage.SetActive(true); // �ڽ� ������Ʈ Ȱ��ȭ
        }
    }

    // ���콺�� ��ư���� ��� �� ����
    public void OnPointerExit(PointerEventData eventData)
    {
        if (childImage != null)
        {
            childImage.SetActive(false); // �ڽ� ������Ʈ ��Ȱ��ȭ
        }
    }
}

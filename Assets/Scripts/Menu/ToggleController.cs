using UnityEngine;
using UnityEngine.UI;

public class ToggleController : MonoBehaviour
{
    public Toggle myToggle; // Toggle ������Ʈ ����

    void Start()
    {
        // �ʱ�ȭ: üũ��ũ�� ��Ȱ��ȭ
        myToggle.isOn = false;
        myToggle.onValueChanged.AddListener(OnToggleChanged);
    }

    void OnToggleChanged(bool isOn)
    {
        if (isOn)
        {
            // Toggle�� Ȱ��ȭ�� ��
            Debug.Log("Toggle ON: üũ��ũ�� ���Դϴ�.");
        }
        else
        {
            // Toggle�� ��Ȱ��ȭ�� ��
            Debug.Log("Toggle OFF: üũ��ũ�� �������ϴ�.");
        }
    }

    void OnDestroy()
    {
        // �̺�Ʈ ����
        myToggle.onValueChanged.RemoveListener(OnToggleChanged);
    }
}
using System.Collections;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public Light[] lights;  // ���� ����Ʈ�� ���� �迭
    public float delayTime = 5.0f;  // ���� ���� �� ��� �ð�

    private void Start()
    {
        // ���� �ð��� ���� �� ����Ʈ�� ���� �ڷ�ƾ ����
        StartCoroutine(TurnOffLightsAfterDelay());
    }

    private IEnumerator TurnOffLightsAfterDelay()
    {
        // ������ �ð���ŭ ���
        yield return new WaitForSeconds(delayTime);

        // �� ����Ʈ�� Ȱ��ȭ ���¸� ��Ȱ��ȭ�� ���� (���� ����)
        foreach (Light light in lights)
        {
            light.enabled = false;
        }
    }
}

using System.Collections;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public Light[] lights;            // ���� ����Ʈ�� ���� �迭
    public float delayTime = 5.0f;     // ���� ���� �� ��� �ð�
    public int blinkCount = 3;         // �����Ÿ� Ƚ��
    public float blinkInterval = 0.2f; // �����Ÿ� ���� (�� ����)

    private void Start()
    {
        // ���� �ð��� ���� �� ����Ʈ�� ���� �ڷ�ƾ ����
        StartCoroutine(TurnOffLightsAfterBlink());
    }

    private IEnumerator TurnOffLightsAfterBlink()
    {
        // ������ �ð���ŭ ���
        yield return new WaitForSeconds(delayTime);

        // ������ Ƚ����ŭ �����Ÿ�
        for (int i = 0; i < blinkCount; i++)
        {
            foreach (Light light in lights)
            {
                light.enabled = !light.enabled; // ����Ʈ ���� ����
            }
            yield return new WaitForSeconds(blinkInterval); // �����Ÿ� ���� ���
        }

        // �����Ÿ��� ���� �� ��� ����Ʈ ����
        foreach (Light light in lights)
        {
            light.enabled = false;
        }
    }
}
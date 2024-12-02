using UnityEngine;
using System.Collections;

public class EffectManager : MonoBehaviour
{
    private float effectDuration = 8f; // ȿ�� ���� �ð�
    private GameObject activeVolume; // ���� Ȱ��ȭ�� GameObject ����
    private Coroutine effectCoroutine; // ȿ�� �ڷ�ƾ ����

    // EffectTrash���� ȣ��� �޼��� (GameObject�� �Ķ���ͷ� ����)
    public void ActivatePostProcessEffect(GameObject postProcessObject)
    {
        if (postProcessObject != null)
        {
            // ���� ���޹��� GameObject Ȱ��ȭ
            activeVolume = postProcessObject;
            activeVolume.SetActive(true); // ȿ�� ��� Ȱ��ȭ
            effectCoroutine = StartCoroutine(ActivateEffectTemporarily(postProcessObject));
        }
        if (postProcessObject != null)
        {
            // �ڽ� ������Ʈ �� GameObject ã�Ƽ� Ȱ��ȭ
            GameObject[] childVolumes = GetComponentsInChildren<GameObject>();
            if (childVolumes.Length > 0)
            {
                activeVolume = childVolumes[0];
                activeVolume.SetActive(true); // ȿ�� ��� Ȱ��ȭ
                effectCoroutine = StartCoroutine(ActivateEffectTemporarily(childVolumes[0]));
            }
        }
    }

    private IEnumerator ActivateEffectTemporarily(GameObject volume)
    {
        // ������ �ð� ���� ���
        yield return new WaitForSeconds(effectDuration);

        // GameObject ��Ȱ��ȭ
        volume.SetActive(false);
        activeVolume = null;
        effectCoroutine = null;
    }

    public void DeactivatePostProcessEffect()
    {
        if (activeVolume != null)
        {
            activeVolume.SetActive(false); // GameObject ��Ȱ��ȭ
            activeVolume = null; // Ȱ��ȭ�� ����Ʈ ���μ��� �ʱ�ȭ
        }
    }
}

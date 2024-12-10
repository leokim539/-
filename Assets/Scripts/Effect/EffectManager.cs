using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.PostProcessing; // PostProcessVolume ��� �� �ʿ�

public class EffectManager : MonoBehaviour
{
    private float effectDuration = 40f; // ȿ�� ���� �ð�
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

        // �ڽ� ������Ʈ �� Ư�� ������Ʈ�� ã�Ƽ� Ȱ��ȭ
        PostProcessVolume[] childVolumes = postProcessObject.GetComponentsInChildren<PostProcessVolume>();
        if (childVolumes.Length > 0)
        {
            // ù ��° �ڽ� ������Ʈ�� Ȱ��ȭ
            activeVolume = childVolumes[0].gameObject;
            activeVolume.SetActive(true); // ȿ�� ��� Ȱ��ȭ
            effectCoroutine = StartCoroutine(ActivateEffectTemporarily(activeVolume));
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

    // ��� �ڽ� ������Ʈ ��Ȱ��ȭ
    public void DeactivateAllChildObjects()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf) // Ȱ��ȭ�� �ڽ� ������Ʈ�� ��Ȱ��ȭ
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}

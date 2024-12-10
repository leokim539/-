using UnityEngine;

public class EffectTrash : MonoBehaviour
{
    public GameObject postProcessObject; // PostProcessVolume ��� GameObject�� ����
    private EffectManager effectManager;

    void Start()
    {
        // EffectManager ã�Ƽ� ����
        effectManager = FindObjectOfType<EffectManager>();
    }

    // Trash2 ��ũ��Ʈ�� CollectItem �ڷ�ƾ�� ����� �� ȣ��� �޼���
    public void TriggerPostProcessEffect()
    {
        if (effectManager != null && postProcessObject != null)
        {
            // postProcessObject�� ���� ����
            effectManager.ActivatePostProcessEffect(postProcessObject);
        }
    }
}

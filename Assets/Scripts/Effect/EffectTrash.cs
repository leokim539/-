using UnityEngine;

public class EffectTrash : MonoBehaviour
{
    public GameObject postProcessObject; // PostProcessVolume 대신 GameObject로 변경
    private EffectManager effectManager;

    void Start()
    {
        // EffectManager 찾아서 참조
        effectManager = FindObjectOfType<EffectManager>();
    }

    // Trash2 스크립트의 CollectItem 코루틴이 실행될 때 호출될 메서드
    public void TriggerPostProcessEffect()
    {
        if (effectManager != null && postProcessObject != null)
        {
            // postProcessObject의 정보 전달
            effectManager.ActivatePostProcessEffect(postProcessObject);
        }
    }
}

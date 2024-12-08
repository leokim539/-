using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCan : MonoBehaviour
{
    private bool TrashCanInRange = false;
    public GameObject Manager;
    private TrashManager trashManager;
    private EffectManager effectManager;
    private TaskUIManager taskUIManager;

    public void Start()
    {
        Manager = GameObject.Find("Manager");
        trashManager = Manager.GetComponent<TrashManager>();
        taskUIManager = Manager.GetComponent<TaskUIManager>(); // TaskUIManager 가져오기

        GameObject postProcessManager = GameObject.Find("PosProcessManager");
        effectManager = postProcessManager.GetComponent<EffectManager>();

        if (effectManager == null)
        {
            Debug.LogError("EffectManager를 찾을 수 없습니다!");
        }
    }

    void Update()
    {
        if (TrashCanInRange && Input.GetKeyDown(KeyCode.F))
        {
            // 쓰레기통과 상호작용 시
            trashManager.scary = 0;
            trashManager.UpdateScaryBar(); // 공포 이미지 관리
            trashManager.isEffectActive = false;

            // PostProcessVolume 비활성화
            effectManager.DeactivateAllChildObjects(); // 추가된 비활성화 기능 호출

            // 저장된 카운트를 실제 카운트로 증가시키기
            taskUIManager.ConfirmCollection();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            TrashCanInRange = true;
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            TrashCanInRange = false;
    }
}

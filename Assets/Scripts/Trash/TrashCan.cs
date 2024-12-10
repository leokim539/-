using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCan : MonoBehaviour
{
    private bool TrashCanInRange = false; // 플레이어가 쓰레기통에 접근했는지 여부
    public GameObject Manager; // 매니저 오브젝트
    private TrashManager trashManager; // TrashManager 컴포넌트
    private EffectManager effectManager; // 효과 매니저
    private TaskUIManager taskUIManager; // UI 매니저

    public void Start()
    {
        Manager = GameObject.Find("Manager"); // 매니저 오브젝트 찾기
        trashManager = Manager.GetComponent<TrashManager>(); // TrashManager 가져오기
        taskUIManager = Manager.GetComponent<TaskUIManager>(); // TaskUIManager 가져오기

        GameObject postProcessManager = GameObject.Find("PosProcessManager");
        effectManager = postProcessManager.GetComponent<EffectManager>(); // EffectManager 가져오기

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
            trashManager.scary = 0; // 공포치 초기화
            trashManager.UpdateScaryBar(); // 공포 이미지 관리
            trashManager.isEffectActive = false; // 효과 비활성화

            // PostProcessVolume 비활성화
            effectManager.DeactivateAllChildObjects(); // 추가된 비활성화 기능 호출

            // 저장된 카운트를 실제 카운트로 증가시키기
            taskUIManager.ConfirmCollection(); // UI 매니저의 ConfirmCollection 호출
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            TrashCanInRange = true; // 플레이어가 쓰레기통에 접근했음을 기록
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            TrashCanInRange = false; // 플레이어가 쓰레기통에서 나갔음을 기록
    }
}

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
        taskUIManager = Manager.GetComponent<TaskUIManager>(); // TaskUIManager ��������

        GameObject postProcessManager = GameObject.Find("PosProcessManager");
        effectManager = postProcessManager.GetComponent<EffectManager>();

        if (effectManager == null)
        {
            Debug.LogError("EffectManager�� ã�� �� �����ϴ�!");
        }
    }

    void Update()
    {
        if (TrashCanInRange && Input.GetKeyDown(KeyCode.F))
        {
            // ��������� ��ȣ�ۿ� ��
            trashManager.scary = 0;
            trashManager.UpdateScaryBar(); // ���� �̹��� ����
            trashManager.isEffectActive = false;

            // PostProcessVolume ��Ȱ��ȭ
            effectManager.DeactivateAllChildObjects(); // �߰��� ��Ȱ��ȭ ��� ȣ��

            // ����� ī��Ʈ�� ���� ī��Ʈ�� ������Ű��
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

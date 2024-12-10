using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCan : MonoBehaviour
{
    private bool TrashCanInRange = false; // �÷��̾ �������뿡 �����ߴ��� ����
    public GameObject Manager; // �Ŵ��� ������Ʈ
    private TrashManager trashManager; // TrashManager ������Ʈ
    private EffectManager effectManager; // ȿ�� �Ŵ���
    private TaskUIManager taskUIManager; // UI �Ŵ���

    public void Start()
    {
        Manager = GameObject.Find("Manager"); // �Ŵ��� ������Ʈ ã��
        trashManager = Manager.GetComponent<TrashManager>(); // TrashManager ��������
        taskUIManager = Manager.GetComponent<TaskUIManager>(); // TaskUIManager ��������

        GameObject postProcessManager = GameObject.Find("PosProcessManager");
        effectManager = postProcessManager.GetComponent<EffectManager>(); // EffectManager ��������

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
            trashManager.scary = 0; // ����ġ �ʱ�ȭ
            trashManager.UpdateScaryBar(); // ���� �̹��� ����
            trashManager.isEffectActive = false; // ȿ�� ��Ȱ��ȭ

            // PostProcessVolume ��Ȱ��ȭ
            effectManager.DeactivateAllChildObjects(); // �߰��� ��Ȱ��ȭ ��� ȣ��

            // ����� ī��Ʈ�� ���� ī��Ʈ�� ������Ű��
            taskUIManager.ConfirmCollection(); // UI �Ŵ����� ConfirmCollection ȣ��
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            TrashCanInRange = true; // �÷��̾ �������뿡 ���������� ���
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            TrashCanInRange = false; // �÷��̾ �������뿡�� �������� ���
    }
}

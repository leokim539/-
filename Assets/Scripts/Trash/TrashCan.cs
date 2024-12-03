using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCan : MonoBehaviour
{
    private bool TrashCanInRange = false;
    public GameObject Manager;
    private TrashManager trashManager;
    private EffectManager effectManager; // EffectManager ���� �߰�

    public void Start()
    {
        Manager = GameObject.Find("Manager");
        trashManager = Manager.GetComponent<TrashManager>();


        GameObject postProcessManager = GameObject.Find("PostProcessManager");
        effectManager = postProcessManager.GetComponent<EffectManager>();  // EffectManager ã��

        // Debug �α� �߰�
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

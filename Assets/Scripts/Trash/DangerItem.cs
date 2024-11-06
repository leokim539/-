using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerItem : MonoBehaviour
{
    public float slowedSpeedMultiplier = 0.5f; //�����ٰ�
    public float slowDuration = 0.5f; //�����ٰ�
    public int Trashscary;//������ ������ �þ�� ����

    private bool TrashInRange = false;

    private GameObject Manager;
    private TrashManager trashManager;

    public void Start()
    {
        Manager = GameObject.Find("Manager");
        trashManager = Manager.GetComponent<TrashManager>();
    }
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && TrashInRange)
        {
            if (trashManager.scary + Trashscary >= 100)
            {
                return;
            }
            trashManager.scary += Trashscary;
            trashManager.UpdateScaryBar(); //���� �̹��� ����
            trashManager.isEffectActive = true;
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            TrashInRange = true;
        
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            TrashInRange = false;
    }

}
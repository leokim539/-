using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{
    public int Trashscary;//������ ������ �þ�� ����
    private bool TrashInRange = false;
    public GameObject Manager;
    private TrashManager trashManager;
    void Start()
    {
        Manager = GameObject.Find("Manager");
        trashManager = Manager.GetComponent<TrashManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && TrashInRange)
        {
            if (trashManager.scary + Trashscary >= 100)
            {
                return;
            }
            trashManager.scary += Trashscary;
            trashManager.UpdateScaryBar(); //���� �̹��� ����
            Destroy(gameObject);
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TrashInRange = true;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TrashInRange = false;
        }
    }
}

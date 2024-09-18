using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{
    public int Trashscary;//쓰레기 먹으면 늘어나는 공포
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
            trashManager.UpdateScaryBar(); //공포 이미지 관리
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

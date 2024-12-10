using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerItem : MonoBehaviour
{
    public float slowedSpeedMultiplier = 0.5f; //보내줄값
    public float slowDuration = 0.5f; //보내줄값
    public int Trashscary;//쓰레기 먹으면 늘어나는 공포

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
            trashManager.UpdateScaryBar(); //공포 이미지 관리
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
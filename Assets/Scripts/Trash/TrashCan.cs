using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCan : MonoBehaviour
{
    private bool TrashCanInRange = false;
    public GameObject Manager;
    private TrashManager trashManager;

    public void Start()
    {
        Manager = GameObject.Find("Manager");
        trashManager = Manager.GetComponent<TrashManager>();
    }
    void Update()
    {
        if (TrashCanInRange && Input.GetKeyDown(KeyCode.F))
        {
            trashManager.scary = 0;
            trashManager.UpdateScaryBar(); //공포 이미지 관리
            trashManager.isEffectActive = false;
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            TrashCanInRange = true;
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            TrashCanInRange = false;
    }
}

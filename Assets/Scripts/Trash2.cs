using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash2 : MonoBehaviour
{
    [Header("상호작용거리")]
    public float interactDistance = 1.5f; // 플레이어와 오브젝트 간의 최대 상호작용 거리
    [Header("쓰레기 먹으면 늘어나는양")]
    public int Trashscary;//쓰레기 먹으면 늘어나는 공포
    private bool TrashInRange = false;
    private GameObject Manager;
    private TrashManager trashManager;
    private TaskUIManager taskUIManager;
    [Header("쓰레기 이름")]
    public string trash1;
    public string trash2;
    public string trash3;

    public Camera camera;

    void Start()
    {
        Manager = GameObject.Find("Manager");
        trashManager = Manager.GetComponent<TrashManager>();
        taskUIManager = Manager.GetComponent<TaskUIManager>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            InteractWithObject();
        }
    }

    void InteractWithObject()
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider != null && hit.collider.CompareTag("Trash"))
            {
                if (trashManager.scary + Trashscary >= 100)
                {
                    return;
                }
                string objectName = hit.collider.gameObject.name;
                hit.collider.gameObject.SetActive(false);

                trashManager.scary += Trashscary;
                trashManager.UpdateScaryBar(); //공포 이미지 관리
                if (objectName.Contains(trash1))
                {
                    taskUIManager.UpdateCircleCount();
                }
                else if (objectName.Contains(trash2))
                {
                    taskUIManager.UpdateCylinderCount();
                }
                else if (objectName.Contains(trash3))
                {
                    taskUIManager.UpdateSquareCount();
                }
            }

            else if (hit.collider != null && hit.collider.CompareTag("DangerTrash"))
            {
                if (trashManager.scary + Trashscary >= 100)
                {
                    return;
                }
                string objectName = hit.collider.gameObject.name;
                hit.collider.gameObject.SetActive(false);
                trashManager.scary += Trashscary;
                trashManager.UpdateScaryBar(); //공포 이미지 관리
                trashManager.isEffectActive = true; //슬로우
                if (objectName.Contains(trash1))
                {
                    taskUIManager.UpdateCircleCount();
                }
                else if (objectName.Contains(trash2))
                {
                    taskUIManager.UpdateCylinderCount();
                }
                else if (objectName.Contains(trash3))
                {
                    taskUIManager.UpdateSquareCount();
                }
                else
                    return;
            }
            else if (hit.collider != null && hit.collider.CompareTag("TrashCan"))
            {
                trashManager.scary = 0;
                trashManager.UpdateScaryBar(); //공포 이미지 관리
                trashManager.isEffectActive = false; //슬로우 풀기
            }
        }
    }
}

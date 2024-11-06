using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trash : MonoBehaviour
{
    [Header("????????")]
    public float interactDistance = 1.5f; // ?б└????? ??????? ???? ??? ?????? ???
    [Header("?????? ?????? ?йн??и·?")]
    public int Trashscary;//?????? ?????? ?йн??? ????
    private bool TrashInRange = false;
    private GameObject Manager;
    private TrashManager trashManager;
    private TaskUIManager taskUIManager;
    [Header("?????? ???")]
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
                trashManager.UpdateScaryBar(); //???? ????? ????
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
                trashManager.UpdateScaryBar(); //???? ????? ????
                trashManager.isEffectActive = true; //???ея?
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
                trashManager.UpdateScaryBar(); //???? ????? ????
                trashManager.isEffectActive = false; //???ея? ???
            }
        }
    }
}

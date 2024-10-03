using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI�� ����ϱ� ���� ���ӽ����̽�

public class Trash2 : MonoBehaviour
{
    [Header("��ȣ�ۿ� �Ÿ�")]
    public float interactDistance = 1.5f; // �÷��̾�� ������Ʈ ���� �ִ� ��ȣ�ۿ� �Ÿ�

    [Header("������ ������ �þ�� ��")]
    public int Trashscary; // ������ ������ �����ϴ� ����ġ

    private GameObject Manager;
    private TrashManager trashManager;
    private TaskUIManager taskUIManager;

    [Header("������ �̸�")]
    public string trash1;
    public string trash2;
    public string trash3;

    public Camera camera;

    [Header("UI ����")]
    public GameObject interactUI; // FŰ UI
    public Slider progressBar; // ���� ������ �����̴�
    public float maxHoldTime = 2f; // �ִ� Ȧ�� �ð�
    private float currentHoldTime = 0f; // ���� ������ �ִ� �ð�
    private bool isHolding = false; // FŰ�� ������ �ִ��� ����

    private GameObject currentTrash; // ���� ��ȣ�ۿ� ���� ������

    void Start()
    {
        Manager = GameObject.Find("Manager");
        trashManager = Manager.GetComponent<TrashManager>();
        taskUIManager = Manager.GetComponent<TaskUIManager>();

        interactUI.SetActive(false); // ó���� UI�� ����ϴ�.
        progressBar.maxValue = 2; // �����̴��� �ִ� �� ����
        progressBar.value = 0; // �����̴� �ʱ�ȭ
    }

    void Update()
    {
        CheckForObject();

        if (isHolding)
        {
            // FŰ�� ���� ���¿��� �����̴� ���� ������Ʈ�մϴ�.
            if (Input.GetKey(KeyCode.F))
            {
                currentHoldTime += Time.deltaTime; // �ʴ� 1�� ����

                // �����̴� �� ������Ʈ
                progressBar.value = currentHoldTime;

                // �����̴� ���� �ִ밪�� �������� ��
                if (currentHoldTime >= maxHoldTime)
                {
                    ConsumeTrash();
                }
            }
            else
            {
                // FŰ�� ���� �ʱ�ȭ
                ResetHold();
            }
        }
    }

    void CheckForObject()
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider != null && (hit.collider.CompareTag("Trash") || hit.collider.CompareTag("DangerTrash")))
            {
                ShowUI(hit.collider.gameObject);
            }
            else
            {
                HideUI();
            }
        }
        else
        {
            HideUI();
        }
    }

    void ShowUI(GameObject trashObject)
    {
        currentTrash = trashObject; // ��ȣ�ۿ��� ������Ʈ ����
        interactUI.SetActive(true); // UI ǥ��
        progressBar.gameObject.SetActive(true); // ���� �� ǥ��
        isHolding = true; // FŰ�� ������ ���·� ����
    }

    void HideUI()
    {
        interactUI.SetActive(false); // UI ����
        progressBar.gameObject.SetActive(false); // ���� �� ����
        ResetHold(); // FŰ ������ ���� �ʱ�ȭ
    }

    void ResetHold()
    {
        currentHoldTime = 0f;
        progressBar.value = 0f; // �����̴� �� �ʱ�ȭ
        isHolding = false;
    }

    void ConsumeTrash()
    {
        if (trashManager.scary + Trashscary >= 100)
        {
            return; // ����ġ�� 100 �̻��̸� �Һ����� ����
        }

        string objectName = currentTrash.name;
        currentTrash.SetActive(false); // ������ ������Ʈ ��Ȱ��ȭ

        trashManager.scary += Trashscary;
        trashManager.UpdateScaryBar(); // ����ġ UI ������Ʈ

        // ������ ������ ���� UI ������Ʈ
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

        // �ʱ�ȭ
        HideUI();
    }
}
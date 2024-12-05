using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI�� ����ϱ� ���� ���ӽ����̽�
using Photon.Pun;

public class Trash2 : MonoBehaviourPunCallbacks
{
    [Header("��ȣ�ۿ� �Ÿ�")]
    public float interactDistance = 3f; // �÷��̾�� ������Ʈ ���� �ִ� ��ȣ�ۿ� �Ÿ�


    [Header("������ ������ �þ�� ��")]
    public int Trashscary; // ������ ������ �����ϴ� ����ġ

    private GameObject Manager;
    private TrashManager trashManager;
    private TaskUIManager taskUIManager;

    [Header("������ �̸�")]
    public string trash1;
    public string trash2;
    public string trash3;

    public Camera ca;

    [Header("UI ����")]
    public GameObject interactUI; // FŰ UI
    public Slider progressBar; // ���� ������ �����̴�
    public float maxHoldTime = 2f; // �ִ� Ȧ�� �ð�
    private float currentHoldTime = 0f; // ���� ������ �ִ� �ð�
    private bool isHolding = false; // FŰ�� ������ �ִ��� ����
    private bool isDangerHolding = false; // FŰ�� ������ �ִ��� ����

    private GameObject currentTrash; // ���� ��ȣ�ۿ� ���� ������

    private EffectTrash effectTrash;

    [Header("�÷��̾� ����")]
    public Transform playerTransform; // �÷��̾��� Transform�� �巡���Ͽ� �Ҵ�

    void Awake()
    {
        //interactUI = GameObject.Find("F");
        GameObject sliderObject = GameObject.Find("Slider");
        if (sliderObject != null)
        {
            progressBar = sliderObject.GetComponent<Slider>();
        }
        Debug.Log("Awake");
    }
    void Start()
    {
        interactUI = GameObject.Find("F"); // Start���� ã��
        if (interactUI == null)
        {
            Debug.LogError("Interact UI not found!");
        }
        Manager = GameObject.Find("Manager");
        trashManager = Manager.GetComponent<TrashManager>();
        taskUIManager = Manager.GetComponent<TaskUIManager>();

        //progressBar.maxValue = maxHoldTime; // �����̴��� �ִ� �� ����
        //progressBar.value = 0; // �����̴� �ʱ�ȭ

        ca = GetComponentInChildren<Camera>();

        effectTrash = FindObjectOfType<EffectTrash>();
  
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            CheckForObject();
        }
        if (isDangerHolding)
        {
            HandleHolding();
        }
        if (isHolding)
        {
            HandleHolding();
        }
    }

    void HandleHolding()
    {
        if (Input.GetKey(KeyCode.F))
        {
            currentHoldTime += Time.deltaTime; // �ʴ� 1�� ����

            // �����̴� �� ������Ʈ
            progressBar.value = currentHoldTime;

            // �����̴� ���� �ִ밪�� �������� ��
            if (currentHoldTime >= maxHoldTime)
            {
                if (isDangerHolding)
                {
                    ConsumeDangerTrash();
                }
                if(isHolding)
                {
                    ConsumeTrash();
                }
            }
        }
        else
        {
            // FŰ�� ���� �ʱ�ȭ
            ResetHold();
        }
    }

    void CheckForObject()
    {
        RaycastHit hit;
        Ray ray = ca.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider.CompareTag("Trash") || hit.collider.CompareTag("GroundTrash"))
            {
                ShowUITrash(hit.collider.gameObject, false);
            }
            else if (hit.collider.CompareTag("DangerTrash") )
            {
                ShowUITrash(hit.collider.gameObject, true);
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
    void ShowUITrash(GameObject trashObject, bool isDanger)
    {
        currentTrash = trashObject; // ��ȣ�ۿ��� ������Ʈ ����
        interactUI.SetActive(true); // UI ǥ��
        progressBar.gameObject.SetActive(true); // ���� �� ǥ��
        if (isDanger)
        {
            isDangerHolding = true; // ���� ������ ���� ����
        }
        else
        {
            isHolding = true; // �Ϲ� ������ ���� ����
        }
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
        isDangerHolding = false; // ���� ������ �ʱ�ȭ
    }

    void ConsumeTrash()
    {
        if (trashManager.scary + Trashscary >= 100)
        {
            return; // ����ġ�� 100 �̻��̸� �Һ����� ����
        }

        string objectName = currentTrash.name;
        StartCoroutine(CollectItem(currentTrash)); // ������ ���� ȿ�� ����
        trashManager.scary += Trashscary;
        trashManager.UpdateScaryBar(); // ����ġ UI ������Ʈ

        if (currentTrash.CompareTag("GroundTrash"))
        {
            FirstPersonController firstPersonController = FindObjectOfType<FirstPersonController>();
            if (firstPersonController != null)
            {
                firstPersonController.PickingUp(); // PickingUp �޼��� ȣ��
            }
        }
        // ������ ������ ���� UI ������Ʈ
        UpdateTaskUI(objectName);

        // �ʱ�ȭ
        HideUI();

        photonView.RPC("UpdateTaskUI", RpcTarget.Others, objectName);

        //photonView.RPC("RPC_CollectItem", RpcTarget.Others, currentTrash.GetPhotonView().ViewID);
    }

    void ConsumeDangerTrash()
    {
        if (trashManager.scary + Trashscary >= 100)
        {
            return; // ����ġ�� 100 �̻��̸� �Һ����� ����
        }

        string objectName = currentTrash.name;
        StartCoroutine(CollectItem(currentTrash)); // ������ ���� ȿ�� ����
        trashManager.scary += Trashscary;
        trashManager.UpdateScaryBar(); // ����ġ UI ������Ʈ
        trashManager.isEffectActive = true;
        

        // ������ ������ ���� UI ������Ʈ
        UpdateTaskUI(objectName);
        
        // �ʱ�ȭ  
        HideUI();

        photonView.RPC("UpdateTaskUI", RpcTarget.Others, objectName);

        //photonView.RPC("RPC_CollectItem", RpcTarget.Others, objectName);
    }

    [PunRPC]
    void UpdateTaskUI(string objectName)
    {
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
        else return;
    }
    [PunRPC]
    public void RPC_CollectItem(string itemName)
    {
        GameObject item = GameObject.Find(itemName); // �̸����� ������ ã��
        if (item != null)
        {
            StartCoroutine(CollectItem(item));
        }
    }
    public IEnumerator CollectItem(GameObject item)
    {
        // �ݶ��̴� �������� �� ��Ȱ��ȭ
        Collider itemCollider = item.GetComponent<Collider>();
        if (itemCollider != null)
        {
            itemCollider.enabled = false; // �ݶ��̴� ��Ȱ��ȭ
        }

        Vector3 originalScale = item.transform.localScale;
        Vector3 targetScale = Vector3.zero; // ���� ũ��
        float duration = 1f; // �̵� �� ��� �ð�
        float elapsedTime = 0f;

        // EffectTrash ������Ʈ Ȯ��
        EffectTrash itemEffectTrash = item.GetComponent<EffectTrash>();

        Vector3 myPosition = transform.position;

        while (elapsedTime < duration)
        {
            // �÷��̾� �������� �̵�
            Vector3 dir = myPosition - item.transform.position;
            dir.y = 0; // Y���� 0���� �����Ͽ� ���� �̵��� �ϵ��� ��
            dir.Normalize(); // ���� ���� ����ȭ

            // �������� �÷��̾� ������ �̵�
            item.transform.position += dir * (3f * Time.deltaTime); // �ӵ� ����

            // ũ�� ���̱�
            item.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / (duration / 3));
            elapsedTime += Time.deltaTime;

            // PostProcess ȿ���� EffectTrash ������Ʈ�� �ִ� ��쿡�� Ʈ����
            if (itemEffectTrash != null)
            {
                itemEffectTrash.TriggerPostProcessEffect();
            }

            yield return null;
        }

        // ������ ��Ȱ��ȭ
        item.SetActive(false); // ������ ��Ȱ��ȭ
    }
}

    /*
        void Update()
        {
            CheckForObject();

            if (isDangerHolding )
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
                        ConsumeDangerTrash();
                    }
                }
                else
                {
                    // FŰ�� ���� �ʱ�ȭ
                    ResetHold();
                }
            }
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
            if (isCanHolding)
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
                        ConsumeTrashCan();
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
                if (hit.collider != null && hit.collider.CompareTag("Trash"))
                {
                    ShowUITrash(hit.collider.gameObject);
                }
                else if (hit.collider != null && hit.collider.CompareTag("DangerTrash"))
                {
                    ShowUIDangerTrash(hit.collider.gameObject);
                }
                else if (hit.collider != null && hit.collider.CompareTag("TrashCan"))
                {
                    ShowUITrashCan(hit.collider.gameObject);
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

        void ShowUITrash(GameObject trashObject)
        {
            currentTrash = trashObject; // ��ȣ�ۿ��� ������Ʈ ����
            interactUI.SetActive(true); // UI ǥ��
            progressBar.gameObject.SetActive(true); // ���� �� ǥ��
            isHolding = true; // FŰ�� ������ ���·� ����
        }
        void ShowUIDangerTrash(GameObject trashObject)
        {
            currentTrash = trashObject; // ��ȣ�ۿ��� ������Ʈ ����
            interactUI.SetActive(true); // UI ǥ��
            progressBar.gameObject.SetActive(true); // ���� �� ǥ��
            isDangerHolding = true; // FŰ�� ������ ���·� ����
        }
        void ShowUITrashCan(GameObject trashObject)
        {
            currentTrash = trashObject; // ��ȣ�ۿ��� ������Ʈ ����
            interactUI.SetActive(true); // UI ǥ��
            progressBar.gameObject.SetActive(true); // ���� �� ǥ��
            isCanHolding = true; // FŰ�� ������ ���·� ����
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
        void ConsumeDangerTrash()
        {
            if (trashManager.scary + Trashscary >= 100)
            {
                return; // ����ġ�� 100 �̻��̸� �Һ����� ����
            }

            string objectName = currentTrash.name;
            currentTrash.SetActive(false); // ������ ������Ʈ ��Ȱ��ȭ

            trashManager.scary += Trashscary;
            trashManager.UpdateScaryBar(); // ����ġ UI ������Ʈ
            trashManager.isEffectActive = true;

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
        void ConsumeTrashCan()
        {
            trashManager.scary = 0;
            trashManager.UpdateScaryBar(); //���� �̹��� ����
            trashManager.isEffectActive = false;
        }*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI�� ����ϱ� ���� ���ӽ����̽�
using Photon.Pun;

public class Trash2 : MonoBehaviourPunCallbacks
{
    [Header("��ȣ�ۿ� �Ÿ�")]
    public float interactDistance = 5f; // �÷��̾�� ������Ʈ ���� �ִ� ��ȣ�ۿ� �Ÿ�
    
    [Header("������ ������ �þ�� ��")]
    public int Trashscary; // ������ ������ �����ϴ� ����ġ

    private GameObject Manager;
    private TrashManager trashManager;
    private TaskUIManager taskUIManager;

    [Header("������ �̸�")]
    public string trash1;
    public string trash2;
    public string trash3;
    public string trash4;
    public string trash5;
    public string trash6;

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
    [Header("���� ������")]
    public Image ItemImage; // �����۸� ǥ���� UI Image
    public Sprite[] ItemSprites; // �ֹ� �̸��� ���� ����� ��������Ʈ �迭
    public string[] ItemNames; // �ֹ� �̸� �迭
    private bool itemCanUse = false;
    private string currentItem; // ���� ȹ���� ������ �̸�
    [Header("�þ� ������ ������")]
    public GameObject canvasPrefab; // �þ߸� ���� Canvas ������
    public float effectDuration = 5f; // ȿ�� ���� �ð�
    void Awake()
    {
        //interactUI = GameObject.Find("F");
        GameObject sliderObject = GameObject.Find("Slider");
        if (sliderObject != null)
        {
            progressBar = sliderObject.GetComponent<Slider>();
        }
        GameObject itemImage = GameObject.Find("RandomItem");
        if (itemImage != null)
        {
            ItemImage = itemImage.GetComponent<Image>();
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
        if (itemCanUse && Input.GetKeyDown(KeyCode.E))
        {
            ItemUse(currentItem);
            currentItem = null; // ���� ������ �ʱ�ȭ
            ItemImage.sprite = null; // UI �̹��� �ʱ�ȭ
            itemCanUse = false;
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

        //photonView.RPC("UpdateTaskUI", RpcTarget.Others, objectName);

        //photonView.RPC("RPC_CollectItem", RpcTarget.Others, currentTrash.GetPhotonView().ViewID);
    }
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
        else if (objectName.Contains(trash4))
        {
            taskUIManager.UpdateBeerCanCount();
        }
        else if (objectName.Contains(trash5))
        {
            taskUIManager.UpdatePetBottleCount();
        }
        else if (objectName.Contains(trash6))
        {
            taskUIManager.UpdateTrashBagCount();
        }
        else return;
    }
    void ConsumeDangerTrash()
    {
        int randomIndex = Random.Range(0, ItemNames.Length); // ���� �ε��� ����
        string selectedOrderName = ItemNames[randomIndex]; // �������� ���õ� �ֹ� �̸�
        int index = System.Array.IndexOf(ItemNames, selectedOrderName); // �ֹ� �̸��� �ε��� ã��

        StartCoroutine(CollectItem(currentTrash));

        if (index >= 0 && index < ItemSprites.Length)
        {
            ItemImage.sprite = ItemSprites[index]; // �ش� �ε����� ��������Ʈ�� �̹��� ����
            Debug.LogWarning(currentItem);
            itemCanUse = true;
        } // ���õ� �ֹ� �̸��� ���� �̹��� ������Ʈ

        HideUI();
        /*if (trashManager.scary + Trashscary >= 100)
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

        photonView.RPC("RPC_CollectItem", RpcTarget.Others, objectName);*/
    }
    void ItemUse(string item)
    {
        switch(item)
        {
            case "�þ߰�����"://��� �þ� 5�ʰ� �Ⱥ���
                if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
                {
                    StartCoroutine(ApplyObscuringEffect());
                }
                break;
            case "��������"://��� 5�ʰ� ��������
                if (PhotonNetwork.IsConnected)
                {
                    // ��� �÷��̾ ��ȸ�Ͽ� �ڽ��� �ƴ� �÷��̾��� PhotonView�� ã�� RPC ȣ��
                    foreach (var player in PhotonNetwork.PlayerList)
                    {
                        if (player != PhotonNetwork.LocalPlayer) // �ڽ��� �ƴ� �÷��̾�
                        {
                            // ���� �÷��̾��� PhotonView�� ã��
                            GameObject playerObject = GameObject.Find("Player(Clone)"); // �÷��̾� ������Ʈ �̸� ��Ģ�� ���� ã��
                            if (playerObject != null)
                            {
                                PhotonView targetPhotonView = playerObject.GetComponent<PhotonView>();
                                if (targetPhotonView != null)
                                {
                                    targetPhotonView.RPC("DisableMovement", RpcTarget.All, 5f); // 5�� ���� �̵� ��Ȱ��ȭ
                                }
                            }
                        }
                    }
                }
                break;
            default:
                break;
        }
    }
    
    private IEnumerator ApplyObscuringEffect()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player != gameObject) // �ڽ��� �ƴ� ���
            {
                GameObject canvasInstance = PhotonNetwork.Instantiate(canvasPrefab.name, player.transform.position, Quaternion.identity, 0);
                canvasInstance.transform.SetParent(player.transform); // ���� ������Ʈ�� �ڽ����� ����

                RectTransform rectTransform = canvasInstance.GetComponent<RectTransform>();
                rectTransform.localPosition = Vector3.zero;
                rectTransform.localScale = new Vector3(1, 1, 1);

                yield return new WaitForSeconds(effectDuration);

                PhotonNetwork.Destroy(canvasInstance);
            }
        }
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

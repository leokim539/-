using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI�� ����ϱ� ���� ���ӽ����̽�
using Photon.Pun;
using Unity.VisualScripting;

public class Trash2 : MonoBehaviourPunCallbacks
{
    [Header("��ȣ�ۿ� �Ÿ�")]
    public float interactDistance = 5f; // �÷��̾�� ������Ʈ ���� �ִ� ��ȣ�ۿ� �Ÿ�
    
    [Header("������ ������ �þ�� ��")]
    public int Trashscary; // ������ ������ �����ϴ� ����ġ

    private GameObject Manager;
    private TrashManager trashManager;
    private TaskUIManager taskUIManager;
    private FirstPersonController firstPersonController;
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
    private string item; // ���� ȹ���� ������ �̸�
    [Header("�þ� ������ ������")]
    public GameObject canvasPrefab; // �þ߸� ���� Canvas ������
    public float effectDuration = 5f; // ȿ�� ���� �ð�
    [Header("�������� ��ġ���� ������")]
    public string trashCanSpawn = "TrashCanSpawn";
    [Header("�����")]
    public AudioClip soundEffect;
    private AudioSource audioSource;
    [Header("�ٳ���")]
    public GameObject baNaNaPrefab;
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
        firstPersonController = GetComponent<FirstPersonController>();
        //progressBar.maxValue = maxHoldTime; // �����̴��� �ִ� �� ����
        //progressBar.value = 0; // �����̴� �ʱ�ȭ

        ca = GetComponentInChildren<Camera>();

        effectTrash = FindObjectOfType<EffectTrash>();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = soundEffect;
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
            else if (hit.collider.CompareTag("UseItem"))
            {
                ShowUITrash(hit.collider.gameObject, true);
            }
            else if (hit.collider.CompareTag("Item"))
            {
                item = hit.collider.gameObject.name;
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
            taskUIManager.StoreCircleCount(); // ī��Ʈ ����
        }
        else if (objectName.Contains(trash2))
        {
            taskUIManager.StoreCylinderCount(); // ī��Ʈ ����
        }
        else if (objectName.Contains(trash3))
        {
            taskUIManager.StoreSquareCount(); // ī��Ʈ ����
        }
        else if (objectName.Contains(trash4))
        {
            taskUIManager.StoreBeerCanCount(); // ī��Ʈ ����
        }
        else if (objectName.Contains(trash5))
        {
            taskUIManager.StorePetBottleCount(); // ī��Ʈ ����
        }
        else if (objectName.Contains(trash6))
        {
            taskUIManager.StoreTrashBagCount(); // ī��Ʈ ����
        }
    }
    void ConsumeDangerTrash()
    {
        currentItem = null; // ���� ������ �ʱ�ȭ
        ItemImage.sprite = null; // UI �̹��� �ʱ�ȭ
        if (currentTrash.CompareTag("Item"))
        {
            currentItem = item;
            int index = System.Array.IndexOf(ItemNames, currentItem); // ������ �̸��� �ε��� ã��

            StartCoroutine(CollectItem(currentTrash));
            if (index >= 0 && index < ItemSprites.Length)
            {
                ItemImage.sprite = ItemSprites[index]; // �ش� �ε����� ��������Ʈ�� �̹��� ����
                Debug.LogWarning(currentItem);
                itemCanUse = true;
            } // ���õ� �ֹ� �̸��� ���� �̹��� ������Ʈ
        }        
        else if (currentTrash.CompareTag("UseItem"))
        {
            int randomIndex = Random.Range(0, ItemNames.Length); // ���� �ε��� ����
            currentItem = ItemNames[randomIndex]; // �������� ���õ� ������ �̸�
            int index = System.Array.IndexOf(ItemNames, currentItem); // ������ �̸��� �ε��� ã��

            StartCoroutine(CollectItem(currentTrash));
            if (index >= 0 && index < ItemSprites.Length)
            {
                ItemImage.sprite = ItemSprites[index]; // �ش� �ε����� ��������Ʈ�� �̹��� ����
                Debug.LogWarning(currentItem);
                itemCanUse = true;
            } // ���õ� �ֹ� �̸��� ���� �̹��� ������Ʈ
        }
        HideUI();
    }
    void Item()
    {
        
    }
    void ItemUse(string item)
    {
        switch(item)
        {
            case "�ȴ���ô"://��� �þ� 5�ʰ� �Ⱥ���
                if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
                {
                    StartCoroutine(ApplyObscuringEffect());
                }
                else Debug.Log("�ȴ���ô");

                break;

            case "�����"://�������� ��� 5�ʰ� ��������
                if (PhotonNetwork.IsConnected)
                {
                    StartCoroutine(TaserGun());
                }
                else Debug.Log("�����");

                break;

            case "���ض��"://�������� ��ġ ���� �ٲٱ�
                if (PhotonNetwork.IsConnected)
                {
                    photonView.RPC("TrashCanSpawns", RpcTarget.All);
                }
                else Debug.Log("���ض��");

                break;

            case "����"://�˽δ¼Ҹ�(�����)
                if (PhotonNetwork.IsConnected)
                {
                    PlaySoundForLocalPlayer();
                }
                else Debug.Log("����");

                break;

            case "��������"://������
                if (PhotonNetwork.IsConnected)
                {
                    if (photonView.IsMine)
                    {
                        StartCoroutine(SpeedUP());
                    }
                    else Debug.Log("�������");
                }

                break;

            case "���ǹ���"://������
                if (PhotonNetwork.IsConnected)
                {
                    StartCoroutine(SpeedDown());
                }
                else Debug.Log("������������");

                break;

            case "��������"://�ƹ�ȿ������
                Debug.Log("��������");

                break;

            case "����ٳ���"://����ġ
                if (PhotonNetwork.IsConnected)
                {
                    BaNaNa();
                }
                Debug.Log("����ٳ���");

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

    public IEnumerator TaserGun()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player != gameObject) // �ڽ��� �ƴ� �÷��̾�
            {
                firstPersonController.canMove = false;
                yield return new WaitForSeconds(5f);
                firstPersonController.canMove = true;
            }
        }
    }
    [PunRPC]
   
    public void TrashCanSpawns()
    {
        GameObject trashCan = GameObject.Find(trashCanSpawn);
        if (trashCan != null)
        {
            TrashCanSpawner trashCanScript = trashCan.GetComponent<TrashCanSpawner>();
            if (trashCanScript != null)
            {
                // �ڷ�ƾ ����
                StartCoroutine(trashCanScript.MoveTrashCan());
            }
            else
            {
                Debug.LogError("Ÿ�� ������Ʈ�� TrashCanSpawner ��ũ��Ʈ�� �����ϴ�.");
            }
        }
    }
    private IEnumerator SpeedUP()
    {
        if (firstPersonController != null)
        {
            float originalSpeed = firstPersonController.moveSpeed;
            firstPersonController.moveSpeed += 30f;
            yield return new WaitForSeconds(5);

            firstPersonController.moveSpeed = originalSpeed;
        }
        else
        {
            Debug.Log("���� �ӵ��� �Ⱥ����ݾ�!!!!!!!!!!!!!");
        }
    }
    private IEnumerator SpeedDown()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player != gameObject) // �ڽ��� �ƴ� ���
            {
                float originalSpeed = firstPersonController.moveSpeed;
                firstPersonController.moveSpeed -= 3f;
                yield return new WaitForSeconds(5);

                firstPersonController.moveSpeed = originalSpeed;
            }
        }
    }

    public void BaNaNa()
    {
        Vector3 playerPosition = transform.position;
        Vector3 playerForward = transform.forward;

        Vector3 trapPosition = playerPosition - playerForward * 2;

        GameObject trap = Instantiate(baNaNaPrefab, trapPosition, Quaternion.identity);
    }

    [PunRPC]
    void UpdatePosition(Vector3 newPosition)
    {
        transform.position = newPosition; // ��ġ ������Ʈ
    }
    private void PlaySoundForLocalPlayer()
    {
        audioSource.Play();
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
/*public void RPC_CollectItem(string itemName)
    {
        GameObject item = GameObject.Find(itemName); // �̸����� ������ ã��
        if (item != null)
        {
            StartCoroutine(CollectItem(item));
        }
    }*/
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

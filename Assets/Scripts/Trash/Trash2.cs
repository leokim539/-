using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI; // UI�� ����ϱ� ���� ���ӽ����̽�

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
    private GameObject TrashCan;
    private TrashCan trashCan;

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
    public float currentHoldTime = 0f; // ���� ������ �ִ� �ð�
    private GameObject currentTrash; // ���� ��ȣ�ۿ� ���� ������
    private EffectTrash effectTrash;
    private bool _trashCan = false;
    [Header("����")]
    public PlayerSound soundManager;
    [Header("�÷��̾� ����")]
    public Transform playerTransform; // �÷��̾��� Transform�� �巡���Ͽ� �Ҵ�
    [Header("���� ������")]
    public Image ItemImage; // �����۸� ǥ���� UI Image
    public Sprite[] ItemSprites; // �ֹ� �̸��� ���� ����� ��������Ʈ �迭
    public string[] ItemNames; // �ֹ� �̸� �迭
    private bool itemCanUse = false;
    private string currentItem; // ���� ȹ���� ������ �̸�
    private string item; // ���� ȹ���� ������ �̸�
    public GameObject speedUPParticleEffectPrefab;
    [Header("�þ� ������ ������")]
    public GameObject canvasPrefab; // �þ߸� ���� Canvas ������
    public float effectDuration = 5f; // ȿ�� ���� �ð�
    [Header("�������� ��ġ���� ������")]
    public string trashCanSpawn = "TrashCanSpawn";
    [Header("�ٳ���")]
    public GameObject baNaNaPrefab;
    [Header("����ŷ")]
    public bool hacking = true;
    [Header("����Ʈ��")]
    public bool smartPhone = true;
    [Header("�ڵ�ũ��")]
    public bool handCream = true;
    [Header("������")]
    public bool spoon = true;

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

        TrashCan = GameObject.Find("TrashCan");
        trashCan = TrashCan.GetComponent<TrashCan>();
        //firstPersonController = GetComponent<FirstPersonController>();
        //progressBar.maxValue = maxHoldTime; // �����̴��� �ִ� �� ����
        //progressBar.value = 0; // �����̴� �ʱ�ȭ

        ca = GetComponentInChildren<Camera>();

        effectTrash = FindObjectOfType<EffectTrash>();
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            CheckForObject();//Ȯ��

            if (itemCanUse && Input.GetKeyDown(KeyCode.E))
            {
                if (smartPhone)
                {
                    ItemUse(currentItem);
                    currentItem = null; // ���� ������ �ʱ�ȭ
                    ItemImage.sprite = null; // UI �̹��� �ʱ�ȭ
                    itemCanUse = false;
                }
                else if (!smartPhone)
                {
                    currentItem = null; // ���� ������ �ʱ�ȭ
                    ItemImage.sprite = null; // UI �̹��� �ʱ�ȭ
                    itemCanUse = false;
                }
            }
        }
        if (_trashCan)
        {
            if (Input.GetKey(KeyCode.F))
            {
                currentHoldTime += Time.deltaTime; // �ʴ� 1�� ����
                progressBar.value = currentHoldTime;
                if (currentHoldTime >= maxHoldTime)
                {
                    trashCan.TrashCans();
                    ResetHold();
                }
            }
            else
            {
                ResetHold();
            }
        }
    }

    void CheckForObject()
    {
        RaycastHit hit;
        Ray ray = ca.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            interactUI.SetActive(true); // UI ǥ��
            progressBar.gameObject.SetActive(true); // ���� �� ǥ��
            if (handCream)
            {
                if (hit.collider.CompareTag("TrashCan"))
                {
                    _trashCan = true;
                }
                else if (hit.collider.CompareTag("Trash"))
                {
                    if (Input.GetKey(KeyCode.F))
                    {
                        currentTrash = hit.collider.gameObject;
                        if (currentTrash != null) // currentTrash�� null���� Ȯ��
                        {
                            ConsumeTrash(currentTrash); // currentTrash�� ���ڷ� ����
                        }
                    }
                }
                else if (hit.collider.CompareTag("UseItem"))
                {
                    if (Input.GetKey(KeyCode.F))
                    {
                        currentTrash = hit.collider.gameObject;
                        ConsumeDangerTrash(currentTrash);
                    }
                }
                else if (hit.collider.CompareTag("Item"))
                {
                    if (Input.GetKey(KeyCode.F))
                    {
                        item = hit.collider.gameObject.name;
                        currentTrash = hit.collider.gameObject;
                        ConsumeDangerTrash(currentTrash);
                    }
                }
                else
                {
                    HideUI();
                }
            }
        }
        else
        {
            HideUI();
        }
    }

    void HideUI()
    {
        interactUI.SetActive(false); // UI ����
        progressBar.gameObject.SetActive(false); // ���� �� ����
    }

    void ResetHold()
    {
        currentHoldTime = 0f;
        progressBar.value = 0f;
    }

    void ConsumeTrash(GameObject trash)
    {
        if (trash == null) // trash�� null���� Ȯ��
        {
            Debug.LogError("Trash object is null!");
            return; // null�� ��� �޼��� ����
        }

        if (trashManager == null) // trashManager�� null���� Ȯ��
        {
            Debug.LogError("TrashManager is null!");
            return; // null�� ��� �޼��� ����
        }
        if (trashManager.scary + Trashscary >= 100)
        {
            return; // ����ġ�� 100 �̻��̸� �Һ����� ����
        }

        string objectName = trash.name;
        StartCoroutine(CollectItem(currentTrash)); // ������ ���� ȿ�� ����
        trashManager.scary += Trashscary;
        trashManager.UpdateScaryBar(); // ����ġ UI ������Ʈ

        UpdateTaskUI(objectName);

        photonView.RPC("DestroyItem", RpcTarget.Others, trash.GetComponent<PhotonView>().ViewID);
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
    void ConsumeDangerTrash(GameObject trash)
    {
        currentItem = null; // ���� ������ �ʱ�ȭ
        ItemImage.sprite = null; // UI �̹��� �ʱ�ȭ
        if (trash.CompareTag("Item"))
        {
            currentItem = item;
            int index = System.Array.IndexOf(ItemNames, currentItem); // ������ �̸��� �ε��� ã��

            //StartCoroutine(CollectItem(currentTrash));
            if (index >= 0 && index < ItemSprites.Length)
            {
                ItemImage.sprite = ItemSprites[index]; // �ش� �ε����� ��������Ʈ�� �̹��� ����
                Debug.LogWarning(currentItem);
                itemCanUse = true;
            } // ���õ� �ֹ� �̸��� ���� �̹��� ������Ʈ
            PhotonView trashPhotonView = trash.GetComponent<PhotonView>();
            if (trashPhotonView != null)
            {
                Debug.Log(trashPhotonView.ViewID);
                photonView.RPC("DestroyItem", RpcTarget.All, trashPhotonView.ViewID);
            }
            else
            {
                Debug.LogError("���� ��ã��");
            }
        }
        else if (trash.CompareTag("UseItem"))
        {
            int randomIndex = Random.Range(0, ItemNames.Length); // ���� �ε��� ����
            currentItem = ItemNames[randomIndex]; // �������� ���õ� ������ �̸�
            int index = System.Array.IndexOf(ItemNames, currentItem); // ������ �̸��� �ε��� ã��

            //StartCoroutine(CollectItem(currentTrash));
            if (index >= 0 && index < ItemSprites.Length)
            {
                ItemImage.sprite = ItemSprites[index]; // �ش� �ε����� ��������Ʈ�� �̹��� ����
                Debug.LogWarning(currentItem);
                itemCanUse = true;
            } // ���õ� �ֹ� �̸��� ���� �̹��� ������Ʈ
            PhotonView trashPhotonView = trash.GetComponent<PhotonView>();
            if (trashPhotonView != null)
            {
                photonView.RPC("DestroyItem", RpcTarget.All, trashPhotonView.ViewID);
            }
            else
            {
                Debug.LogError("���� ��ã��");
            }
        }
        HideUI();
    }

    public void ItemUse(string item)
    {
        switch (item)
        {
            case "�ȴ���ô"://��� �þ� 5�ʰ� �Ⱥ���
                if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
                {
                    ItemManager.instane.RPCUseSkill0();
                }
                else Debug.Log("�ȴ���ô");
                break;

            case "�����"://�������� ��� 5�ʰ� ��������
                if (PhotonNetwork.IsConnected)
                {
                    ItemManager.instane.RPCUseSkill1();
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
                        photonView.RPC("SpawnParticleEffect", RpcTarget.All, transform.position);
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

            case "����ŷ"://��� 10�ʰ� ������ ���Ұ�
                if (PhotonNetwork.IsConnected)
                {
                    StartCoroutine(Hacking());
                }
                Debug.Log("����ŷ");
                break;

            case "�޴�뾲������"://��� 10�ʰ� ������ ���Ұ�
                if (PhotonNetwork.IsConnected)
                {
                    if (photonView.IsMine)
                    {
                        UseTrashCan();
                    }
                }
                Debug.Log("�޴�뾲������");
                break;

            case "������"://Ƽ��Ҹ�����
                if (PhotonNetwork.IsConnected)
                {
                    if (photonView.IsMine)
                    {
                        TVOn();
                    }
                }
                Debug.Log("������");
                break;

            case "����Ʈ��"://���� ����ϴ� ���� ������ ����ȭ 
                if (PhotonNetwork.IsConnected)
                {
                    SmartPhone();
                }
                Debug.Log("����Ʈ��");
                break;

            case "�ڵ�ũ��":// 5�ʰ� �ƹ��͵� ������ �� ���� ����
                if (PhotonNetwork.IsConnected)
                {
                    StartCoroutine(HandCream());
                }
                Debug.Log("�ڵ�ũ��");
                break;

            case "���º���":// ��� �÷��̾ 5�ʰ� �޸� �� ���� ��
                if (PhotonNetwork.IsConnected)
                {
                    StartCoroutine(Bond());
                }
                Debug.Log("���º���");
                break;

            case "Ŀ��"://5�� ���� ����� �÷��̾��� ���¹̳� �Ҹ���� �޸� �� ����
                if (PhotonNetwork.IsConnected)
                {
                    if (photonView.IsMine)
                        StartCoroutine(Coffee());
                }
                Debug.Log("Ŀ��");
                break;

            case "����"://����� �÷��̾��� ���¹̳ʸ� ��� 0���� ����
                if (PhotonNetwork.IsConnected)
                {
                    if (photonView.IsMine)
                        StartCoroutine(Soy());
                }
                Debug.Log("����");
                break;

            case "������"://����� �÷��̾��� ���¹̳ʸ� ��� 0���� ����
                if (PhotonNetwork.IsConnected)
                {
                    if (photonView.IsMine)
                        StartCoroutine(Spoon());
                }
                Debug.Log("������");
                break;
            default:
                break;
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
    public IEnumerator SpeedUP()
    {
        if (firstPersonController != null)
        {
            float originalSpeed = firstPersonController.moveSpeed;
            float runSpeed = firstPersonController.runSpeed;
            firstPersonController.moveSpeed += 5f;
            firstPersonController.runSpeed += 5f;
            yield return new WaitForSeconds(5);

            firstPersonController.moveSpeed = originalSpeed;
            firstPersonController.runSpeed = runSpeed;
        }
        else
        {
            Debug.Log("���� �ӵ��� �Ⱥ����ݾ�!!!!!!!!!!!!!");
        }
    }
    [PunRPC]
    private void SpawnParticleEffect(Vector3 position)
    {
        // ��ƼŬ ����Ʈ�� ����
        GameObject particleEffect = Instantiate(speedUPParticleEffectPrefab, position, Quaternion.identity);
        Destroy(particleEffect, 2f); // 2�� �Ŀ� ��ƼŬ ����Ʈ ����
    }
    private IEnumerator SpeedDown()
    {
        if (spoon)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                if (player != gameObject) // �ڽ��� �ƴ� ���
                {
                    float originalSpeed = firstPersonController.moveSpeed;
                    float runSpeed = firstPersonController.runSpeed;
                    firstPersonController.moveSpeed -= 3f;
                    firstPersonController.runSpeed -= 3f;
                    yield return new WaitForSeconds(5);

                    firstPersonController.moveSpeed = originalSpeed;
                    firstPersonController.runSpeed = runSpeed;
                }
            }
        }
        else yield return null;
    }

    public void BaNaNa()
    {
        Vector3 playerPosition = transform.position;
        Vector3 playerForward = transform.forward;

        Vector3 trapPosition = playerPosition - playerForward * 2;
        PhotonNetwork.Instantiate(baNaNaPrefab.name, trapPosition, Quaternion.identity);
    }
    public IEnumerator Hacking()
    {
        if (spoon)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                if (player != gameObject) // �ڽ��� �ƴ� �÷��̾�
                {
                    Trash2 otherPlayer = player.GetComponent<Trash2>();
                    if (otherPlayer != null)
                    {
                        otherPlayer.hacking = false;
                        yield return new WaitForSeconds(5f);
                        otherPlayer.hacking = true;
                    }
                }
            }
        }
        else yield return null;
    }
    public void UseTrashCan()
    {
        trashManager.scary = 0;
    }
    public void TVOn()
    {
        soundManager.OnTVSound();
    }
    public void SmartPhone()
    {
        if (spoon)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                if (player != gameObject) // �ڽ��� �ƴ� �÷��̾�
                {
                    smartPhone = false;
                }
            }
        }
        else return;
    }
    public IEnumerator HandCream()
    {
        if (spoon)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                if (player != gameObject) // �ڽ��� �ƴ� �÷��̾�
                {
                    handCream = false;
                    yield return new WaitForSeconds(10);
                    handCream = true;
                }
            }
        }
        else yield return null;
    }
    public IEnumerator Bond()
    {
        if (spoon)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                if (player != gameObject) // �ڽ��� �ƴ� ���
                {
                    if (firstPersonController != null)
                    {
                        firstPersonController.bond = false;
                        yield return new WaitForSeconds(5);
                        firstPersonController.bond = true;
                    }
                }
            }
        }
        else yield return null;
    }
    public IEnumerator Coffee()
    {
        if (firstPersonController != null)
        {
            firstPersonController.coffee = false;
            yield return new WaitForSeconds(5);
            firstPersonController.coffee = true;
        }
    }
    public IEnumerator Soy()
    {
        if (firstPersonController != null)
        {
            firstPersonController.stamina = 0;
            yield return null;
        }
    }
    public IEnumerator Spoon()
    {
        if (spoon)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                if (player != gameObject) // �ڽ��� �ƴ� ���
                {
                    spoon = false;
                    yield return new WaitForSeconds(10);
                    spoon = true;
                }
            }
        }
        else yield return null;
    }
    private void PlaySoundForLocalPlayer()
    {
        soundManager.PoopSound();
    }
    [PunRPC]
    public void DestroyItem(int viewID)
    {
        PhotonView view = PhotonView.Find(viewID);
        Debug.Log("viewID");
        if (view != null)
        {
            Destroy(view.gameObject); // �ش� ������Ʈ ����
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

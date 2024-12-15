using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI; // UI�� ����ϱ� ���� ���ӽ����̽�

public class Trash2 : MonoBehaviourPunCallbacks
{
    [Header("상호작용 거리")]
    public float interactDistance = 5f; // 플레이어와 오브젝트 간의 최대 상호작용 거리

    [Header("쓰레기 먹으면 늘어나는 양")]
    public int Trashscary; // 쓰레기 먹으면 증가하는 공포치

    [SerializeField] private ItemPickupUI itemPickupUI; // Inspector에서 할당


    private GameObject Manager;
    private TrashManager trashManager;
    private TaskUIManager taskUIManager;
    private FirstPersonController firstPersonController;
    private GameObject TrashCan;
    private TrashCan trashCan;

    [Header("쓰레기 이름")]
    public string trash1;
    public string trash2;
    public string trash3;
    public string trash4;
    public string trash5;
    public string trash6;

    [Header("쓰레기 종류별 증가량")]
    public int trash2ScaryAmount = 10;  // 생선뼈 쓰레기
    public int trash4ScaryAmount = 15;  // 맥주캔
    public int trash5ScaryAmount = 20;  // 페트병
    public int trash6ScaryAmount = 25;  // 쓰레기봉투

    public Camera ca;

    [Header("UI 관련")]
    public GameObject interactUI; // F키 UI
    public GameObject DontUI; // 더 이상 못주울 때 뜨는 UI
    public GameObject ThrowUI; // 쓰레기통과 상호작용할 때 뜨는 UI
    public GameObject NoTrashUI; // 버릴게없음 UI
    public GameObject ItemUseUI; // 아이템 사용 알림 UI

    public Slider progressBar; // 원형 게이지 슬라이더
    public float maxHoldTime = 2f; // 최대 홀드 시간
    public float currentHoldTime = 0f; // 현재 누르고 있는 시간
    private GameObject currentTrash; // 현재 상호작용 중인 쓰레기
    private EffectTrash effectTrash;
    private bool _trashCan = false;
    [Header("사운드")]
    public PlayerSound soundManager;
    [Header("플레이어 설정")]
    public Transform playerTransform; // 플레이어의 Transform을 드래그하여 할당
    [Header("랜덤 아이템")]
    public Image ItemImage; // 아이템를 표시할 UI Image
    public Sprite[] ItemSprites; // 주문 이름에 따라 사용할 스프라이트 배열
    public string[] ItemNames; // 주문 이름 배열
    private bool itemCanUse = false;
    private string currentItem; // 현재 획득한 아이템 이름
    private string item; // 현재 획득한 아이템 이름
    public GameObject speedUPParticleEffectPrefab;
    [Header("시야 가리기 아이템")]
    public GameObject canvasPrefab; // 시야를 가릴 Canvas 프리팹
    public float effectDuration = 5f; // 효과 지속 시간
    [Header("쓰레기통 위치변경 아이템")]
    public string trashCanSpawn = "TrashCanSpawn";
    [Header("바나나")]
    public GameObject baNaNaPrefab;
    [Header("너해킹")]
    public bool hacking = true;
    [Header("스마트폰")]
    public bool smartPhone = true;
    [Header("핸드크림")]
    public bool handCream = true;
    

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
        
        ThrowUI = GameObject.Find("버리기");
        interactUI = GameObject.Find("F"); // Start에서 찾기
        if (interactUI == null)
        {
            Debug.LogError("Interact UI not found!");
        }
        Manager = GameObject.Find("Manager");
        trashManager = Manager.GetComponent<TrashManager>();
        taskUIManager = Manager.GetComponent<TaskUIManager>();

        // 기존 Start 코드...
        if (itemPickupUI == null)
        {
            itemPickupUI = FindObjectOfType<ItemPickupUI>();
        }

        TrashCan = GameObject.Find("TrashCan");
        trashCan = TrashCan.GetComponent<TrashCan>();
        //firstPersonController = GetComponent<FirstPersonController>();
        //progressBar.maxValue = maxHoldTime; // 슬라이더의 최대 값 설정
        //progressBar.value = 0; // 슬라이더 초기화
        DontUI = GameObject.Find("DontUI"); // DontUI 찾기
        if (DontUI == null)
        {
            Debug.LogError("DontUI not found!");
        }

       ItemUseUI = GameObject.Find("ItemUseUI");
     if (ItemUseUI == null)
     {
        Debug.LogError("ItemUseUI not found!");
     }
     else
     {
        ItemUseUI.SetActive(false); // 초기 상태는 비활성화
     }

        DontUI.SetActive(false);
        ThrowUI.SetActive(false);

        ca = GetComponentInChildren<Camera>();

        effectTrash = FindObjectOfType<EffectTrash>();

        NoTrashUI = GameObject.Find("버릴게없음");
         if (NoTrashUI == null)
      {
         Debug.LogError("NoTrashUI not found!");
      }
    
    // UI 초기 상태 설정
       NoTrashUI.SetActive(false);
    }

   

    public void UpdateTrashCanReference(GameObject trashCanReference)
    {
        trashCan = trashCanReference.GetComponent<TrashCan>();
    }

 

    void Update()
    {
       
    if (photonView.IsMine)
    {
        CheckForObject();

        if (itemCanUse && Input.GetMouseButtonDown(0))
        {
          
     if (smartPhone)
     {
        Debug.Log("Using item: " + currentItem);
        ItemUse(currentItem);
        if (PhotonNetwork.IsConnected)
        {
            photonView.RPC("ShowItemUseUI", RpcTarget.All);
        }
        currentItem = null;
        ItemImage.sprite = null;
        itemCanUse = false;
     }
            else if (!smartPhone)
            {
                currentItem = null;
                ItemImage.sprite = null;
                itemCanUse = false;
            }
        }
    }
        
      if (_trashCan)
        {
            if (Input.GetKey(KeyCode.F))
            {
                currentHoldTime += Time.deltaTime; // 초당 1씩 증가
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

    [PunRPC]
private void ShowItemUseUI()
{
    Debug.Log("ShowItemUseUI RPC Called");
    if (!photonView.IsMine)
    {
        if (ItemUseUI != null)
        {
            StartCoroutine(ShowItemUseUICoroutine());
        }
        else
        {
            ItemUseUI = GameObject.Find("아이템에당함");
            if (ItemUseUI != null)
            {
                StartCoroutine(ShowItemUseUICoroutine());
            }
            else
            {
                Debug.LogError("ItemUseUI not found in RPC!");
            }
        }
    }
}

 private IEnumerator ShowItemUseUICoroutine()
 {
    Debug.Log("Starting UI Coroutine");  // 코루틴 시작 확인
    if (ItemUseUI != null)
    {
        ItemUseUI.SetActive(true);
        Debug.Log("UI Activated");  // UI 활성화 확인
        yield return new WaitForSeconds(3f);
        ItemUseUI.SetActive(false);
        Debug.Log("UI Deactivated");  // UI 비활성화 확인
    }
    else
    {
        Debug.LogError("ItemUseUI is null!");
    }
 }


   
   void CheckForObject()
 {
    RaycastHit hit;
    Ray ray = ca.ScreenPointToRay(Input.mousePosition);

    if (Physics.Raycast(ray, out hit, interactDistance))
    {
        // 쓰레기통이 감지되면
        if (hit.collider.CompareTag("TrashCan"))
        {
            if (trashManager.scary <= 0)
            {
                // 쓰레기 수치가 0일 때
                NoTrashUI.SetActive(true);
                ThrowUI.SetActive(false);
                interactUI.SetActive(false);
                progressBar.gameObject.SetActive(false);
                _trashCan = false;
            }
            else
            {
                // 쓰레기 수치가 0보다 클 때
                NoTrashUI.SetActive(false);
                interactUI.SetActive(false);
                ThrowUI.SetActive(true);
                _trashCan = true;
                progressBar.gameObject.SetActive(true);
            }
        }
        // 쓰레기나 아이템이 감지되면
        else if (hit.collider.CompareTag("Trash") || hit.collider.CompareTag("UseItem") || hit.collider.CompareTag("Item"))
        {
            NoTrashUI.SetActive(false);
            interactUI.SetActive(true);
            ThrowUI.SetActive(false);
            _trashCan = false;
            progressBar.gameObject.SetActive(true);

            if (handCream)
            {
                if (hit.collider.CompareTag("Trash"))
                {
                    string objectName = hit.collider.gameObject.name;
                    int increaseAmount;

                   if (objectName.Contains(trash2)) {
                        increaseAmount = trash2ScaryAmount;  // 수정된 부분
                    }
                    else if (objectName.Contains(trash4)) {
                        increaseAmount = trash4ScaryAmount;
                    }
                    else if (objectName.Contains(trash5)) {
                        increaseAmount = trash5ScaryAmount;
                    }
                    else if (objectName.Contains(trash6)) {
                        increaseAmount = trash6ScaryAmount;
                    }
                    else {
                        increaseAmount = Trashscary;
                    }

                    // 수정된 체크 조건
                    if (trashManager.scary + increaseAmount >= 100) {
                        DontUI.SetActive(true);
                        currentHoldTime = 0f;
                        progressBar.value = 0f;
                        progressBar.gameObject.SetActive(false);
                        currentTrash = null;
                        return;
                    }

                    if (Input.GetKey(KeyCode.F)) {
                        currentTrash = hit.collider.gameObject;
                        if (currentTrash != null) {
                            ConsumeTrash(currentTrash);
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
            }
        }
        else
        {
            DontUI.SetActive(false);
            ThrowUI.SetActive(false);
            _trashCan = false;
            HideUI();
        }
    }
    else
    {
        // 레이캐스트에 감지가 없을 때 모든 UI 비활성화
        DontUI.SetActive(false);
        ThrowUI.SetActive(false);
        NoTrashUI.SetActive(false);
        _trashCan = false;
        HideUI();
    }
}


    void HideUI()
{
    interactUI.SetActive(false);
    progressBar.gameObject.SetActive(false);
    NoTrashUI.SetActive(false);
}

    void ResetHold()
    {
        currentHoldTime = 0f;
        progressBar.value = 0f;
    }

    void ConsumeTrash(GameObject trash)
{
    string objectName = trash.name;
    StartCoroutine(CollectItem(currentTrash));
    
    // 쓰레기 종류에 따라 다른 수치 적용
    if (objectName.Contains(trash2))
    {
        trashManager.scary += trash2ScaryAmount;
    }
    else if (objectName.Contains(trash4))
    {
        trashManager.scary += trash4ScaryAmount;
    }
    else if (objectName.Contains(trash5))
    {
        trashManager.scary += trash5ScaryAmount;
    }
    else if (objectName.Contains(trash6))
    {
        trashManager.scary += trash6ScaryAmount;
    }
    else
    {
        trashManager.scary += Trashscary;  // 기본 증가량
    }

    trashManager.UpdateScaryBar();
    UpdateTaskUI(objectName);
    photonView.RPC("DestroyItem", RpcTarget.Others, trash.GetComponent<PhotonView>().ViewID);
}
    void UpdateTaskUI(string objectName)
    {
        if (objectName.Contains(trash1))
        {
            taskUIManager.StoreCircleCount(); // 카운트 저장
        }
        else if (objectName.Contains(trash2))
        {
            taskUIManager.StoreCylinderCount(); // 카운트 저장
        }
        else if (objectName.Contains(trash3))
        {
            taskUIManager.StoreSquareCount(); // 카운트 저장
        }
        else if (objectName.Contains(trash4))
        {
            taskUIManager.StoreBeerCanCount(); // 카운트 저장
        }
        else if (objectName.Contains(trash5))
        {
            taskUIManager.StorePetBottleCount(); // 카운트 저장
        }
        else if (objectName.Contains(trash6))
        {
            taskUIManager.StoreTrashBagCount(); // 카운트 저장
        }
    }
   void ConsumeDangerTrash(GameObject trash)
{
    currentItem = null;
    ItemImage.sprite = null;
    
    if (trash.CompareTag("Item") || trash.CompareTag("UseItem"))
    {
        // UseItem인 경우 랜덤 아이템 선택
        if(trash.CompareTag("UseItem")) {
            int randomIndex = Random.Range(0, ItemNames.Length);
            currentItem = ItemNames[randomIndex];
        } else {
            currentItem = item;
        }

        // Trash2의 UI 업데이트
        int itemIndex = System.Array.IndexOf(ItemNames, currentItem);
        if (itemIndex >= 0 && itemIndex < ItemSprites.Length)
        {
            ItemImage.sprite = ItemSprites[itemIndex];
            itemCanUse = true;
        }

        // ItemPickupUI 업데이트
        if (itemPickupUI != null)
        {
            // trash 오브젝트와 매칭되는 이미지 찾기
            int pickupIndex = System.Array.IndexOf(itemPickupUI.gameObjects, trash);
            if (pickupIndex >= 0 && pickupIndex < itemPickupUI.objectImages.Length)
            {
                itemPickupUI.TriggerItemPickupUI(itemPickupUI.objectImages[pickupIndex]);
            }
        }

        PhotonView trashPhotonView = trash.GetComponent<PhotonView>();
        if (trashPhotonView != null)
        {
            photonView.RPC("DestroyItem", RpcTarget.All, trashPhotonView.ViewID);
        }
    }
    HideUI();
}


    public void ItemUse(string item)
    {
        switch (item)
        {
            case "안대투척"://상대 시야 5초간 안보임
                if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
                {
                    ItemManager.instane.RPCUseSkill0();
                }
                else Debug.Log("안대투척");
                break;

            case "정상수"://테이져건 상대 5초간 못움직임
                if (PhotonNetwork.IsConnected)
                {
                    ItemManager.instane.RPCUseSkill1();
                }
                else Debug.Log("정상수");
                break;

            case "변해라얍"://쓰레기통 위치 렌덤 바꾸기
                if (PhotonNetwork.IsConnected)
                {

                    photonView.RPC("TrashCanSpawns", RpcTarget.All);
                }
                else Debug.Log("변해라얍");
                break;

            case "약통"://똥싸는소리(변비약)
                if (PhotonNetwork.IsConnected)
                {
                    PlaySoundForLocalPlayer();
                }
                else Debug.Log("약통");
                break;

            case "빨랏버섯"://빨라짐
                if (PhotonNetwork.IsConnected)
                {
                    if (photonView.IsMine)
                    {
                        StartCoroutine(SpeedUP());
                        photonView.RPC("SpawnParticleEffect", RpcTarget.All, transform.position);
                    }
                    else Debug.Log("빨라버섯");
                }
                break;

            case "느렷버섯"://느려짐
                if (PhotonNetwork.IsConnected)
                {

                    StartCoroutine(SpeedDown());
                }
                else Debug.Log("느려느려버섯");
                break;

            case "없섯버섯"://아무효과없음
                Debug.Log("없섯버섯");
                break;

            case "노란바나나"://덫설치
                if (PhotonNetwork.IsConnected)
                {
                    BaNaNa();
                }
                Debug.Log("노란바나나");
                break;

            case "너해킹"://상대 10초간 아이템 사용불가
                if (PhotonNetwork.IsConnected)
                {
                    ItemManager.instane.RPCUseSkill2();
                }
                Debug.Log("너해킹");
                break;

            case "휴대용쓰레기통"://나 쓰레기통 한번 사용
                if (PhotonNetwork.IsConnected)
                {
                    if (photonView.IsMine)
                    {
                        UseTrashCan();
                    }
                }
                Debug.Log("휴대용쓰레기통");
                break;

            case "리모컨"://티비소리나옴
                if (PhotonNetwork.IsConnected)
                {
                    if (photonView.IsMine)
                    {
                        TVOn();
                    }
                }
                Debug.Log("리모컨");
                break;

            case "스마트폰"://적이 사용하는 다음 아이템 무력화 
                if (PhotonNetwork.IsConnected)
                {
                    ItemManager.instane.RPCUseSkill3();
                }
                Debug.Log("스마트폰");
                break;

            case "핸드크림":// 5초간 아무것도 수집할 수 없게 만듦
                if (PhotonNetwork.IsConnected)
                {
                    ItemManager.instane.RPCUseSkill4();
                }
                Debug.Log("핸드크림");
                break;

            case "강력본드":// 상대 플레이어가 5초간 달릴 수 없게 함
                if (PhotonNetwork.IsConnected)
                {
                    ItemManager.instane.RPCUseSkill5();
                }
                Debug.Log("강력본드");
                break;

            case "커피"://5초 동안 사용한 플레이어의 스태미너 소모없이 달릴 수 있음
                if (PhotonNetwork.IsConnected)
                {
                    if (photonView.IsMine)
                        StartCoroutine(Coffee());
                }
                Debug.Log("커피");
                break;

            case "간장"://사용한 플레이어의 스태미너를 즉시 0으로 만듦
                if (PhotonNetwork.IsConnected)
                {
                    if (photonView.IsMine)
                        StartCoroutine(Soy());
                }
                Debug.Log("간장");
                break;

            case "숟가락"://사용한 플레이어의 스태미너를 즉시 0으로 만듦
                if (PhotonNetwork.IsConnected)
                {
                    if (photonView.IsMine)
                        StartCoroutine(Spoon());
                }
                Debug.Log("숟가락");
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
                // 코루틴 시작
                StartCoroutine(trashCanScript.MoveTrashCan());
            }
            else
            {
                Debug.LogError("타겟 오브젝트에 TrashCanSpawner 스크립트가 없습니다.");
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
            Debug.Log("씨발 속도가 안변하잖아!!!!!!!!!!!!!");
        }
    }
    [PunRPC]
    private void SpawnParticleEffect(Vector3 position)
    {
        // 파티클 이펙트를 생성
        GameObject particleEffect = Instantiate(speedUPParticleEffectPrefab, position, Quaternion.identity);
        Destroy(particleEffect, 2f); // 2초 후에 파티클 이펙트 삭제
    }
    private IEnumerator SpeedDown()
    {
        if (firstPersonController != null)
        {
            float originalSpeed = firstPersonController.moveSpeed;
            float runSpeed = firstPersonController.runSpeed;
            firstPersonController.moveSpeed -= 2f;
            firstPersonController.runSpeed -= 3f;
            yield return new WaitForSeconds(5);

            firstPersonController.moveSpeed = originalSpeed;
            firstPersonController.runSpeed = runSpeed;
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
    
    public void UseTrashCan()
    {
        trashManager.scary = 0;
    }
    public void TVOn()
    {
        soundManager.OnTVSound();
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
        firstPersonController.spoon = false;
        yield return new WaitForSeconds(10);
        firstPersonController.spoon = true;
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
            Destroy(view.gameObject); // 해당 오브젝트 삭제
        }
    }
    public IEnumerator CollectItem(GameObject item)
    {
        // 콜라이더 가져오기 및 비활성화
        Collider itemCollider = item.GetComponent<Collider>();
        if (itemCollider != null)
        {
            itemCollider.enabled = false; // 콜라이더 비활성화
        }

        Vector3 originalScale = item.transform.localScale;
        Vector3 targetScale = Vector3.zero; // 최종 크기
        float duration = 1f; // 이동 및 축소 시간
        float elapsedTime = 0f;

        // EffectTrash 컴포넌트 확인
        EffectTrash itemEffectTrash = item.GetComponent<EffectTrash>();

        Vector3 myPosition = transform.position;

        while (elapsedTime < duration)
        {
            // 플레이어 방향으로 이동
            Vector3 dir = myPosition - item.transform.position;
            dir.y = 0; // Y축을 0으로 설정하여 수평 이동만 하도록 함
            dir.Normalize(); // 방향 벡터 정규화

            // 아이템을 플레이어 쪽으로 이동
            item.transform.position += dir * (3f * Time.deltaTime); // 속도 조정

            // 크기 줄이기
            item.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / (duration / 3));
            elapsedTime += Time.deltaTime;

            // PostProcess 효과는 EffectTrash 컴포넌트가 있는 경우에만 트리거
            if (itemEffectTrash != null)
            {
                itemEffectTrash.TriggerPostProcessEffect();
            }

            yield return null;
        }

        // 아이템 비활성화
        item.SetActive(false); // 아이템 비활성화
    }
}
/*public IEnumerator Spoon()
    {
        if (spoon)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                if (player != gameObject) // 자신이 아닌 경우
                {
                    spoon = false;
                    yield return new WaitForSeconds(10);
                    spoon = true;
                }
            }
        }
        else yield return null;
    }*/
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

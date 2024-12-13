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

    public Camera ca;

    [Header("UI 관련")]
    public GameObject interactUI; // F키 UI
    public GameObject DontUI; // 더 이상 못주울 때 뜨는 UI
    public GameObject ThrowUI; // 쓰레기통과 상호작용할 때 뜨는 UI

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

        DontUI.SetActive(false);
        ThrowUI.SetActive(false);

        ca = GetComponentInChildren<Camera>();

        effectTrash = FindObjectOfType<EffectTrash>();
    }

   

    public void UpdateTrashCanReference(GameObject trashCanReference)
    {
        trashCan = trashCanReference.GetComponent<TrashCan>();
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            CheckForObject();//확인

            if (itemCanUse && Input.GetMouseButtonDown(0))
            {
                if (smartPhone)
                {
                    ItemUse(currentItem);
                    currentItem = null; // 현재 아이템 초기화
                    ItemImage.sprite = null; // UI 이미지 초기화
                    itemCanUse = false;
                }
                else if (!smartPhone)
                {
                    currentItem = null; // 현재 아이템 초기화
                    ItemImage.sprite = null; // UI 이미지 초기화
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

    void CheckForObject()
    {
        RaycastHit hit;
        Ray ray = ca.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
             // 쓰레기통이 감지되면 interactUI 비활성화
            if (hit.collider.CompareTag("TrashCan"))
            {
                interactUI.SetActive(false); // 쓰레기통을 볼 때는 interactUI 비활성화
                ThrowUI.SetActive(true); // 쓰레기통 UI 활성화
                _trashCan = true;
                progressBar.gameObject.SetActive(true); // 진행 바 활성화
            }
            else
            {
                interactUI.SetActive(true); // 다른 오브젝트와 상호작용할 때는 interactUI 활성화
                ThrowUI.SetActive(false); // 쓰레기통이 아닐 경우 ThrowUI 비활성화
            }
            progressBar.gameObject.SetActive(true); // 진행 바 표시

            if (handCream)
            {
                   if (hit.collider.CompareTag("Trash"))
                {
                    // 공포치가 100 이상인 경우 완전히 상호작용 차단
                    if (trashManager.scary + Trashscary >= 100)
                    {
                        DontUI.SetActive(true); // 활성화

                        // 진행 바 및 상호작용 완전 차단
                        currentHoldTime = 0f;
                        progressBar.value = 0f;
                        progressBar.gameObject.SetActive(false);
                        currentTrash = null;
                        return; // 더 이상 진행하지 않음
                    }

                    // 기존 로직 유지 (공포치 100 미만일 때만 상호작용 가능)
                    if (Input.GetKey(KeyCode.F))
                    {
                        currentTrash = hit.collider.gameObject;
                        if (currentTrash != null)
                        {
                            ConsumeTrash(currentTrash);
                        }
                    }
                }
                // 기존의 다른 태그 체크 로직들 그대로 유지
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
            // 레이캐스트에 감지가 없을 때 DontUI 비활성화
            DontUI.SetActive(false);
            ThrowUI.SetActive(false);
            HideUI();
        }
    }


    void HideUI()
    {
        interactUI.SetActive(false); // UI 숨김
        progressBar.gameObject.SetActive(false); // 진행 바 숨김
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
        trashManager.scary += Trashscary;
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
        currentItem = null; // 현재 아이템 초기화
        ItemImage.sprite = null; // UI 이미지 초기화
        if (trash.CompareTag("Item"))
        {
            currentItem = item;
            int index = System.Array.IndexOf(ItemNames, currentItem); // 아이템 이름의 인덱스 찾기

            //StartCoroutine(CollectItem(currentTrash));
            if (index >= 0 && index < ItemSprites.Length)
            {
                ItemImage.sprite = ItemSprites[index]; // 해당 인덱스의 스프라이트로 이미지 변경
                Debug.LogWarning(currentItem);
                itemCanUse = true;
            } // 선택된 주문 이름에 따라 이미지 업데이트
            PhotonView trashPhotonView = trash.GetComponent<PhotonView>();
            if (trashPhotonView != null)
            {
                Debug.Log(trashPhotonView.ViewID);
                photonView.RPC("DestroyItem", RpcTarget.All, trashPhotonView.ViewID);
            }
            else
            {
                Debug.LogError("지금 못찾음");
            }
        }
        else if (trash.CompareTag("UseItem"))
        {
            int randomIndex = Random.Range(0, ItemNames.Length); // 랜덤 인덱스 생성
            currentItem = ItemNames[randomIndex]; // 랜덤으로 선택된 아이템 이름
            int index = System.Array.IndexOf(ItemNames, currentItem); // 아이템 이름의 인덱스 찾기

            //StartCoroutine(CollectItem(currentTrash));
            if (index >= 0 && index < ItemSprites.Length)
            {
                ItemImage.sprite = ItemSprites[index]; // 해당 인덱스의 스프라이트로 이미지 변경
                Debug.LogWarning(currentItem);
                itemCanUse = true;
            } // 선택된 주문 이름에 따라 이미지 업데이트
            PhotonView trashPhotonView = trash.GetComponent<PhotonView>();
            if (trashPhotonView != null)
            {
                photonView.RPC("DestroyItem", RpcTarget.All, trashPhotonView.ViewID);
            }
            else
            {
                Debug.LogError("지금 못찾음");
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

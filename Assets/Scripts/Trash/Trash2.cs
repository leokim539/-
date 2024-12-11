using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI를 사용하기 위한 네임스페이스
using Photon.Pun;
using Unity.VisualScripting;
using Photon.Realtime;

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
    public Slider progressBar; // 원형 게이지 슬라이더
    public float maxHoldTime = 2f; // 최대 홀드 시간
    public float currentHoldTime = 0f; // 현재 누르고 있는 시간
    private GameObject currentTrash; // 현재 상호작용 중인 쓰레기
    private EffectTrash effectTrash;
    private bool _trashCan= false;
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
    [Header("숟가락")]
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
        firstPersonController = GetComponent<FirstPersonController>();
        //progressBar.maxValue = maxHoldTime; // 슬라이더의 최대 값 설정
        //progressBar.value = 0; // 슬라이더 초기화

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
            
            if (itemCanUse && Input.GetKeyDown(KeyCode.E))
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
            interactUI.SetActive(true); // UI 표시
            progressBar.gameObject.SetActive(true); // 진행 바 표시
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
                        if (currentTrash != null) // currentTrash가 null인지 확인
                        {
                            ConsumeTrash(currentTrash); // currentTrash를 인자로 전달
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
        if (trash == null) // trash가 null인지 확인
        {
            Debug.LogError("Trash object is null!");
            return; // null인 경우 메서드 종료
        }

        if (trashManager == null) // trashManager가 null인지 확인
        {
            Debug.LogError("TrashManager is null!");
            return; // null인 경우 메서드 종료
        }
        if (trashManager.scary + Trashscary >= 100)
        {
            return; // 공포치가 100 이상이면 소비하지 않음
        }

        string objectName = trash.name;
        StartCoroutine(CollectItem(currentTrash)); // 아이템 수집 효과 실행
        trashManager.scary += Trashscary;
        trashManager.UpdateScaryBar(); // 공포치 UI 업데이트

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
        switch(item)
        {
            case "안대투척"://상대 시야 5초간 안보임
                if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
                {
                    StartCoroutine(ApplyObscuringEffect());
                }
                else Debug.Log("안대투척");
                break;

            case "정상수"://테이져건 상대 5초간 못움직임
                if (PhotonNetwork.IsConnected)
                {
                    Debug.Log("123");
                    photonView.RPC("TaserGuns", RpcTarget.Others);
                    Debug.Log("567");
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
                    StartCoroutine(Hacking());
                }
                Debug.Log("너해킹");
                break;

            case "휴대용쓰레기통"://상대 10초간 아이템 사용불가
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
                    SmartPhone();
                }
                Debug.Log("스마트폰");
                break;

            case "핸드크림":// 5초간 아무것도 수집할 수 없게 만듦
                if (PhotonNetwork.IsConnected)
                {
                    StartCoroutine(HandCream());
                }
                Debug.Log("핸드크림");
                break;

            case "강력본드":// 상대 플레이어가 5초간 달릴 수 없게 함
                if (PhotonNetwork.IsConnected)
                {
                    StartCoroutine(Bond());
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
    
    private IEnumerator ApplyObscuringEffect()
    {
        if (spoon)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                if (player != gameObject) // 자신이 아닌 경우
                {
                    GameObject canvasInstance = PhotonNetwork.Instantiate(canvasPrefab.name, player.transform.position, Quaternion.identity, 0);
                    canvasInstance.transform.SetParent(player.transform); // 상대방 오브젝트의 자식으로 설정

                    RectTransform rectTransform = canvasInstance.GetComponent<RectTransform>();
                    rectTransform.localPosition = Vector3.zero;
                    rectTransform.localScale = new Vector3(1, 1, 1);

                    yield return new WaitForSeconds(effectDuration);

                    PhotonNetwork.Destroy(canvasInstance);
                }
            }
        }
        else yield return null;
    }
    [PunRPC]
    public void TaserGuns()
    {
        Debug.Log("456");
        StartCoroutine(TaserGun());
        
    }
    public IEnumerator TaserGun()
    {
        if (spoon)
        {
            if (firstPersonController != null)
            {
                Debug.Log("코루틴 진입");
                firstPersonController.canMove = false; // 상대방의 이동 비활성화
                yield return new WaitForSeconds(5f); // 5초 대기
                firstPersonController.canMove = true; // 상대방의 이동 활성화
            }
        }
        else yield return null; // 타겟이 없을 경우
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
    private IEnumerator SpeedUP()
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
        if (spoon)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                if (player != gameObject) // 자신이 아닌 경우
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

        Instantiate(baNaNaPrefab, trapPosition, Quaternion.identity);
    }
    public IEnumerator Hacking()
    {
        if (spoon)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                if (player != gameObject) // 자신이 아닌 플레이어
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
                if (player != gameObject) // 자신이 아닌 플레이어
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
                if (player != gameObject) // 자신이 아닌 플레이어
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
                if (player != gameObject) // 자신이 아닌 경우
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
                if (player != gameObject) // 자신이 아닌 경우
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
/*public void RPC_CollectItem(string itemName)
    {
        GameObject item = GameObject.Find(itemName); // 이름으로 아이템 찾기
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
            // F키가 눌린 상태에서 슬라이더 값을 업데이트합니다.
            if (Input.GetKey(KeyCode.F))
            {
                currentHoldTime += Time.deltaTime; // 초당 1씩 증가

                // 슬라이더 값 업데이트
                progressBar.value = currentHoldTime;

                // 슬라이더 값이 최대값에 도달했을 때
                if (currentHoldTime >= maxHoldTime)
                {
                    ConsumeDangerTrash();
                }
            }
            else
            {
                // F키를 떼면 초기화
                ResetHold();
            }
        }
        if (isHolding)
        {
            // F키가 눌린 상태에서 슬라이더 값을 업데이트합니다.
            if (Input.GetKey(KeyCode.F))
            {
                currentHoldTime += Time.deltaTime; // 초당 1씩 증가

                // 슬라이더 값 업데이트
                progressBar.value = currentHoldTime;

                // 슬라이더 값이 최대값에 도달했을 때
                if (currentHoldTime >= maxHoldTime)
                {
                    ConsumeTrash();
                }
            }
            else
            {
                // F키를 떼면 초기화
                ResetHold();
            }
        }
        if (isCanHolding)
        {
            // F키가 눌린 상태에서 슬라이더 값을 업데이트합니다.
            if (Input.GetKey(KeyCode.F))
            {
                currentHoldTime += Time.deltaTime; // 초당 1씩 증가

                // 슬라이더 값 업데이트
                progressBar.value = currentHoldTime;

                // 슬라이더 값이 최대값에 도달했을 때
                if (currentHoldTime >= maxHoldTime)
                {
                    ConsumeTrashCan();
                }
            }
            else
            {
                // F키를 떼면 초기화
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
        currentTrash = trashObject; // 상호작용할 오브젝트 저장
        interactUI.SetActive(true); // UI 표시
        progressBar.gameObject.SetActive(true); // 진행 바 표시
        isHolding = true; // F키를 누르는 상태로 설정
    }
    void ShowUIDangerTrash(GameObject trashObject)
    {
        currentTrash = trashObject; // 상호작용할 오브젝트 저장
        interactUI.SetActive(true); // UI 표시
        progressBar.gameObject.SetActive(true); // 진행 바 표시
        isDangerHolding = true; // F키를 누르는 상태로 설정
    }
    void ShowUITrashCan(GameObject trashObject)
    {
        currentTrash = trashObject; // 상호작용할 오브젝트 저장
        interactUI.SetActive(true); // UI 표시
        progressBar.gameObject.SetActive(true); // 진행 바 표시
        isCanHolding = true; // F키를 누르는 상태로 설정
    }

    void HideUI()
    {
        interactUI.SetActive(false); // UI 숨김
        progressBar.gameObject.SetActive(false); // 진행 바 숨김
        ResetHold(); // F키 누르는 상태 초기화
    }

    void ResetHold()
    {
        currentHoldTime = 0f;
        progressBar.value = 0f; // 슬라이더 값 초기화
        isHolding = false;
    }

    void ConsumeTrash()
    {
        if (trashManager.scary + Trashscary >= 100)
        {
            return; // 공포치가 100 이상이면 소비하지 않음
        }

        string objectName = currentTrash.name;
        currentTrash.SetActive(false); // 쓰레기 오브젝트 비활성화

        trashManager.scary += Trashscary;
        trashManager.UpdateScaryBar(); // 공포치 UI 업데이트

        // 쓰레기 종류에 따른 UI 업데이트
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

        // 초기화
        HideUI();
    }
    void ConsumeDangerTrash()
    {
        if (trashManager.scary + Trashscary >= 100)
        {
            return; // 공포치가 100 이상이면 소비하지 않음
        }

        string objectName = currentTrash.name;
        currentTrash.SetActive(false); // 쓰레기 오브젝트 비활성화

        trashManager.scary += Trashscary;
        trashManager.UpdateScaryBar(); // 공포치 UI 업데이트
        trashManager.isEffectActive = true;

        // 쓰레기 종류에 따른 UI 업데이트
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

        // 초기화
        HideUI();
    }
    void ConsumeTrashCan()
    {
        trashManager.scary = 0;
        trashManager.UpdateScaryBar(); //공포 이미지 관리
        trashManager.isEffectActive = false;
    }*/

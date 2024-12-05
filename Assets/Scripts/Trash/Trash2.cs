using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI를 사용하기 위한 네임스페이스
using Photon.Pun;

public class Trash2 : MonoBehaviourPunCallbacks
{
    [Header("상호작용 거리")]
    public float interactDistance = 3f; // 플레이어와 오브젝트 간의 최대 상호작용 거리


    [Header("쓰레기 먹으면 늘어나는 양")]
    public int Trashscary; // 쓰레기 먹으면 증가하는 공포치

    private GameObject Manager;
    private TrashManager trashManager;
    private TaskUIManager taskUIManager;

    [Header("쓰레기 이름")]
    public string trash1;
    public string trash2;
    public string trash3;

    public Camera ca;

    [Header("UI 관련")]
    public GameObject interactUI; // F키 UI
    public Slider progressBar; // 원형 게이지 슬라이더
    public float maxHoldTime = 2f; // 최대 홀드 시간
    private float currentHoldTime = 0f; // 현재 누르고 있는 시간
    private bool isHolding = false; // F키를 누르고 있는지 여부
    private bool isDangerHolding = false; // F키를 누르고 있는지 여부

    private GameObject currentTrash; // 현재 상호작용 중인 쓰레기

    private EffectTrash effectTrash;

    [Header("플레이어 설정")]
    public Transform playerTransform; // 플레이어의 Transform을 드래그하여 할당

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
        interactUI = GameObject.Find("F"); // Start에서 찾기
        if (interactUI == null)
        {
            Debug.LogError("Interact UI not found!");
        }
        Manager = GameObject.Find("Manager");
        trashManager = Manager.GetComponent<TrashManager>();
        taskUIManager = Manager.GetComponent<TaskUIManager>();

        //progressBar.maxValue = maxHoldTime; // 슬라이더의 최대 값 설정
        //progressBar.value = 0; // 슬라이더 초기화

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
            currentHoldTime += Time.deltaTime; // 초당 1씩 증가

            // 슬라이더 값 업데이트
            progressBar.value = currentHoldTime;

            // 슬라이더 값이 최대값에 도달했을 때
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
            // F키를 떼면 초기화
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
        currentTrash = trashObject; // 상호작용할 오브젝트 저장
        interactUI.SetActive(true); // UI 표시
        progressBar.gameObject.SetActive(true); // 진행 바 표시
        if (isDanger)
        {
            isDangerHolding = true; // 위험 쓰레기 상태 설정
        }
        else
        {
            isHolding = true; // 일반 쓰레기 상태 설정
        }
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
        isDangerHolding = false; // 위험 쓰레기 초기화
    }

    void ConsumeTrash()
    {
        if (trashManager.scary + Trashscary >= 100)
        {
            return; // 공포치가 100 이상이면 소비하지 않음
        }

        string objectName = currentTrash.name;
        StartCoroutine(CollectItem(currentTrash)); // 아이템 수집 효과 실행
        trashManager.scary += Trashscary;
        trashManager.UpdateScaryBar(); // 공포치 UI 업데이트

        if (currentTrash.CompareTag("GroundTrash"))
        {
            FirstPersonController firstPersonController = FindObjectOfType<FirstPersonController>();
            if (firstPersonController != null)
            {
                firstPersonController.PickingUp(); // PickingUp 메서드 호출
            }
        }
        // 쓰레기 종류에 따른 UI 업데이트
        UpdateTaskUI(objectName);

        // 초기화
        HideUI();

        photonView.RPC("UpdateTaskUI", RpcTarget.Others, objectName);

        //photonView.RPC("RPC_CollectItem", RpcTarget.Others, currentTrash.GetPhotonView().ViewID);
    }

    void ConsumeDangerTrash()
    {
        if (trashManager.scary + Trashscary >= 100)
        {
            return; // 공포치가 100 이상이면 소비하지 않음
        }

        string objectName = currentTrash.name;
        StartCoroutine(CollectItem(currentTrash)); // 아이템 수집 효과 실행
        trashManager.scary += Trashscary;
        trashManager.UpdateScaryBar(); // 공포치 UI 업데이트
        trashManager.isEffectActive = true;
        

        // 쓰레기 종류에 따른 UI 업데이트
        UpdateTaskUI(objectName);
        
        // 초기화  
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
        GameObject item = GameObject.Find(itemName); // 이름으로 아이템 찾기
        if (item != null)
        {
            StartCoroutine(CollectItem(item));
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

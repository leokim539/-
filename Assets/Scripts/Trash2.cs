using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI를 사용하기 위한 네임스페이스

public class Trash2 : MonoBehaviour
{
    [Header("상호작용 거리")]
    public float interactDistance = 1.5f; // 플레이어와 오브젝트 간의 최대 상호작용 거리

    [Header("쓰레기 먹으면 늘어나는 양")]
    public int Trashscary; // 쓰레기 먹으면 증가하는 공포치

    private GameObject Manager;
    private TrashManager trashManager;
    private TaskUIManager taskUIManager;

    [Header("쓰레기 이름")]
    public string trash1;
    public string trash2;
    public string trash3;

    public Camera camera;

    [Header("UI 관련")]
    public GameObject interactUI; // F키 UI
    public Slider progressBar; // 원형 게이지 슬라이더
    public float maxHoldTime = 2f; // 최대 홀드 시간
    private float currentHoldTime = 0f; // 현재 누르고 있는 시간
    private bool isHolding = false; // F키를 누르고 있는지 여부

    private GameObject currentTrash; // 현재 상호작용 중인 쓰레기

    void Start()
    {
        Manager = GameObject.Find("Manager");
        trashManager = Manager.GetComponent<TrashManager>();
        taskUIManager = Manager.GetComponent<TaskUIManager>();

        interactUI.SetActive(false); // 처음엔 UI를 숨깁니다.
        progressBar.maxValue = 2; // 슬라이더의 최대 값 설정
        progressBar.value = 0; // 슬라이더 초기화
    }

    void Update()
    {
        CheckForObject();

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
        currentTrash = trashObject; // 상호작용할 오브젝트 저장
        interactUI.SetActive(true); // UI 표시
        progressBar.gameObject.SetActive(true); // 진행 바 표시
        isHolding = true; // F키를 누르는 상태로 설정
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
}
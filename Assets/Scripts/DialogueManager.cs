using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public SlideInImage slideInImage; // SlideInImage 스크립트
    public GameObject dialoguePanel; // 대화창 Panel
    public TMP_Text dialogueText; // TextMeshPro 대화 텍스트
    public Button nextButton; // 다음 버튼

    public string[] dialogueLines; // 대화 내용 배열
    public float textSpeed = 0.05f; // 텍스트 표시 속도
    public Vector2 slideDownPosition = new Vector2(0, -500); // 이미지가 내려갈 위치 설정
    public float slideDownDelay = 2f; // 이미지가 내려가는 딜레이 시간 (초)

    private int currentLineIndex = 0; // 현재 대화 인덱스
    private bool isTyping = false; // 텍스트가 출력 중인지 확인
    private bool isDialogueEnded = false; // 대화가 끝났는지 확인

    void Start()
    {
        // 대화창과 버튼을 비활성화 상태로 시작
        dialoguePanel.SetActive(false);
        nextButton.gameObject.SetActive(false);

        // SlideInImage 완료 시점에 대화창 실행
        slideInImage.OnSlideComplete += StartDialogue;
    }

    void StartDialogue()
    {
        if (isDialogueEnded)
            return;

        StartCoroutine(ShowDialogueAfterDelay(3f));
    }

    IEnumerator ShowDialogueAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!isDialogueEnded)
        {
            // 대화창 활성화 및 첫 대화 출력
            dialoguePanel.SetActive(true);
            StartCoroutine(TypeText());
        }
    }

    void Update()
    {
        // 대화창 활성화 상태에 따라 커서 관리
        ManageCursorVisibility();

        // 스페이스바로 다음 대화로 넘어가기
        if (Input.GetKeyDown(KeyCode.Space) && !isTyping && !isDialogueEnded)
        {
            NextDialogue();
        }
    }

    private void ManageCursorVisibility()
    {
        if (dialoguePanel.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None; // 커서 잠금 해제
            Cursor.visible = true; // 커서 보이기
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; // 커서 잠금
            Cursor.visible = false; // 커서 숨기기
        }
    }

    IEnumerator TypeText()
    {
        if (isDialogueEnded)
            yield break; // 대화가 끝났으면 종료

        isTyping = true;
        dialogueText.text = "";

        // 텍스트를 한 글자씩 표시
        foreach (char c in dialogueLines[currentLineIndex])
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        // 텍스트 표시 완료 후 버튼 활성화
        isTyping = false;
        nextButton.gameObject.SetActive(true);
    }

    public void NextDialogue()
    {
        if (isDialogueEnded)
            return;

        // 다음 대화로 이동
        currentLineIndex++;
        if (currentLineIndex < dialogueLines.Length)
        {
            StartCoroutine(TypeText());
        }
        else
        {
            EndDialogue();
        }
    }

    public void OnNextButtonClicked()
    {
        if (isTyping || isDialogueEnded)
            return; // 텍스트 출력 중 또는 대화 종료 후 버튼 작동 금지

        // 다음 대화로 이동
        currentLineIndex++;

        if (currentLineIndex < dialogueLines.Length)
        {
            StartCoroutine(TypeText());
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        isDialogueEnded = true;

        // 대화창 비활성화
        dialoguePanel.SetActive(false);

        // 딜레이 후 SlideDown 호출
        StartCoroutine(DelayedSlideDown());
    }

    IEnumerator DelayedSlideDown()
    {
        yield return new WaitForSeconds(slideDownDelay);

        // SlideDown 호출 (사용자 정의 위치 전달)
        slideInImage.SlideDown(slideDownPosition);
    }
}
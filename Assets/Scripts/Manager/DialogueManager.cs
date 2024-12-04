using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public SlideInImage slideInImage; // SlideInImage ��ũ��Ʈ
    public GameObject dialoguePanel; // ��ȭâ Panel
    public TMP_Text dialogueText; // TextMeshPro ��ȭ �ؽ�Ʈ

    public string[] dialogueLines; // ��ȭ ���� �迭
    public float textSpeed = 0.05f; // �ؽ�Ʈ ǥ�� �ӵ�
    public Vector2 slideDownPosition = new Vector2(0, -500); // �̹����� ������ ��ġ ����
    public float slideDownDelay = 2f; // �̹����� �������� ������ �ð� (��)

    private int currentLineIndex = 0; // ���� ��ȭ �ε���
    private bool isTyping = false; // �ؽ�Ʈ�� ��� ������ Ȯ��
    private bool isDialogueEnded = false; // ��ȭ�� �������� Ȯ��

    void Start()
    {
        // ��ȭâ�� ��Ȱ��ȭ ���·� ����
        dialoguePanel.SetActive(false);

        // SlideInImage �Ϸ� ������ ��ȭâ ����
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
            // ��ȭâ Ȱ��ȭ �� ù ��ȭ ���
            dialoguePanel.SetActive(true);
            StartCoroutine(TypeText());
        }
    }

    void Update()
    {
        // ��ȭâ Ȱ��ȭ ���¿� ���� Ŀ�� ����
        ManageCursorVisibility();

        // �����̽��ٷ� ���� ��ȭ�� �Ѿ��
        if (Input.GetKeyDown(KeyCode.Space) && dialoguePanel.activeSelf && !isTyping && !isDialogueEnded)
        {
            NextDialogue();
        }

        // ESC�� ��ȭ ���� ��û (��ȭâ�� Ȱ��ȭ�� ��쿡�� �۵�)
        if (Input.GetKeyDown(KeyCode.Escape) && dialoguePanel.activeSelf)
        {
            CloseDialoguePanelImmediately();
        }
    }

    private void ManageCursorVisibility()
    {
        if (dialoguePanel.activeSelf)
        {
            Cursor.lockState = CursorLockMode.None; // Ŀ�� ��� ����
            Cursor.visible = true; // Ŀ�� ���̱�
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked; // Ŀ�� ���
            Cursor.visible = false; // Ŀ�� �����
        }
    }

    IEnumerator TypeText()
    {
        if (isDialogueEnded)
            yield break; // ��ȭ�� �������� ����

        isTyping = true;
        dialogueText.text = "";

        // �ؽ�Ʈ�� �� ���ھ� ǥ��
        foreach (char c in dialogueLines[currentLineIndex])
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        // �ؽ�Ʈ ǥ�� �Ϸ�
        isTyping = false;
    }

    public void NextDialogue()
    {
        if (isDialogueEnded)
            return;

        // ���� ��ȭ�� �̵�
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

        // ��ȭâ ��Ȱ��ȭ
        dialoguePanel.SetActive(false);

        // ������ �� SlideDown ȣ��
        StartCoroutine(DelayedSlideDown());
    }

    private void CloseDialoguePanelImmediately()
    {
        // ESC ������ ��ȭâ ��� ��Ȱ��ȭ
        dialoguePanel.SetActive(false);

        // SlideDown ȣ�� (�̹����� �������� ������ ��ȭâ�� ������ ����)
        slideInImage.SlideDown(slideDownPosition);
    }

    IEnumerator DelayedSlideDown()
    {
        yield return new WaitForSeconds(slideDownDelay);

        // SlideDown ȣ�� (����� ���� ��ġ ����)
        slideInImage.SlideDown(slideDownPosition);
    }
}
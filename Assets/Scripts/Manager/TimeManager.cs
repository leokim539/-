using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public Image[] images; // ����� �̹��� �迭
    public AudioClip[] audioClips; // ����� ����� Ŭ�� �迭
    public AudioSource audioSource; // AudioSource ������Ʈ

    public Text timerText; // �ð�
    public float totalTime; // �� �ð� (�� ����)
    private float remainingTime; // �پ�� �ð�

    // �¸� �� �й� UI
    public GameObject WinUI;
    public GameObject LoseUI;

    void Start()
    {
        remainingTime = totalTime; // �ð� �ʱ�ȭ
        WinUI.SetActive(false); // �ʱ� ���¿��� ��Ȱ��ȭ
        LoseUI.SetActive(false); // �ʱ� ���¿��� ��Ȱ��ȭ
    }

    void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime; // �ð� ���
            UpdateTimerDisplay(); // �ð� ǥ��

            // �̺�Ʈ Ʈ����
            if (remainingTime <= 100f && remainingTime > 100f - Time.deltaTime)
            {
                StartCoroutine(TriggerEvent());
                Debug.Log("100");
            }
            if (remainingTime <= 90f && remainingTime > 90f - Time.deltaTime)
            {
                StartCoroutine(TriggerEvent());
                Debug.Log("90");
            }
            if (remainingTime <= 80f && remainingTime > 80f - Time.deltaTime)
            {
                StartCoroutine(TriggerEvent());
                Debug.Log("80");
            }
        }
        else
        {
            remainingTime = 0; // ���� �ð��� 0���� ����
            timerText.color = Color.red; // �ð��� ������ �� ���� ����
            DetermineWinner(); // �¸��� ����
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60); // �� ���
        int seconds = Mathf.FloorToInt(remainingTime % 60); // �� ���
        timerText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds); // ǥ��
    }

    void DetermineWinner()
    {
        // ��� �÷��̾��� TaskUIManager ã��
        TaskUIManager[] players = FindObjectsOfType<TaskUIManager>();

        int player1Count = 0;
        int player2Count = 0;

        // �÷��̾� ���� ���� ���� ���
        if (players.Length > 0)
        {
            player1Count = players[0].GetTotalTrashCount(); // ù ��° �÷��̾� ����

            if (players.Length > 1)
            {
                player2Count = players[1].GetTotalTrashCount(); // �� ��° �÷��̾� ����
            }
        }

        // �¸� �� �й� UI Ȱ��ȭ
        if (players.Length == 1)
        {
            // �÷��̾ �� ���� ���, �ڵ����� �¸� ó��
            WinUI.SetActive(true);
            LoseUI.SetActive(false);
        }
        else if (player1Count > player2Count)
        {
            WinUI.SetActive(true);
            LoseUI.SetActive(false);
        }
        else if (player1Count < player2Count)
        {
            WinUI.SetActive(false);
            LoseUI.SetActive(true);
        }
        else
        {
            // ���º� ó�� (���ϴ� ���)
            WinUI.SetActive(false);
            LoseUI.SetActive(false);
        }
    }


    IEnumerator TriggerEvent()
    {
        int randomImageIndex = Random.Range(0, images.Length); // ���� �̹��� ����
        foreach (var img in images)
        {
            img.gameObject.SetActive(false); // ��� �̹��� ��Ȱ��ȭ
        }
        images[randomImageIndex].gameObject.SetActive(true); // ���� �̹��� Ȱ��ȭ

        // ���� �Ҹ� ����
        int randomSoundIndex = Random.Range(0, audioClips.Length);
        audioSource.clip = audioClips[randomSoundIndex]; // ���� �Ҹ� ����
        audioSource.Play(); // �Ҹ� ���

        if (images.Length == 0 || audioClips.Length == 0)
        {
            yield break;
        }

        yield return new WaitForSeconds(5);

        // ��Ȱ��ȭ
        images[randomImageIndex].gameObject.SetActive(false); // �̹��� ��Ȱ��ȭ
        audioSource.Stop(); // �Ҹ� ����
    }
}

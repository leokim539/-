using System.Collections; // IEnumerator�� ����ϱ� ���� �߰�
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public Text timerText; // �ð�
    public float totalTime; // �� �ð� (�� ����)
    private float remainingTime; // �پ�� �ð�

    // ���� ���� UI
    public GameObject GameEndUI;

    void Start()
    {
        remainingTime = totalTime; // �ð� �ʱ�ȭ
        GameEndUI.SetActive(false); // �ʱ� ���¿��� ��Ȱ��ȭ
    }

    void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime; // �ð� ���
            UpdateTimerDisplay(); // �ð� ǥ��
        }
        else
        {
            remainingTime = 0; // ���� �ð��� 0���� ����
            timerText.color = Color.red; // �ð��� ������ �� ���� ����
            ShowGameEndUI(); // ���� ���� UI ǥ��
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60); // �� ���
        int seconds = Mathf.FloorToInt(remainingTime % 60); // �� ���
        timerText.text = string.Format("{0:D2}:{1:D2}", minutes, seconds); // ǥ��
    }

    void ShowGameEndUI()
    {
        GameEndUI.SetActive(true); // ���� ���� UI Ȱ��ȭ
        StopPlayerMovement(); // �÷��̾��� ������ ����
        DetermineWinner(); // �¸��� ����
    }

    void StopPlayerMovement()
    {
        // "Player" �±׸� ���� ��� ������Ʈ ã��
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            FirstPersonController controller = player.GetComponent<FirstPersonController>();
            if (controller != null)
            {
                controller.enabled = false; // FirstPersonController ��ũ��Ʈ ��Ȱ��ȭ
                Debug.Log($"Stopped movement for player: {player.name}"); // ����� �α� �߰�
            }
            else
            {
                Debug.LogWarning($"No FirstPersonController found on player: {player.name}"); // ��� �α� �߰�
            }
        }

        // 2�� �� Ending �� �ε�
        StartCoroutine(LoadEndingSceneAfterDelay(2f));
    }

    private IEnumerator LoadEndingSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // ������
        EndingScene endingScene = FindObjectOfType<EndingScene>(); // EndingScene ��ũ��Ʈ ã��
        if (endingScene != null)
        {
            endingScene.LoadEndingScene(); // Ending �� �ε�
        }
        else
        {
            Debug.LogWarning("EndingScene script not found in the scene."); // ��� �α� �߰�
        }
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

        // �¸��� ���� �� UI ó��
        if (player1Count > player2Count)
        {
            Debug.Log("Player 1 Wins!");
            // �¸� �ִϸ��̼� �Ǵ� UI ó��
        }
        else if (player1Count < player2Count)
        {
            Debug.Log("Player 2 Wins!");
            // �¸� �ִϸ��̼� �Ǵ� UI ó��
        }
        else
        {
            Debug.Log("It's a Draw!");
            // ���º� ó��
        }
    }
}

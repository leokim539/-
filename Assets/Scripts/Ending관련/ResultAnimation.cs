using UnityEngine;
using System.Collections.Generic;

public class ResultAnimation : MonoBehaviour
{
    private Animator[] animators;
    private void Start()
    {
        // Player �±׸� ���� ������Ʈ�� �߿��� �ڽ� Idle ������Ʈ�� �ִϸ����� ã��
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        // �ӽ� ����Ʈ ����
        List<Animator> foundAnimators = new List<Animator>();
        foreach (GameObject player in players)
        {
            // �� �÷��̾� ������Ʈ�� �ڽ� �� Idle ������Ʈ ã��
            Transform idleTransform = player.transform.Find("Idle");
            if (idleTransform != null)
            {
                Animator idleAnimator = idleTransform.GetComponent<Animator>();
                if (idleAnimator != null)
                {
                    foundAnimators.Add(idleAnimator);
                }
                else
                {
                    Debug.LogWarning($"Idle ������Ʈ�� Animator ������Ʈ�� �����ϴ�: {idleTransform.name}");
                }
            }
            else
            {
                Debug.LogWarning($"Idle ������Ʈ�� ã�� �� �����ϴ�: {player.name}");
            }
        }
        animators = foundAnimators.ToArray();
        if (animators.Length == 0)
        {
            Debug.LogError("�÷��̾� �ִϸ����͸� ã�� �� �����ϴ�.");
        }
    }

    public void SetResult(bool isPlayer1Winner)
    {
        if (animators == null || animators.Length == 0)
        {
            Debug.LogError("�ִϸ����Ͱ� �������� �ʾҽ��ϴ�.");
            return;
        }

        foreach (Animator animator in animators)
        {
            if (animator != null)
            {
                // Player1�� �¸����� ��
                if (isPlayer1Winner)
                {
                    // Player1 �ִϸ������� isLoser�� false
                    if (animator.gameObject.transform.parent.name == "Player1")
                    {
                        animator.SetBool("isLoser", false);
                    }
                    // Player2 �ִϸ������� isLoser�� true
                    else if (animator.gameObject.transform.parent.name == "Player2")
                    {
                        animator.SetBool("isLoser", true);
                    }
                }
                // Player2�� �¸����� ��
                else
                {
                    // Player2 �ִϸ������� isLoser�� false
                    if (animator.gameObject.transform.parent.name == "Player2")
                    {
                        animator.SetBool("isLoser", false);
                    }
                    // Player1 �ִϸ������� isLoser�� true
                    else if (animator.gameObject.transform.parent.name == "Player1")
                    {
                        animator.SetBool("isLoser", true);
                    }
                }
            }
        }
    }
}
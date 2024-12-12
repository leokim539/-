using UnityEngine;
using System.Collections.Generic;

public class ResultAnimation : MonoBehaviour
{
    private Animator[] animators;
    private void Start()
    {
        // Player 태그를 가진 오브젝트들 중에서 자식 Idle 오브젝트의 애니메이터 찾기
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        // 임시 리스트 생성
        List<Animator> foundAnimators = new List<Animator>();
        foreach (GameObject player in players)
        {
            // 각 플레이어 오브젝트의 자식 중 Idle 오브젝트 찾기
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
                    Debug.LogWarning($"Idle 오브젝트에 Animator 컴포넌트가 없습니다: {idleTransform.name}");
                }
            }
            else
            {
                Debug.LogWarning($"Idle 오브젝트를 찾을 수 없습니다: {player.name}");
            }
        }
        animators = foundAnimators.ToArray();
        if (animators.Length == 0)
        {
            Debug.LogError("플레이어 애니메이터를 찾을 수 없습니다.");
        }
    }

    public void SetResult(bool isPlayer1Winner)
    {
        if (animators == null || animators.Length == 0)
        {
            Debug.LogError("애니메이터가 설정되지 않았습니다.");
            return;
        }

        foreach (Animator animator in animators)
        {
            if (animator != null)
            {
                // Player1이 승리했을 때
                if (isPlayer1Winner)
                {
                    // Player1 애니메이터의 isLoser는 false
                    if (animator.gameObject.transform.parent.name == "Player1")
                    {
                        animator.SetBool("isLoser", false);
                    }
                    // Player2 애니메이터의 isLoser는 true
                    else if (animator.gameObject.transform.parent.name == "Player2")
                    {
                        animator.SetBool("isLoser", true);
                    }
                }
                // Player2가 승리했을 때
                else
                {
                    // Player2 애니메이터의 isLoser는 false
                    if (animator.gameObject.transform.parent.name == "Player2")
                    {
                        animator.SetBool("isLoser", false);
                    }
                    // Player1 애니메이터의 isLoser는 true
                    else if (animator.gameObject.transform.parent.name == "Player1")
                    {
                        animator.SetBool("isLoser", true);
                    }
                }
            }
        }
    }
}
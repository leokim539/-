using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleEvent : MonoBehaviour
{
    private bool hasTriggered = false; // 트리거가 실행되었는지 여부

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true; // 트리거가 실행됨을 기록
            StartCoroutine(HandleCandleEvent());
        }
    }

    private IEnumerator HandleCandleEvent()
    {
        // 이 오브젝트의 자식 오브젝트를 순회
        foreach (Transform child in transform)
        {
            // AudioSource가 있는 경우 재생
            AudioSource audioSource = child.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.Play();
            }
        }

        // 1초 대기
        yield return new WaitForSeconds(0.3f);

        // 자식 오브젝트의 자식 오브젝트 비활성화
        foreach (Transform child in transform)
        {
            foreach (Transform grandChild in child)
            {
                grandChild.gameObject.SetActive(false);
            }
        }

        // 이 스크립트를 비활성화
        this.enabled = false;
    }
}


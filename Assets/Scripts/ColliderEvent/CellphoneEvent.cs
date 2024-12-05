using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellphoneEvent : MonoBehaviour
{
    private bool hasTriggered = false; // 트리거가 호출되었는지 여부

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered) // Player와 충돌하고, 아직 트리거되지 않은 경우
        {
            hasTriggered = true; // 트리거가 호출되었음을 기록

            // 이 오브젝트의 자식 오브젝트를 순회
            foreach (Transform child in transform)
            {
                // 자식 오브젝트 활성화
                child.gameObject.SetActive(true);

                // AudioSource가 있는 경우 재생
                AudioSource audioSource = child.GetComponent<AudioSource>();
                if (audioSource != null)
                {
                    audioSource.Play();
                }
            }

            // 스크립트를 비활성화하여 다시 호출되지 않도록 설정
            this.enabled = false; // 스크립트 비활성화
        }
    }
}


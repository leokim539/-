using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellphoneEvent : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
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
        }
    }
}


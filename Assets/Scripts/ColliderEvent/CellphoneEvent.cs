using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellphoneEvent : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // �� ������Ʈ�� �ڽ� ������Ʈ�� ��ȸ
            foreach (Transform child in transform)
            {
                // �ڽ� ������Ʈ Ȱ��ȭ
                child.gameObject.SetActive(true);

                
                    // AudioSource�� �ִ� ��� ���
                    AudioSource audioSource = child.GetComponent<AudioSource>();
                    if (audioSource != null)
                    {
                        audioSource.Play();
                    }

               
            }
        }
    }
}


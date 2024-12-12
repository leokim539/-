using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellphoneEvent : MonoBehaviour
{
    private bool hasTriggered = false; // Ʈ���Ű� ȣ��Ǿ����� ����

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered) // Player�� �浹�ϰ�, ���� Ʈ���ŵ��� ���� ���
        {
            hasTriggered = true; // Ʈ���Ű� ȣ��Ǿ����� ���

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

            // ��ũ��Ʈ�� ��Ȱ��ȭ�Ͽ� �ٽ� ȣ����� �ʵ��� ����
            this.enabled = false; // ��ũ��Ʈ ��Ȱ��ȭ
        }
    }
}


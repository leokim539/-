using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleEvent : MonoBehaviour
{
    private bool hasTriggered = false; // Ʈ���Ű� ����Ǿ����� ����

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true; // Ʈ���Ű� ������� ���
            StartCoroutine(HandleCandleEvent());
        }
    }

    private IEnumerator HandleCandleEvent()
    {
        // �� ������Ʈ�� �ڽ� ������Ʈ�� ��ȸ
        foreach (Transform child in transform)
        {
            // AudioSource�� �ִ� ��� ���
            AudioSource audioSource = child.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.Play();
            }
        }

        // 1�� ���
        yield return new WaitForSeconds(0.3f);

        // �ڽ� ������Ʈ�� �ڽ� ������Ʈ ��Ȱ��ȭ
        foreach (Transform child in transform)
        {
            foreach (Transform grandChild in child)
            {
                grandChild.gameObject.SetActive(false);
            }
        }

        // �� ��ũ��Ʈ�� ��Ȱ��ȭ
        this.enabled = false;
    }
}


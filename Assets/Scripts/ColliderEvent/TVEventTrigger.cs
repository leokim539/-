using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TVEventTrigger : MonoBehaviour
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

                // �ڽ� ������Ʈ�� �ڽ� ������Ʈ Ȱ��ȭ
                foreach (Transform grandChild in child)
                {
                    grandChild.gameObject.SetActive(true);

                    // AudioSource�� �ִ� ��� ���
                    AudioSource audioSource = grandChild.GetComponent<AudioSource>();
                    if (audioSource != null)
                    {
                        audioSource.Play();
                    }
                }
            }
        }
    }
}

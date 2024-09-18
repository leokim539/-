using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float interactionDistance = 3f; // ��ȣ�ۿ� �Ÿ�
    public GameObject hiddenObject; // ������ ������Ʈ

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ���콺 ��Ŭ��
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, interactionDistance))
            {
                if (hit.collider.CompareTag("Haunted")) // �±� Ȯ��(�±״� �ƹ����Գ� �ٲ㵵 �˴ϴ�)
                {
                    Destroy(hit.collider.gameObject); // HauntedObject ����
                    if (hiddenObject != null)
                    {
                        hiddenObject.SetActive(true); // ������ ������Ʈ Ȱ��ȭ
                    }
                }
            }
        }

    }
    // �̰� Ŭ���ϸ� ������Ʈ �ٲ�� �̺�Ʈ



    private void OnTriggerEnter(Collider other)
    {
        // "Event" �±׸� ���� ������Ʈ�� �������� ��(�±״� �ƹ����Գ� �ٲ㵵 �˴ϴ�)
        if (other.CompareTag("Event"))
        {
            // �ڽ� ������Ʈ �� TV �±׸� ���� ������Ʈ ã��(�±״� �ƹ����Գ� �ٲ㵵 �˴ϴ�)
            foreach (Transform child in other.transform)
            {
                foreach (Transform grandChild in child)  // �ڽ� ������Ʈ�� �ڽ� ������Ʈ ã��
                {
                    if (grandChild.CompareTag("TV")) //(�±״� �ƹ����Գ� �ٲ㵵 �˴ϴ�)
                    {
                        // TV Ȱ��ȭ
                        grandChild.gameObject.SetActive(true);

                        // AudioSource ������Ʈ�� ã�Ƽ� �Ҹ� ���
                        AudioSource audioSource = grandChild.GetComponent<AudioSource>();
                        if (audioSource != null)
                        {
                            audioSource.Play();
                        }
                    }
                    // �̰� �ݶ��̴� �����ϸ� Ƽ�� ������ �̺�Ʈ
                }

                // 1 �±׸� ���� �ڽ� ������Ʈ ã��
                if (child.CompareTag("1")) // (�±״� �ƹ����Գ� �ٲ㵵 �˴ϴ�)
                {
                    // 1 Ȱ��ȭ
                    child.gameObject.SetActive(true);
                }
            }
        }
        // �̰� �ݶ��̴� �����ϸ� ������ ��Ÿ���� �̺�Ʈ
    }

}
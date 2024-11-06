using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public GameObject interactionUI; // ��ȣ�ۿ� UI
    public float moveDistance = 90f; // ���� ȸ���� ����
    private bool isOpen = false; // ���� �����ִ��� ����
    private bool isMoving = false; // ���� �̵� ������ ����

    // ����� Ŭ�� ����
    public AudioClip openSound; // ���� ���� �� ����� �Ҹ�
    public AudioClip closeSound; // ���� ���� �� ����� �Ҹ�
    private AudioSource audioSource; // ����� �ҽ�

    private void Start()
    {
        // �ʱ� ���¿��� X���� -90���� ����
        transform.localEulerAngles = new Vector3(-90, transform.localEulerAngles.y, 0);

        // AudioSource ������Ʈ ��������
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        // E Ű �Է� üũ
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;

            // �÷��̾� ��ġ���� �������� ����ĳ��Ʈ
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 1.5f))
            {
                // ����ĳ��Ʈ�� ������ ������Ʈ�� DoorController ������Ʈ Ȯ��
                DoorController door = hit.transform.GetComponent<DoorController>();
                if (door != null && !door.isMoving) // DoorController�� �����ϰ� �̵� ���� �ƴ� ��
                {
                    StartCoroutine(door.ToggleDoor()); // �ش� ���� ����
                }
            }
        }

        // ����ĳ��Ʈ�� ���� UI Ȱ��ȭ/��Ȱ��ȭ
        RaycastHit hitInteraction;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInteraction, 3f))
        {
            if (hitInteraction.transform.CompareTag("Door"))
            {
                interactionUI.SetActive(true); // UI Ȱ��ȭ
            }
            else
            {
                interactionUI.SetActive(false); // UI ��Ȱ��ȭ
            }
        }
        else
        {
            interactionUI.SetActive(false); // UI ��Ȱ��ȭ
        }
    }

    public IEnumerator ToggleDoor() // public���� �����Ͽ� �ܺο��� ȣ�� �����ϵ���
    {
        isMoving = true; // �̵� �� ���� ����
        float targetAngle = isOpen ? -moveDistance : moveDistance; // ȸ���� ���� ����
        float currentAngle = transform.localEulerAngles.y; // ���� Y�� ����
        float newAngle = currentAngle + targetAngle; // ��ǥ ����

        // ����� Ŭ�� ����
        AudioClip currentClip = isOpen ? closeSound : openSound;
        audioSource.clip = currentClip;
        audioSource.loop = true; // ���� ����
        audioSource.Play(); // �Ҹ� ��� ����

        // ���� ȸ����Ű�� �ִϸ��̼�
        while (Mathf.Abs(newAngle - transform.localEulerAngles.y) > 0.1f)
        {
            float step = Time.deltaTime * 150f; // ȸ�� �ӵ�
            float angle = Mathf.MoveTowardsAngle(transform.localEulerAngles.y, newAngle, step);
            transform.localEulerAngles = new Vector3(-90, angle, 0); // X���� -90, Y�ุ ȸ��
            yield return null; // ���� �����ӱ��� ���
        }

        // ���� ������ ����
        transform.localEulerAngles = new Vector3(-90, newAngle, 0); // X���� -90, Y�ุ ����

        // �Ҹ� ��� ����
        audioSource.loop = false; // ���� ����
        audioSource.Stop();

        isOpen = !isOpen; // �� ���� ����
        isMoving = false; // �̵� �Ϸ� ���·� ����
    }
}
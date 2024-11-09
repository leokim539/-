using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public GameObject interactionUI; // ��ȣ�ۿ� UI

    private bool isOpen = false; // ���� �����ִ��� ����
    private bool isMoving = false; // ���� �̵� ������ ����

    // ����� Ŭ�� ����
    public AudioClip openSound; // ���� ���� �� ����� �Ҹ�
    public AudioClip closeSound; // ���� ���� �� ����� �Ҹ�
    private AudioSource audioSource; // ����� �ҽ�

    private void Start()
    {
        // �ʱ� ���¿��� X���� -90���� ����
      //  transform.localEulerAngles = new Vector3(-90, transform.localEulerAngles.y, 0);

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

        // ���� ���� ���� ���� ���� ���� ������ ������ ����
        float currentAngle = transform.localEulerAngles.y;
        float targetAngle;

        // ���� ���� �ִ� ������ ��
        if (!isOpen)
        {
            targetAngle = currentAngle + 90f; // ���� �������� 90�� ���ϱ�
        }
        else // ���� ���� �ִ� ������ ��
        {
            targetAngle = currentAngle - 90f; // ���� �������� 90�� ����
        }

        // ����� Ŭ�� ����
        AudioClip currentClip = isOpen ? closeSound : openSound;
        audioSource.clip = currentClip;
        audioSource.loop = false; // ���� ���� ����
        audioSource.Play(); // �Ҹ� ��� ����

        // ���� ȸ����Ű�� �ִϸ��̼�
        while (Mathf.Abs(targetAngle - currentAngle) > 0.1f)
        {
            float step = Time.deltaTime * 150f; // ȸ�� �ӵ�
            currentAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, step); // ���� ������ ��ǥ ������ �̵�
            transform.localEulerAngles = new Vector3(0, currentAngle, 90); // X���� 0, Y�ุ ȸ��
            yield return null; // ���� �����ӱ��� ���
        }

        // ���� ������ ����
        transform.localEulerAngles = new Vector3(0, targetAngle, 90); // X���� 0, Y�ุ ����

        // �Ҹ� ��� ����
        audioSource.Stop();

        isOpen = !isOpen; // �� ���� ����
        isMoving = false; // �̵� �Ϸ� ���·� ����
    }

}

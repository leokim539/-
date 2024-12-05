using System.Collections;
using UnityEngine;
using Photon.Pun;

public class DrawerController : MonoBehaviourPunCallbacks
{
    public GameObject openUI;  // ������ ������ �� ǥ���� UI
    public GameObject closeUI;  // ������ ������ �� ǥ���� UI
    private bool isOpen = false;
    private bool isMoving = false;

    public AudioClip openSound;  // ���� �� �� �Ҹ�
    public AudioClip closeSound;  // ���� ���� �� �Ҹ�
    private AudioSource audioSource;

    [SerializeField]
    private Vector3 openPosition;    // ������ ������ �� ��ġ
    [SerializeField]
    private Vector3 closedPosition;  // ������ ������ �� ��ġ
    [SerializeField]
    private float moveDuration = 0.5f;   // ������ ������ �ð�

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // UI �ʱ� ���� ����
        openUI.SetActive(false);
        closeUI.SetActive(false);
    }

    private void Update()
    {
        // E Ű �Է� üũ
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 2f))
            {
                DrawerController drawer = hit.transform.GetComponent<DrawerController>();
                if (drawer != null && drawer.enabled && !drawer.isMoving)
                {
                    // RPC ȣ��� ��ξ ���� �۾�
                    drawer.photonView.RPC("ToggleDrawerRPC", RpcTarget.All);
                }
            }
        }

        // UI ǥ�� ���� ����ĳ��Ʈ
        RaycastHit hitInteraction;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInteraction, 2f))
        {
            if (hitInteraction.transform.CompareTag("Drawer"))
            {
                DrawerController drawer = hitInteraction.transform.GetComponent<DrawerController>();
                if (drawer != null && drawer.enabled)
                {
                    // ���� ���¿� ���� UI ����
                    if (!drawer.isOpen) // ������ ���� ���� ��
                    {
                        openUI.SetActive(true);  // OpenUI Ȱ��ȭ
                        closeUI.SetActive(false); // CloseUI ��Ȱ��ȭ
                    }
                    else // ������ ���� ���� ��
                    {
                        openUI.SetActive(false);  // OpenUI ��Ȱ��ȭ
                        closeUI.SetActive(true);   // CloseUI Ȱ��ȭ
                    }
                }
            }
            else
            {
                // ������ �ƴ� ��� UI ��Ȱ��ȭ
                closeUI.SetActive(false);
                openUI.SetActive(false);
            }
        }
        else
        {
            // ����ĳ��Ʈ�� �������� ���� ��� ��� UI ��Ȱ��ȭ
            openUI.SetActive(false);
            closeUI.SetActive(false);
        }
    }

    [PunRPC] // RPC �޼���� ����
    public void ToggleDrawerRPC()
    {
        StartCoroutine(ToggleDrawer());
    }

    public IEnumerator ToggleDrawer()
    {
        if (!enabled || audioSource == null) yield break;

        isMoving = true;
        Vector3 targetPosition = isOpen ? closedPosition : openPosition; // ����/���� ��ġ ����

        if (audioSource != null)
        {
            AudioClip currentClip = isOpen ? closeSound : openSound;
            if (currentClip != null)
            {
                audioSource.clip = currentClip;
                audioSource.loop = false;
                audioSource.Play();
            }
        }

        float elapsedTime = 0;
        Vector3 initialPosition = transform.localPosition;

        while (elapsedTime < moveDuration)
        {
            transform.localPosition = Vector3.Lerp(initialPosition, targetPosition, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = targetPosition; // ���� ��ġ ����

        if (audioSource != null)
        {
            audioSource.Stop();
        }

        isOpen = !isOpen; // ���� ������Ʈ
        isMoving = false;
    }
}

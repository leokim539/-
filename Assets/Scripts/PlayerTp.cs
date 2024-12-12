using Photon.Pun;
using UnityEngine;

public class PlayerTp : MonoBehaviourPunCallbacks
{
    [Header("�����̵���ġ")] 
    public Transform portal1; // ��Ż A ��ġ
    public Transform portal2;
    public float stayDuration = 1f; // �ӹ��� �ð�
    private bool isInZone = false; // �÷��̾ Ư�� ������ �ִ��� ����
    private float stayTimer = 0f; // �ӹ��� �ð� ī����
    private PhotonView playerPhotonView; // �����̵��� �÷��̾��� PhotonView

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInZone = true;
            stayTimer = 0f;
            playerPhotonView = other.GetComponent<PhotonView>(); // �浹�� �÷��̾��� PhotonView ����
            Debug.Log("Player entered the teleport zone");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInZone = false;
            stayTimer = 0f;
            playerPhotonView = null; // �÷��̾ ������ PhotonView �ʱ�ȭ
            Debug.Log("Player exited the teleport zone");
        }
    }

    private void Update()
    {
        if (isInZone && playerPhotonView != null)
        {
            stayTimer += Time.deltaTime;

            if (stayTimer >= stayDuration)
            {
                TeleportPlayerToLocation();
            }
        }
    }

    private void TeleportPlayerToLocation()
    {
        if (playerPhotonView == null)
        {
            Debug.LogError("Player's PhotonView is missing!");
            return;
        }

        // ���� ��ġ�� ���� ��Ż A �Ǵ� B�� �����̵�
        Vector3 targetPosition = Vector3.Distance(playerPhotonView.transform.position, portal1.position) < 1f ? portal2.position : portal1.position;

        Debug.Log("Calling RPC to teleport player to: " + targetPosition);
        playerPhotonView.RPC("TeleportTo", RpcTarget.All, targetPosition);

        // �̵� �� ���� �ʱ�ȭ
        isInZone = false;
        stayTimer = 0f;
    }
}
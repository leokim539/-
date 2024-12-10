using Photon.Pun;
using UnityEngine;

public class PlayerTp : MonoBehaviourPunCallbacks
{
    [Header("�����̵���ġ")] 
    public Transform portal1; // ��Ż A ��ġ
    public Transform portal2;
    public float stayDuration = 2f; // �ӹ��� �ð�
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
            if (playerPhotonView != null)
            {
                // ���� ��ġ�� ���� ��Ż A �Ǵ� B�� �����̵�
                if (Vector3.Distance(other.transform.position, portal1.position) < 1f)
                {
                    playerPhotonView.RPC("TeleportTo", RpcTarget.All, portal2.position);
                }
                else if (Vector3.Distance(other.transform.position, portal2.position) < 1f)
                {
                    playerPhotonView.RPC("TeleportTo", RpcTarget.All, portal1.position);
                }
            }
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

        Debug.Log("Calling RPC to teleport player");
        playerPhotonView.RPC("Teleport", RpcTarget.All);
    }
}
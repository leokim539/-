using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaItem : MonoBehaviour
{
    public GameObject Player;
    private FirstPersonController firstPersonController;

    void Start()
    {
        firstPersonController = Player.GetComponent<FirstPersonController>();
    }
    void OnTriggerEnter(Collider player)
    {
        if (player.CompareTag("Player"))
        {
            firstPersonController.canMove = false;
            StartCoroutine(BananaItemDelay(5f));
        }
        PhotonNetwork.Destroy(gameObject); // 아이템을 파괴
    }
    private IEnumerator BananaItemDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        firstPersonController.canMove = true;
    }
}

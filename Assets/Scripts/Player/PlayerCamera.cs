using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCamera : MonoBehaviour
{
    public Camera playerCamera;  // �÷��̾� ī�޶� �ν����Ϳ��� �Ҵ�
    public float mouseSensitivity = 2f;  // ���콺 ����
    public Transform player;
    private float verticalRotation = 0f;
    private float horizontalRotation = 0f;

    void Start()
    {
        // playerCamera�� null�̸� Camera.main���� �Ҵ� �õ�
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    void Update()
    {
        // ���콺 �Է� �ޱ�
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // X�� ȸ�� (�¿� ȸ��)
        horizontalRotation += mouseX;
        transform.localRotation = Quaternion.Euler(0f, horizontalRotation, 0f);

        // Y�� ȸ�� (���� ȸ��)
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        // ī�޶��� ���� ȸ�� ����
        if (playerCamera != null)
        {
            playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        }
    }
}
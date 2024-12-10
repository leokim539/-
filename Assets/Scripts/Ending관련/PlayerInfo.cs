using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInfo
{
    public string playerName;
    public int totalTrashCount;

    public PlayerInfo(string name, int trashCount) // �� ���� �Ű������� �޴� ������
    {
        playerName = name;
        totalTrashCount = trashCount;
    }
}

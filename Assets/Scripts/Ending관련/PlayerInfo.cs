using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInfo
{
    public string playerName;
    public int totalTrashCount;

    public PlayerInfo(string name, int trashCount) // 두 개의 매개변수를 받는 생성자
    {
        playerName = name;
        totalTrashCount = trashCount;
    }
}

using UnityEngine;

public class TrashCount : MonoBehaviour
{
    [SerializeField] private int totalTrashCount;

    public void AddTrash(int count)
    {
        if (count > 0)
        {
            totalTrashCount += count;
        }
    }

    public int GetTotalTrashCount() // �� �޼���� public���� ���ǵǾ�� �մϴ�.
    {
        return totalTrashCount;
    }

    public void ResetTrashCount()
    {
        totalTrashCount = 0;
    }

    public void LogTrashCount()
    {
        Debug.Log($"Total Trash Count: {totalTrashCount}");
    }
}

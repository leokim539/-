using UnityEngine;

public class Exit : MonoBehaviour
{
    // Settings Panel
    public GameObject settingsPanel;

    // ��Ȱ��ȭ �Լ�
    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Settings Panel is not assigned!");
        }
    }
}

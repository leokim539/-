using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingScene : MonoBehaviour
{
    public void LoadEndingScene()
    {
        // "Ending" ���� �ε�
        SceneManager.LoadScene("Ending");
    }
}

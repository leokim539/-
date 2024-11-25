using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Loading : MonoBehaviour
{
    public GameObject loadingUI; // �ε� ȭ�� UI
    public Slider progressBar;   // �ε� ���� ��
    public float loadingTime = 2f; // �����̴��� �����ϴ� �� �ɸ� �ð� (��)

    // ��ư Ŭ�� �� ȣ��
    public void StartLoadingScene(string sceneName)
    {
        if (loadingUI != null)
            loadingUI.SetActive(true); // �ε� UI Ȱ��ȭ

        StartCoroutine(LoadSceneWithProgress(sceneName)); // �ε� �ڷ�ƾ ����
    }

    private IEnumerator LoadSceneWithProgress(string sceneName)
    {
        float elapsedTime = 0f; // ��� �ð�
        progressBar.value = 0f; // �����̴� �ʱ�ȭ

        // 2�� ���� �����̴� ����
        while (elapsedTime < loadingTime)
        {
            elapsedTime += Time.deltaTime;
            progressBar.value = Mathf.Clamp01(elapsedTime / loadingTime); // �����̴� �� ����
            yield return null; // ���� �����ӱ��� ���
        }

        // �����̴��� ���� �� �� �� ��ȯ
        SceneManager.LoadScene(sceneName);
    }
}

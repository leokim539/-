using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public Image fadeImage; // ���̵� �̹���
    public float fadeDuration = 2f; // ���̵� ���� �ð� (2�ʷ� ����)

    private void Start()
    {
        // ���� �� ���̵� ��
        StartCoroutine(FadeIn());
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName)); // ���̵� �ƿ� �� �� �ε�
    }

    private IEnumerator FadeOut()
    {
        float time = 0;
        fadeImage.gameObject.SetActive(true); // ���̵� �̹����� Ȱ��ȭ

        // ���̵� �ƿ� (���� ȭ�鿡�� ��ο���)
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, Mathf.Clamp01(time / fadeDuration));
            yield return null;
        }
    }

    private IEnumerator FadeIn()
    {
        float time = 0;
        fadeImage.gameObject.SetActive(true); // ���̵� �̹����� Ȱ��ȭ
        fadeImage.color = new Color(0, 0, 0, 1); // ������ �� ������ ��ο� ���·� ����

        // ���̵� �� (��ο� ȭ�鿡�� �����)
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, Mathf.Clamp01(1 - (time / fadeDuration)));
            yield return null;
        }

        fadeImage.gameObject.SetActive(false); // ���̵� �� �� �̹��� ��Ȱ��ȭ
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        // ���̵� �ƿ�
        yield return StartCoroutine(FadeOut());

        // �� �ε�
        SceneManager.LoadScene(sceneName);

        // ���̵� ��
        yield return StartCoroutine(FadeIn());
    }
}

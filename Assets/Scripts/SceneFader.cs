using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    public Image fadeImage; // 페이드 이미지
    public float fadeDuration = 2f; // 페이드 지속 시간 (2초로 설정)

    private void Start()
    {
        // 시작 시 페이드 인
        StartCoroutine(FadeIn());
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOutAndLoad(sceneName)); // 페이드 아웃 후 씬 로드
    }

    private IEnumerator FadeOut()
    {
        float time = 0;
        fadeImage.gameObject.SetActive(true); // 페이드 이미지를 활성화

        // 페이드 아웃 (밝은 화면에서 어두워짐)
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
        fadeImage.gameObject.SetActive(true); // 페이드 이미지를 활성화
        fadeImage.color = new Color(0, 0, 0, 1); // 시작할 때 완전히 어두운 상태로 설정

        // 페이드 인 (어두운 화면에서 밝아짐)
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadeImage.color = new Color(0, 0, 0, Mathf.Clamp01(1 - (time / fadeDuration)));
            yield return null;
        }

        fadeImage.gameObject.SetActive(false); // 페이드 인 후 이미지 비활성화
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        // 페이드 아웃
        yield return StartCoroutine(FadeOut());

        // 씬 로드
        SceneManager.LoadScene(sceneName);

        // 페이드 인
        yield return StartCoroutine(FadeIn());
    }
}

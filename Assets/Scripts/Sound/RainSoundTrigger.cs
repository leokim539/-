using UnityEngine;
using System.Collections;

public class RainSoundManager : MonoBehaviour
{
    public AudioSource rainSound;       // �ϳ��� ���Ҹ� ����� �ҽ�
    public float nearVolume = 1.0f;     // â�� ��ó������ �ִ� ����
    public float farVolume = 0.05f;      // �⺻ ������ ���� ����
    public float fadeDuration = 1.0f;   // ���� ��ȭ�� �ɸ��� �ð� (�� ����)

    private Coroutine fadeCoroutine;    // ���� ���� ���� ���� ���� �ڷ�ƾ

    private void Start()
    {
        // ���Ҹ��� �ʱ� ������ ���� ����
        if (rainSound != null)
        {
            rainSound.volume = farVolume;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // �÷��̾ "RainZone" �±��� �ݶ��̴��� ���� �� ������ ������ ����
        if (other.CompareTag("RainZone") && rainSound != null)
        {
            // ���� ���� �ڷ�ƾ�� ������ ����
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

            // �ڷ�ƾ�� ���� ������ ���������� nearVolume���� ����
            fadeCoroutine = StartCoroutine(FadeVolume(nearVolume));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // �÷��̾ "RainZone" �±��� �ݶ��̴����� ������ �� ������ ������ ����
        if (other.CompareTag("RainZone") && rainSound != null)
        {
            // ���� ���� �ڷ�ƾ�� ������ ����
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);

            // �ڷ�ƾ�� ���� ������ ���������� farVolume���� ����
            fadeCoroutine = StartCoroutine(FadeVolume(farVolume));
        }
    }

    private IEnumerator FadeVolume(float targetVolume)
    {
        float startVolume = rainSound.volume;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            // �ð��� ���� ������ ���������� ����
            rainSound.volume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ���� ������ ��ǥ ������ ����
        rainSound.volume = targetVolume;
    }
}
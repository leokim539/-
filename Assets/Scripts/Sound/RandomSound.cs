using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomSound : MonoBehaviour
{
    public AudioSource audioSource;             // �ϳ��� AudioSource
    public AudioClip[] sounds;                  // ����� AudioClip �迭
    public List<Vector2> intervalRanges;        // ���� ���� ���� ����Ʈ (��: 5~10��, 15~20��, 25~30��)

    private List<AudioClip> remainingSounds;    // �����ִ� ���� ���

    private void Start()
    {
        remainingSounds = new List<AudioClip>(sounds);  // ���带 �ʱ�ȭ
        StartCoroutine(PlaySoundWithIntervals());
    }

    private IEnumerator PlaySoundWithIntervals()
    {
        // �� �ð��븶�� ���������� ���带 ���
        foreach (Vector2 intervalRange in intervalRanges)
        {
            // �ش� �ð��뿡�� �ϳ��� ���� �������� ���
            float delay = Random.Range(intervalRange.x, intervalRange.y); // �ð��� ���� �� ���� ��� �ð�
            yield return new WaitForSeconds(delay);  // �� �ð���ŭ ���

            // ����� ���带 �������� ����
            if (remainingSounds.Count > 0)
            {
                int randomIndex = Random.Range(0, remainingSounds.Count);  // �������� ���� ����
                AudioClip selectedSound = remainingSounds[randomIndex];

                // ���带 AudioSource�� �Ҵ��ϰ� ���
                audioSource.clip = selectedSound;
                audioSource.Play();

                // ����� ����� ��Ͽ��� ���� (�ߺ� ��� ����)
                remainingSounds.RemoveAt(randomIndex);
            }
            else
            {
                Debug.Log("��� ���尡 ����Ǿ����ϴ�.");
                break;
            }
        }
    }
}
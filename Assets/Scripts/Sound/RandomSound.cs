using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomSound : MonoBehaviour
{
    public AudioSource audioSource;             // 하나의 AudioSource
    public AudioClip[] sounds;                  // 재생할 AudioClip 배열
    public List<Vector2> intervalRanges;        // 여러 간격 범위 리스트 (예: 5~10초, 15~20초, 25~30초)

    private List<AudioClip> remainingSounds;    // 남아있는 사운드 목록

    private void Start()
    {
        remainingSounds = new List<AudioClip>(sounds);  // 사운드를 초기화
        StartCoroutine(PlaySoundWithIntervals());
    }

    private IEnumerator PlaySoundWithIntervals()
    {
        // 각 시간대마다 순차적으로 사운드를 재생
        foreach (Vector2 intervalRange in intervalRanges)
        {
            // 해당 시간대에서 하나의 사운드 랜덤으로 재생
            float delay = Random.Range(intervalRange.x, intervalRange.y); // 시간대 범위 내 랜덤 대기 시간
            yield return new WaitForSeconds(delay);  // 그 시간만큼 대기

            // 재생할 사운드를 랜덤으로 선택
            if (remainingSounds.Count > 0)
            {
                int randomIndex = Random.Range(0, remainingSounds.Count);  // 무작위로 사운드 선택
                AudioClip selectedSound = remainingSounds[randomIndex];

                // 사운드를 AudioSource에 할당하고 재생
                audioSource.clip = selectedSound;
                audioSource.Play();

                // 재생한 사운드는 목록에서 제거 (중복 재생 방지)
                remainingSounds.RemoveAt(randomIndex);
            }
            else
            {
                Debug.Log("모든 사운드가 재생되었습니다.");
                break;
            }
        }
    }
}
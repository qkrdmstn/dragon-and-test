using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWaveEffect : MonoBehaviour
{
    private Coroutine shockWaveCoroutine;
    private Material material;
    private static int waveDistanceFromCenter = Shader.PropertyToID("_WaveDistanceFromCenter");
    public bool isScale = true;

    private void Awake()
    {
        material = GetComponent<SpriteRenderer>().material;
    }

    public void CallShockWave(float shockWaveTime)
    {
        shockWaveCoroutine = StartCoroutine(ShockWaveAction(-0.1f, 0.12f, shockWaveTime));
    }

    private IEnumerator ShockWaveAction(float startPos, float endPos, float shockWaveTime)
    {
        material.SetFloat(waveDistanceFromCenter, startPos);
        float lerpedAmount = 0f;

        float elapsedTime = 0f;
        while (elapsedTime < shockWaveTime)
        {
            if(isScale)
                elapsedTime += Time.deltaTime;
            else
                elapsedTime += Time.unscaledDeltaTime;

            lerpedAmount = Mathf.Lerp(startPos, endPos, (elapsedTime/shockWaveTime));
            material.SetFloat(waveDistanceFromCenter, lerpedAmount);

            yield return null;
        }
        material.SetFloat(waveDistanceFromCenter, 1.0f);
    }
}

using System;
using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public CinemachineBasicMultiChannelPerlin vcam;

    private void Start()
    {
        StartCoroutine(Shake(1,1));
    }

    public IEnumerator Shake(float duration, float amplitude)
    {
        
        vcam.AmplitudeGain = amplitude;

        yield return new WaitForSeconds(duration);

        vcam.AmplitudeGain = 0f;
    }
}
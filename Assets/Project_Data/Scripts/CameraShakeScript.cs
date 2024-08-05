using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeScript : MonoBehaviour
{
    private float Float_ShakeDuration = 0.5f;
    private float Float_ShakeMagnitude = 0.7f;
    private float Float_DecreaseFactor = 1.0f;

    private Vector3 Vec3_OriginalPos;

    private void Start()
    {
        Vec3_OriginalPos = transform.localPosition;
    }

    public void CameraShake()
    {
        StartCoroutine("ShakeCoroutine");
    }

    private IEnumerator ShakeCoroutine()
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < Float_ShakeDuration)
        {
            transform.localPosition = Vec3_OriginalPos + Random.insideUnitSphere * Float_ShakeMagnitude;
            Float_ShakeMagnitude *= Float_DecreaseFactor;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = Vec3_OriginalPos;
    }
}

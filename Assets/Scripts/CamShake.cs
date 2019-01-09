using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamShake : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void CameraShake (float intensity, float duration)
    {
        StartCoroutine(Shaker(intensity, duration));
    }

    IEnumerator Shaker(float intensity, float duration)
    {
        float shakeTime = 0;
        while (shakeTime < duration)
        {
            Vector3 randomPos = new Vector3(Mathf.Clamp(Random.Range(-0.2f, 0.2f) * intensity,0.1f,2f), Mathf.Clamp(Random.Range(-0.2f, 0.2f) * intensity, 0.1f, 2f), 0);            
            transform.localPosition += (randomPos);
            yield return null;
            shakeTime += Time.deltaTime;
            transform.localPosition -= (randomPos);
            yield return null;
            shakeTime += Time.deltaTime;
        }

        //float shakeTime = 0;
        //Vector3 originalPos = transform.position;
        //while (shakeTime < duration)
        //{
        //    Vector3 randomPos = new Vector3(Random.Range(-0.2f, 0.2f) * intensity, Random.Range(-0.2f, 0.2f) * intensity, originalPos.z);
        //    float transitionTime = 0f;
        //    do
        //    {
        //        transitionTime += Time.deltaTime/duration;
        //        transform.localPosition = Vector3.Lerp(transform.localPosition, randomPos, transitionTime);
        //        yield return null;
        //        shakeTime += Time.deltaTime;
        //    } while (transform.localPosition != randomPos);

        //    transitionTime = 0f;
        //    do
        //    {
        //        transitionTime += Time.deltaTime/duration;
        //        transform.localPosition = Vector3.Lerp(transform.localPosition, originalPos, Time.deltaTime);
        //        yield return null;
        //        shakeTime += Time.deltaTime;
        //    } while (transform.localPosition != originalPos);

        //}
    }
}

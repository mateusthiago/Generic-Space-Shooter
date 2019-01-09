using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class infoCanvas : MonoBehaviour {

    [SerializeField] Image weakSpot;
    [SerializeField] Transform shield;
    [SerializeField] Transform bullet;
    float growth = 100;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        weakSpot.CrossFadeAlpha(Mathf.PingPong(Time.unscaledTime, 1), Time.unscaledDeltaTime, true);

        shield.transform.localScale += new Vector3(growth, growth, 0) * Time.unscaledDeltaTime;
        bullet.transform.localScale += new Vector3(growth, growth, 0) * Time.unscaledDeltaTime;
        if (shield.transform.localScale.x >= 180) growth *= -1;
        if (shield.transform.localScale.x <= 100) growth *= -1;
    }
}

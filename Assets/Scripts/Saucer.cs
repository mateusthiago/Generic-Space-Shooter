using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saucer : MonoBehaviour {

    [SerializeField] float torque = 10f;
    Rigidbody2D rb;
    

	void Start ()
    {
        rb = GetComponent<Rigidbody2D>();        
        rb.AddTorque(torque);
	}

    private void Update()
    {
        // if (Input.GetKeyDown("space")) StartCoroutine(GetComponent<Enemy>().SaucerFire());   FOR TESTING
    }

}

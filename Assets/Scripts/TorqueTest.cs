using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TorqueTest : MonoBehaviour {

    public float force;
    Rigidbody2D rb;

	void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update ()
    {
       rb.AddTorque(Input.GetAxis("Horizontal") * force);
	}

    private void OnGUI()
    {
       GUI.Label(new Rect(40, 40, 100, 100), rb.angularVelocity.ToString());
    }
}

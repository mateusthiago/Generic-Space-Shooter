using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail : MonoBehaviour {

    GameObject player;

	void Start ()
    {
        player = FindObjectOfType<Player>().gameObject;
	}	
	
	void Update ()
    {
        transform.position = player.transform.position;
	}

    public void DestroyTrail()
    {
        Destroy(gameObject);
    }
}

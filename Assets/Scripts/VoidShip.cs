using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidShip : MonoBehaviour {

    Transform player;
    public bool isFiring = false;    

	void Start ()
    {
        player = FindObjectOfType<Player>().transform;
	}
	
	
	void Update ()
    {
        if (!isFiring && player != null)
        {
            //Vector3 relativePos = player.position;
            Vector2 direction = new Vector2(player.position.x - transform.position.x, player.position.y - transform.position.y);

            transform.up = -direction;
        }        
    }
}

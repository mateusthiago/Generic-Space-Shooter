using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPickUp : MonoBehaviour {


    [SerializeField] AudioClip pickupSFX;
    [SerializeField] int score;
    GameObject player;
    Rigidbody2D rb;


	// Use this for initialization
	void Start ()
    {
        player = FindObjectOfType<Player>().gameObject;
        rb = GetComponent<Rigidbody2D>();
        int[] lefOrRight = { -2, +2 };
        rb.velocity = new Vector2(lefOrRight[Random.Range(0, 1)], -1);
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (transform.position.x <= -5.4)
            rb.velocity = new Vector2(+2, -1);
        else if (transform.position.x >= 5.4)
            rb.velocity = new Vector2(-2, -1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            player.GetComponent<Player>().PickUpShield();
            FindObjectOfType<GameSession>().AddScore(score);
            AudioSource.PlayClipAtPoint(pickupSFX, Camera.main.transform.position, 0.5f);
            Destroy(this.gameObject);
        }
        
    }
}

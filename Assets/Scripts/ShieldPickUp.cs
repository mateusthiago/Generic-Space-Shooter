using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPickUp : MonoBehaviour {


    [SerializeField] AudioClip pickupSFX;
    [SerializeField] int score;
    GameObject player;
    Rigidbody2D rb;
    float grow = 0.4f;


    void Start()
    {
        if (FindObjectOfType<Player>() != null) player = FindObjectOfType<Player>().gameObject;
        else return;
        rb = GetComponent<Rigidbody2D>();
        int[] lefOrRight = { -2, +2 };
        rb.velocity = new Vector2(lefOrRight[Random.Range(0, 1)], -3);
    }

    // Update is called once per frame
    void Update ()
    {        

        if (transform.position.x <= -5.4)
            rb.velocity = new Vector2(+2, -3);
        else if (transform.position.x >= 5.4)
            rb.velocity = new Vector2(-2, -3);

        transform.localScale += new Vector3(grow, grow, 0) * Time.deltaTime;

        if (transform.localScale.x >= 2f) grow *= -1;
        else if (transform.localScale.x <= 1.5f) grow *= -1;
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            player.GetComponent<Player>().PickUpShield();
            FindObjectOfType<GameSession>().AddScore(score);
            AudioSource.PlayClipAtPoint(pickupSFX, Camera.main.transform.position, 0.9f);
            Destroy(this.gameObject);
        }
        
    }
}

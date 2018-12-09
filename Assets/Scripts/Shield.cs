using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour {

    [SerializeField] GameObject hitFX;
    [SerializeField] AudioClip shieldOffSFX;

    GameObject player;

	void Start ()
    {
        player = FindObjectOfType<Player>().gameObject;
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.position = player.transform.position;
	}

    private void OnDestroy()
    {
        AudioSource.PlayClipAtPoint(shieldOffSFX, Camera.main.transform.position, 0.5f);
        //var newHitFX = Instantiate(hitFX, new Vector2(player.transform.position.x, player.transform.position.y + 0.5f), Quaternion.Euler(90, 0, 0));
        var newHitFX = Instantiate(hitFX, player.GetComponent<Transform>());
        Destroy(newHitFX, 0.5f);
    }    
}

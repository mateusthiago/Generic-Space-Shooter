﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shredder : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision) //ontrigger
    {
        if (collision.gameObject.transform.parent != null)
            Destroy(collision.gameObject.transform.parent.gameObject);
        else
            Destroy(collision.gameObject);
    }

}

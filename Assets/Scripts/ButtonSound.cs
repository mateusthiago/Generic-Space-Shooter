using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour, ISelectHandler
{
	public void OnSelect(BaseEventData eventData)
    {
        
            GetComponent<AudioSource>().Play();            
    }
}

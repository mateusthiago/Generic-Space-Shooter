using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour, ISelectHandler, IDeselectHandler
{
	public void OnSelect(BaseEventData eventData)
    {        
        GetComponent<AudioSource>().Play();
        transform.localScale = new Vector3(1.5f, 1.5f, 1);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        transform.localScale = new Vector3(1, 1, 1);
    }

    void OnMouseEnter()
    {
        GetComponent<AudioSource>().Play();
        transform.localScale = new Vector3(1.5f, 1.5f, 1);        
    }

    void OnMouseExit()
    {
        transform.localScale = new Vector3(1, 1, 1);
        EventSystem.current.SetSelectedGameObject(null);
    }
}

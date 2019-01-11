using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour {

    [SerializeField] float scrollSpeed = 0.05f;
    [SerializeField] GameObject backgroundStars;
    [SerializeField] GameObject foregroundStars;
    [SerializeField] GameObject bgStars2;
    [SerializeField] GameObject fgStars2;
    [SerializeField] GameObject bgStars3;
    //[SerializeField] GameObject bgBgStars3;
    [SerializeField] GameObject fgStars3;
    Material myMaterial;
    Vector2 offset;

    public ParticleSystem.MainModule bgStars;
    public ParticleSystem.MainModule fgStars;


    private void Start()
    {
        myMaterial = gameObject.GetComponent<Renderer>().material;
        offset = new Vector2(0f, scrollSpeed);
        bgStars = backgroundStars.GetComponent<ParticleSystem>().main;
        fgStars = foregroundStars.GetComponent<ParticleSystem>().main;
    }

    void Update ()
    {
       myMaterial.mainTextureOffset += offset * Time.deltaTime; //scrolling background if a material is set in inspector
	}

    public IEnumerator GameIntroScroll()
    {
        Vector2 introScroll = new Vector2(0f, scrollSpeed);
        for (float i = 0.05f; i <= 1f; i += 0.05f)
        {
            bgStars.simulationSpeed += i;
            fgStars.simulationSpeed += i;
            introScroll = new Vector2(0f, i);
            offset = introScroll;            
            yield return new WaitForSeconds(0.1f);
        }

    }

    public IEnumerator NormalScroll()
    {
        Vector2 introScroll = new Vector2(0f, scrollSpeed);
        for (float i = 1f; i >= 0.05f; i -= 0.05f)
        {
            bgStars.simulationSpeed += 0.05f - i;
            fgStars.simulationSpeed += 0.05f - i;
            introScroll = new Vector2(0f, i);
            offset = introScroll;            
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void SectorStars(int sector)
    {
        switch (sector)
        {
            case 1:
                backgroundStars.SetActive(true);
                foregroundStars.SetActive(true);
                bgStars2.SetActive(false);
                fgStars2.SetActive(false);
                bgStars3.SetActive(false);
                //bgBgStars3.SetActive(false);
                fgStars3.SetActive(false);
                break;
            case 2:
                backgroundStars.SetActive(false);
                foregroundStars.SetActive(false);
                bgStars2.SetActive(true);
                fgStars2.SetActive(true);
                bgStars3.SetActive(false);
                //bgBgStars3.SetActive(false);
                fgStars3.SetActive(false);
                break;
            case 3:
            case 4:
                backgroundStars.SetActive(false);
                foregroundStars.SetActive(false);
                bgStars2.SetActive(false);
                fgStars2.SetActive(false);
                bgStars3.SetActive(true);
                //bgBgStars3.SetActive(true);
                fgStars3.SetActive(true);
                break;


        }
            

        
    }
}

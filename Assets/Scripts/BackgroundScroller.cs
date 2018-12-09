using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour {

    [SerializeField] float scrollSpeed = 0.05f;
    [SerializeField] GameObject backgroundStars;
    [SerializeField] GameObject foregroundStars;
    Material myMaterial;
    Vector2 offset;

    

    private void Start()
    {
        myMaterial = gameObject.GetComponent<Renderer>().material;
        offset = new Vector2(0f, scrollSpeed);

        
    }

    void Update ()
    {
        myMaterial.mainTextureOffset += offset * Time.deltaTime;
	}

    public IEnumerator GameIntroScroll()
    {
        var bgStars = backgroundStars.GetComponent<ParticleSystem>().main;
        var fgStars = foregroundStars.GetComponent<ParticleSystem>().main;
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
        var bgStars = backgroundStars.GetComponent<ParticleSystem>().main;
        var fgStars = foregroundStars.GetComponent<ParticleSystem>().main;
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
}

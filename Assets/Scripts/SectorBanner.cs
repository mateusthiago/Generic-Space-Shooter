using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SectorBanner : MonoBehaviour
{

    [SerializeField] GameObject bannerImage;
    [SerializeField] TextMeshProUGUI bannerText;    
    Vector3 startPosition = new Vector3(-540, 0, 0);
    Vector3 middlePosition = new Vector3(540, 0, 0);
    Vector3 endPosition = new Vector3(1620, 0 ,0);    

    float timeOfTravel = 1;
    float currentTime = 0;
    float t;    


    //public void CallSectorBanner(int sector, float wait)
    //{
    //    if (sector == 4)
    //    {
    //        StartCoroutine(ShowDangerBanner());
    //        return;
    //    }
    //    StartCoroutine(ShowSectorBanner(sector, wait));
    //}

    public IEnumerator ShowSectorBanner(int sector, float wait)
    {
        yield return new WaitForSeconds(wait);
        RectTransform banner = bannerImage.GetComponent<RectTransform>();
        bannerText.text = "SECTOR " + sector;
        GetComponent<Animator>().enabled = true;
        currentTime = 0;

        while (currentTime <= timeOfTravel)
        {
            currentTime += Time.deltaTime;
            t = currentTime / timeOfTravel;
            t = t * t * t * (t * (6f * t - 15f) + 10f); // smoothstep formula - https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/

            banner.anchoredPosition = Vector3.Lerp(startPosition, middlePosition, t);
            yield return null;
        }

        yield return new WaitForSeconds(2);
        currentTime = 0;
        
        while (currentTime <= timeOfTravel)
        {
            currentTime += Time.deltaTime;
            t = currentTime / timeOfTravel;
            t = t * t * t * (t * (6f * t - 15f) + 10f);

            banner.anchoredPosition = Vector3.Lerp(middlePosition, endPosition, t);
            yield return null;
        }

        banner.anchoredPosition = startPosition;
        GetComponent<Animator>().enabled = false;
    }

    public IEnumerator ShowDangerBanner()
    {
        yield return new WaitForSeconds(1);
        RectTransform banner = bannerImage.GetComponent<RectTransform>();
        bannerText.text = "DANGER";
        GetComponent<Animator>().SetBool("danger", true);
        GetComponent<Animator>().enabled = true;
        currentTime = 0;

        StartCoroutine(PlayDangerSound());

        while (currentTime <= timeOfTravel)
        {
            currentTime += Time.deltaTime;
            t = currentTime / timeOfTravel;
            t = t * t * t * (t * (6f * t - 15f) + 10f); // smoothstep formula - https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/

            banner.anchoredPosition = Vector3.Lerp(startPosition, middlePosition, t);
            yield return null;
        }

        yield return new WaitForSeconds(2f);
        currentTime = 0;

        while (currentTime <= timeOfTravel)
        {
            currentTime += Time.deltaTime;
            t = currentTime / timeOfTravel;
            t = t * t * t * (t * (6f * t - 15f) + 10f);

            banner.anchoredPosition = Vector3.Lerp(middlePosition, endPosition, t);
            yield return null;
        }

        banner.anchoredPosition = startPosition;
        GetComponent<Animator>().enabled = false;
        GetComponent<Animator>().SetBool("danger", false);
    }

    private IEnumerator PlayDangerSound()
    {
        for (int i = 0; i < 3; i++)
        {
            GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(1f);
        }
    }
}

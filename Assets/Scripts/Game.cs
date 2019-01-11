using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class Game : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    GameObject player;
    [SerializeField] GameObject speedParticleFX;
    [SerializeField] GameObject enemySpawner;
    [SerializeField] GameObject titleCanvas;
    [SerializeField] GameObject infoCanvas;
    [SerializeField] GameObject gameCanvas;
    [SerializeField] GameObject startButton;
    [SerializeField] GameObject infoButton;
    [SerializeField] GameObject quitButton;
    [SerializeField] GameObject mobilePauseButton;
    [SerializeField] Button backButton;
    [SerializeField] TextMeshProUGUI startButtontText;
    [SerializeField] TextMeshProUGUI sectorBanner;
    [SerializeField] AudioClip pauseSFX;
    [SerializeField] AudioClip introLaunchSFX;
    [SerializeField] AudioClip endingBoostSFX;
    [SerializeField] AudioClip endingWarpSFX;
    [SerializeField] Image fadeScreen;
    [SerializeField] TextMeshProUGUI endGameText;
    [SerializeField] public bool skipIntro;
    [SerializeField] int sector = 1;
    [SerializeField] bool startingWaveSetInEnemySpawner = false;
    [SerializeField] bool dontShowMenu = false;

    bool infoScreen = false;
    public bool gameIsPaused = false;
    bool gameIsOn = false;

    public static Game game;

    public AnimationCurve debugCurve;
	
	void Start ()
    {
        game = this;
        if (SystemInfo.deviceType != DeviceType.Handheld) mobilePauseButton.SetActive(false);
        if (Application.platform == RuntimePlatform.WebGLPlayer) quitButton.SetActive(false);
        if (!dontShowMenu) ShowMenu();
        if (FindObjectOfType<GameSession>().GetHiScore() > 0) skipIntro = true;
        sector = FindObjectOfType<GameSession>().GetLastSector();
        fadeScreen.CrossFadeAlpha(0, 0, false);
        endGameText.CrossFadeAlpha(0, 0, false);

        // set the desired aspect ratio (the values in this example are
        // hard-coded for 9:16, but you could make them into public
        // variables instead so you can set them at design time)
        float targetaspect = 9.0f / 16.0f;

        // determine the game window's current aspect ratio
        float windowaspect = (float)Screen.width / (float)Screen.height;

        // current viewport height should be scaled by this amount
        float scaleheight = windowaspect / targetaspect;

        // obtain camera component so we can modify its viewport
        Camera camera = Camera.main;

        // if scaled height is less than current height, add letterbox
        if (scaleheight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;

            camera.rect = rect;
        }
        else // add pillarbox
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = Camera.main.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }
    }

    private void ShowMenu()
    {
        EventSystem.current.SetSelectedGameObject(startButton);
        startButton.GetComponent<Button>().OnSelect(null);
        titleCanvas.SetActive(true);
        GameObject.Find("HiScoreDisplay").GetComponent<TextMeshProUGUI>().text = "hiscore \n" + FindObjectOfType<GameSession>().GetHiScore();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel") && gameIsOn)
        {
            PauseGame();
        }
    }

    public void ShowInfo()
    {
        infoScreen = true;
        infoButton.GetComponent<ButtonSound>().OnDeselect(null);
        titleCanvas.SetActive(false);        
        infoCanvas.SetActive(true);
        EventSystem.current.SetSelectedGameObject(backButton.gameObject);
        backButton.OnSelect(null);
    }

    public void backToTitle()
    {     
        infoCanvas.SetActive(false);
        titleCanvas.SetActive(true);
        EventSystem.current.SetSelectedGameObject(startButton);
        startButton.GetComponent<Button>().OnSelect(null);
        infoScreen = false;
    }



    public void StartGame()
    {
        if (!gameIsOn)
        {
            StartCoroutine(GameIntro());            
        }
        else if (gameIsPaused && gameIsOn)
        {
            Time.timeScale = 1;
            AudioSource.PlayClipAtPoint(pauseSFX, Camera.main.transform.position, 0.2f);
            titleCanvas.SetActive(false);
            gameIsPaused = false;
            fadeScreen.CrossFadeAlpha(0, 0, true);
        }

    }

    public void PauseGame()
    {
        if (gameIsOn)
        {
            if (!gameIsPaused)
            {
                fadeScreen.CrossFadeAlpha(0.5f, 0, true);
                AudioSource.PlayClipAtPoint(pauseSFX, Camera.main.transform.position, 0.2f);
                Time.timeScale = 0;                
                EventSystem.current.SetSelectedGameObject(startButton);
                startButton.GetComponent<Button>().OnSelect(null);
                startButtontText.text = "resume";
                titleCanvas.SetActive(true);
                gameIsPaused = true;
            }
            else if (gameIsPaused && !infoScreen)
            {
                StartGame();
                AudioSource.PlayClipAtPoint(pauseSFX, Camera.main.transform.position, 0.2f);
            }
        }
        
        
    }

    private IEnumerator GameIntro()
    {
        if (!skipIntro && sector == 1)
        {
            titleCanvas.SetActive(false);

            yield return (StartCoroutine(FindObjectOfType<BackgroundScroller>().GetComponent<BackgroundScroller>().GameIntroScroll()));

            yield return new WaitForSeconds(1f);

            player = Instantiate(playerPrefab, new Vector3(0, -15f, -1), Quaternion.identity);
            GameObject newTrail = Instantiate(speedParticleFX, player.transform);
            AudioSource.PlayClipAtPoint(introLaunchSFX, Camera.main.transform.position, 0.6f);            
            StartCoroutine(LerpPlayerToPosition(0, 3, 2, true));
            yield return new WaitForSeconds(1.8f);
            newTrail.GetComponent<ParticleSystem>().Stop();        
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(FindObjectOfType<BackgroundScroller>().GetComponent<BackgroundScroller>().NormalScroll());
            yield return StartCoroutine(LerpPlayerToPosition(0, -7, 2.5f, true));
            Destroy(newTrail);

            yield return new WaitForSeconds(1f);

            player.GetComponent<Player>().SetBulletType(sector);
            player.GetComponent<Player>().ToggleMove(true);
            gameCanvas.SetActive(true);
            enemySpawner.SetActive(true);            
        }
        else
        {
            titleCanvas.SetActive(false);
            player = Instantiate(playerPrefab, new Vector3(0, -7f, -1), Quaternion.identity);
            yield return null;
            Player.instance.SetBulletType(sector);
            if (sector == 4) Player.instance.PickUpShield();
            Player.instance.ToggleMove(true);
            gameCanvas.SetActive(true);
            if (!startingWaveSetInEnemySpawner) enemySpawner.GetComponent<EnemySpawner>().SetLastSector(sector);
            enemySpawner.SetActive(true);            
        }

        SetSectorStarsAndShowBanner(sector, 0);
        gameIsOn = true;
    }    

    public void SetSectorStarsAndShowBanner(int newSector, int wait)
    {
        sector = newSector;
        FindObjectOfType<BackgroundScroller>().GetComponent<BackgroundScroller>().SectorStars(sector);
        if (sector == 4)
            StartCoroutine(sectorBanner.GetComponent<SectorBanner>().ShowDangerBanner());
        else        
            StartCoroutine(sectorBanner.GetComponent<SectorBanner>().ShowSectorBanner(sector, wait));
    }

    public void CallFinalSector()
    {
        sector = 4;
        StartCoroutine(sectorBanner.GetComponent<SectorBanner>().ShowDangerBanner());
    }


    public void GameOver()
    {
        StartCoroutine(DelayGameOver());                        
    }

    public IEnumerator DelayGameOver(bool ending = false)
    {
        yield return new WaitForSeconds(1f);
        fadeScreen.CrossFadeAlpha(1, 2f, false);
        if (ending) endGameText.text = "congratulations!";
        endGameText.CrossFadeAlpha(1, 3f, false);
        yield return new WaitForSeconds(4f);
        FindObjectOfType<GameSession>().ResetScore();
        if (ending) FindObjectOfType<GameSession>().SetLastSector(1);
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetGameIsOn(bool state)
    {
        gameIsOn = state;
    }

    public bool getGameIsOn()
    {
        return gameIsOn;
    }

    public void CallEnding()
    {
        StartCoroutine(Ending());
    }

    public IEnumerator MovePlayerToPosition(float x, float y)
    {
        Vector3 targetPos = new Vector3(x, y, player.transform.position.z);
        do
        {
            Vector3 direction = (targetPos - player.transform.position);
            if (direction.magnitude < 0.1f) player.transform.position = targetPos;
            else player.transform.Translate(direction.normalized * Time.deltaTime * Player.instance.GetMoveSpeed());
            yield return null;            
        } while (player.transform.position != targetPos);

        //do
        //{
        //    if (player.transform.position.x > targetPos.x)
        //        if (player.transform.position.y > targetPos.y)
        //            Player.player.UpdateMove(true, -0.5f, -0.5f);
        //        else if (player.transform.position.y < targetPos.y)
        //            Player.player.UpdateMove(true, -0.5f, 0.5f);
        //        else
        //            Player.player.UpdateMove(true, -0.5f, 0);

        //    else if (player.transform.position.x < targetPos.x)
        //        if (player.transform.position.y > targetPos.y)
        //            Player.player.UpdateMove(true, 0.5f, -0.5f);
        //        else if (player.transform.position.y < targetPos.y)
        //            Player.player.UpdateMove(true, 0.5f, 0.5f);
        //        else
        //            Player.player.UpdateMove(true, 0.5f, 0);

        //    else if (player.transform.position.y > targetPos.y)
        //        if (player.transform.position.x > targetPos.x)
        //            Player.player.UpdateMove(true, -0.5f, -0.5f);
        //        else if (player.transform.position.x < targetPos.x)
        //            Player.player.UpdateMove(true, 0.5f, -0.5f);
        //        else
        //            Player.player.UpdateMove(true, 0, -0.5f);

        //    else if (player.transform.position.y < targetPos.y)
        //        if (player.transform.position.x > targetPos.x)
        //            Player.player.UpdateMove(true, -0.5f, 0.5f);
        //        else if (player.transform.position.x < targetPos.x)
        //            Player.player.UpdateMove(true, 0.5f, 0.5f);
        //        else
        //            Player.player.UpdateMove(true, 0, 0.5f);

        //    if (Mathf.Abs(player.transform.position.x - targetPos.x) < 0.1 && Mathf.Abs(player.transform.position.y - targetPos.y) < 0.1)
        //        player.transform.position = targetPos;

        //    yield return null;
        //} while (player.transform.position != targetPos);
    }

    IEnumerator LerpPlayerToPosition (float x, float y, float duration, bool smoothstep)
    {        
        Vector3 targetPos = new Vector3(x, y, player.transform.position.z);
        Vector3 playerPreviousPos = player.transform.position;
        float elapsedTime = 0;
        do
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            if (smoothstep) t = t * t * t * (t * (6f * t - 15f) + 10f); // smootherstep formula - https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/
            debugCurve.AddKey(Time.time, t);
            player.transform.position = Vector3.Lerp(playerPreviousPos, targetPos, t);
            yield return null;
        } while (elapsedTime < duration);
    }

    public IEnumerator Ending()
    {
        gameIsOn = false;
        player.GetComponent<Player>().ToggleMove(false);
        yield return new WaitForSeconds(3);       
        yield return StartCoroutine(MovePlayerToPosition(0, -8));

        // SONS, CAMERA SHAKE E RASTRO DA NAVE
        AudioSource.PlayClipAtPoint(endingBoostSFX, Camera.main.transform.position);
        Camera.main.GetComponent<CamShake>().CameraShake(0.1f, 1);
        Instantiate(speedParticleFX, player.transform);
        yield return StartCoroutine(LerpPlayerToPosition(0, -4, 1.5f, false));
        StartCoroutine(LerpPlayerToPosition(0, 25, 1f, false));
        AudioSource.PlayClipAtPoint(endingWarpSFX, Camera.main.transform.position);        
        Camera.main.GetComponent<CamShake>().CameraShake(1f, 0.5f);
        player.GetComponent<CapsuleCollider2D>().enabled = false; // desabilita collider pra nao ser destruido no shredder
        //yield return StartCoroutine(LerpPlayerToPosition(0, 25, 1.8f, true));

        yield return new WaitForSeconds(1);
        StartCoroutine(DelayGameOver(true));
    }

    //void OnGUI()
    //{
    //    if (!gameStarted)
    //    {
    //        skipIntro = GUI.Toggle(new Rect(10, 10, 200, 100), skipIntro, "Skip Intro");
    //    }        
    //}

}

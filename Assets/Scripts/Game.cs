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
    [SerializeField] GameObject gameCanvas;
    [SerializeField] GameObject startButton;
    [SerializeField] TextMeshProUGUI startButtontText;
    [SerializeField] TextMeshProUGUI sectorBanner;
    [SerializeField] AudioClip pauseSFX;
    [SerializeField] AudioClip introLaunchSFX;
    [SerializeField] AudioClip endingBoostSFX;
    [SerializeField] AudioClip endingWarpSFX;
    [SerializeField] Image fadeScreen;
    [SerializeField] public bool skipIntro;
    [SerializeField] int sector = 1;
    [SerializeField] bool startingWaveSetInEnemySpawner = false;
    [SerializeField] bool dontShowMenu = false;

    [Header("Debug")]
    public AnimationCurve pingpong = new AnimationCurve();


    bool gameIsPaused = false;
    bool gameStarted = false;

    public static Game game;
	
	void Start ()
    {
        game = this;
        if (!dontShowMenu) ShowMenu();
        if (FindObjectOfType<GameSession>().GetHiScore() > 0) skipIntro = true;
        sector = FindObjectOfType<GameSession>().GetLastSector();
        fadeScreen.CrossFadeAlpha(0, 0, false);
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
        if (Input.GetButtonDown("Cancel") && gameStarted)
        {
            PauseGame();
        }
    }



    public void StartGame()
    {
        if (!gameStarted)
        {
            StartCoroutine(GameIntro());            
        }
        else if (gameIsPaused && gameStarted)
        {
            Time.timeScale = 1;
            AudioSource.PlayClipAtPoint(pauseSFX, Camera.main.transform.position, 0.2f);
            titleCanvas.SetActive(false);
            gameIsPaused = false;
        }

    }

    public void PauseGame()
    {
        if (gameStarted)
        {
            if (!gameIsPaused)
            {
                AudioSource.PlayClipAtPoint(pauseSFX, Camera.main.transform.position, 0.2f);
                Time.timeScale = 0;                
                EventSystem.current.SetSelectedGameObject(startButton);
                startButton.GetComponent<Button>().OnSelect(null);
                startButtontText.text = "resume";
                titleCanvas.SetActive(true);
                gameIsPaused = true;
            }
            else if (gameIsPaused)
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

            player = Instantiate(playerPrefab, new Vector3(0, -11f, -1), Quaternion.identity);
            GameObject newTrail = Instantiate(speedParticleFX, player.transform);
            newTrail.transform.Rotate(-270, 0, 0);
            player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 15f);
            AudioSource.PlayClipAtPoint(introLaunchSFX, Camera.main.transform.position, 0.6f);

            while (player.transform.position.y < 0)
            {
                yield return new WaitForSeconds(Time.deltaTime);
            }

            player.GetComponent<Rigidbody2D>().gravityScale = 0f;
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            yield return new WaitForSeconds(1f);

            player.GetComponent<Rigidbody2D>().gravityScale = 0.3f;
            newTrail.GetComponent<ParticleSystem>().Stop();
            StartCoroutine(FindObjectOfType<BackgroundScroller>().GetComponent<BackgroundScroller>().NormalScroll());

            while (player.transform.position.y > -7)
            {
                yield return new WaitForSeconds(Time.deltaTime);
            }

            player.GetComponent<Rigidbody2D>().gravityScale = 0f;
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
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
            player.GetComponent<Rigidbody2D>().gravityScale = 0f;
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            player.GetComponent<Player>().SetBulletType(sector);
            player.GetComponent<Player>().ToggleMove(true);
            gameCanvas.SetActive(true);
            if (!startingWaveSetInEnemySpawner) enemySpawner.GetComponent<EnemySpawner>().SetLastSector(sector);
            enemySpawner.SetActive(true);            
        }

        SetSectorStarsAndShowBanner(sector, 0);
        gameStarted = true;
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

    public IEnumerator DelayGameOver()
    {
        yield return new WaitForSeconds(1f);
        //fadeScreen.CrossFadeAlpha(0, 0, false);
        //fadeScreen.gameObject.SetActive(true);
        fadeScreen.CrossFadeAlpha(1, 2f, false);
        yield return new WaitForSeconds(3f);
        FindObjectOfType<GameSession>().ResetScore();
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public bool GameStarted()
    {
        return gameStarted;
    }

    public void callEnding()
    {
        StartCoroutine(Ending());
    }

    public IEnumerator Ending()
    {
        player.GetComponent<Player>().ToggleMove(false);
        yield return new WaitForSeconds(3);        
        Vector3 playerPos = player.transform.position;
        Vector3 gotoPos = new Vector3(0, -7, player.transform.position.z);
        do
        {
            if (player.transform.position.x > gotoPos.x)
                if (player.transform.position.y > gotoPos.y)
                    Player.player.UpdateMove(-0.5f, -0.5f);
                else if (player.transform.position.y < gotoPos.y)
                    Player.player.UpdateMove(-0.5f, 0.5f);
                else
                    Player.player.UpdateMove(-0.5f, 0);

            else if (player.transform.position.x < gotoPos.x)
                if (player.transform.position.y > gotoPos.y)
                    Player.player.UpdateMove(0.5f, -0.5f);
                else if (player.transform.position.y < gotoPos.y)
                    Player.player.UpdateMove(0.5f, 0.5f);
                else
                    Player.player.UpdateMove(0.5f, 0);

            else if (player.transform.position.y > gotoPos.y)
                if (player.transform.position.x > gotoPos.x)
                    Player.player.UpdateMove(-0.5f, -0.5f);
                else if (player.transform.position.x < gotoPos.x)
                    Player.player.UpdateMove(0.5f, -0.5f);
                else
                    Player.player.UpdateMove(0, -0.5f);

            else if (player.transform.position.y < gotoPos.y)
                if (player.transform.position.x > gotoPos.x)
                    Player.player.UpdateMove(-0.5f, 0.5f);
                else if (player.transform.position.x < gotoPos.x)
                    Player.player.UpdateMove(0.5f, 0.5f);
                else
                    Player.player.UpdateMove(0, 0.5f);

            if (Mathf.Abs(player.transform.position.x - gotoPos.x) < 0.1 && Mathf.Abs(player.transform.position.y - gotoPos.y) < 0.1)
                player.transform.position = gotoPos;

            yield return null;
        } while (player.transform.position != gotoPos);

        AudioSource.PlayClipAtPoint(endingBoostSFX, Camera.main.transform.position);
        Camera.main.GetComponent<CamShake>().CameraShake(0.1f, 1);
        yield return new WaitForSeconds(1);
        AudioSource.PlayClipAtPoint(endingWarpSFX, Camera.main.transform.position);
        GameObject newTrail = Instantiate(speedParticleFX, player.transform);
        newTrail.transform.Rotate(-270, 0, 0);
        Camera.main.GetComponent<CamShake>().CameraShake(0.5f, 0.5f);

        player.GetComponent<CapsuleCollider2D>().enabled = false;
        playerPos = player.transform.position;
        gotoPos = new Vector3(0, 25, player.transform.position.y);
        float lerpTime = 1f;
        float elapsedTime = 0;
        do
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / lerpTime;
            t = t * t * t * (t * (6f * t - 15f) + 10f); // smoothstep formula - https://chicounity3d.wordpress.com/2014/05/23/how-to-lerp-like-a-pro/
            pingpong.AddKey(Time.time, t);
            player.transform.position = Vector3.Lerp(playerPos, gotoPos, t);
            yield return null;
        } while (elapsedTime < lerpTime);

        StartCoroutine(DelayGameOver());

    }

    void OnGUI()
    {
        if (!gameStarted)
        {
            skipIntro = GUI.Toggle(new Rect(10, 10, 200, 100), skipIntro, "Skip Intro");
        }        
    }

}

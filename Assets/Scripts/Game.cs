using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class Game : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject speedParticleFX;
    [SerializeField] GameObject enemySpawner;
    [SerializeField] GameObject titleCanvas;
    [SerializeField] GameObject gameCanvas;
    [SerializeField] GameObject startButton;
    [SerializeField] TextMeshProUGUI startButtontText;
    [SerializeField] AudioClip pauseSFX;
    [SerializeField] AudioClip introLaunchSFX;
    [SerializeField] Image fadeScreen;
    [SerializeField] public bool skipIntro;


    bool gameIsPaused = false;
    bool gameStarted = false;    
	
	void Start ()
    {
        ShowMenu();
        if (FindObjectOfType<GameSession>().GetHiScore() > 0) skipIntro = true;
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
        if (!skipIntro)
        {
            titleCanvas.SetActive(false);

            yield return (StartCoroutine(FindObjectOfType<BackgroundScroller>().GetComponent<BackgroundScroller>().GameIntroScroll()));

            yield return new WaitForSeconds(1f);

            GameObject newPlayer = Instantiate(player, new Vector3(0, -11f, 0), Quaternion.identity);
            GameObject newTrail = Instantiate(speedParticleFX, newPlayer.transform.position, Quaternion.Euler(-270, 0, 0));
            newPlayer.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 15f);
            AudioSource.PlayClipAtPoint(introLaunchSFX, Camera.main.transform.position, 0.6f);

            while (newPlayer.transform.position.y < 0)
            {
                yield return new WaitForSeconds(Time.deltaTime);
            }

            newPlayer.GetComponent<Rigidbody2D>().gravityScale = 0f;
            newPlayer.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            yield return new WaitForSeconds(1f);

            newPlayer.GetComponent<Rigidbody2D>().gravityScale = 0.3f;
            newTrail.GetComponent<ParticleSystem>().Stop();
            StartCoroutine(FindObjectOfType<BackgroundScroller>().GetComponent<BackgroundScroller>().NormalScroll());

            while (newPlayer.transform.position.y > -7)
            {
                yield return new WaitForSeconds(Time.deltaTime);
            }

            newPlayer.GetComponent<Rigidbody2D>().gravityScale = 0f;
            newPlayer.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            newTrail.GetComponent<Trail>().DestroyTrail();

            yield return new WaitForSeconds(1f);

            newPlayer.GetComponent<Player>().ToggleMove(true);
            gameCanvas.SetActive(true);
            enemySpawner.SetActive(true);
            gameStarted = true;
        }
        else
        {
            titleCanvas.SetActive(false);
            GameObject newPlayer = Instantiate(player, new Vector3(0, -7f, 0), Quaternion.identity);
            newPlayer.GetComponent<Rigidbody2D>().gravityScale = 0f;
            newPlayer.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            newPlayer.GetComponent<Player>().ToggleMove(true);
            gameCanvas.SetActive(true);
            enemySpawner.SetActive(true);
            gameStarted = true;
        }
        
    }


    public void GameOver()
    {
        StartCoroutine(DelayGameOver());                        
    }

    public IEnumerator DelayGameOver()
    {
        yield return new WaitForSeconds(2f);
        fadeScreen.CrossFadeAlpha(0, 0, false);
        fadeScreen.gameObject.SetActive(true);
        fadeScreen.CrossFadeAlpha(1, 2f, false);
        yield return new WaitForSeconds(3f);
        FindObjectOfType<GameSession>().ResetScore();
        SceneManager.LoadScene(0);
    }

    public void EndGame()
    {
       gameStarted = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public bool GameStarted()
    {
        return gameStarted;
    }

    void OnGUI()
    {
        if (!gameStarted)
        {
            skipIntro = GUI.Toggle(new Rect(10, 10, 200, 100), skipIntro, "Skip Intro");
        }        
        //GUI.Label(new Rect(10, 10, 200, 40), health.ToString());
    }

}

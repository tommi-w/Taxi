using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GUIManager : MonoBehaviour
{

    //public static GUIManager instance = null;


    public Text playerHealthText;
    public Text playerInfamyText;
    public Text PlayerFameText;
    public GameObject popupPrefab;
    public GameObject menuItems;
    public GameObject restartMenu;
    public GameObject creditsMenu;
    public Text fameScoreText;
    public Text infamyScoreText;

    private Button QuitButton;
    private Button CreditsButton;
    private Button PlayButton;
    private Button RestartButton;

    private bool isPaused;
    private GameObject popupPoolGameobject;
    private GameObject[] popupPool;
    private int lastpopup = -1;
    private bool creatingDamagePopup = false;
    private int totalDamageForPopup = 0;
    private bool creatingPointsPopup = false;
    private int totalPointsForPopup = 0;

    private Animator anim;

    // Use this for initialization
    void Awake()
    {
        //if (instance == null)
        //    instance = this;
        //else if (instance != this)
        //    Destroy(gameObject);

        //DontDestroyOnLoad(gameObject);

        anim = GetComponent<Animator>();

        //StartCoroutine(PopulateVariables());

        Invoke("UpdateScore", .01f);

        PopulatePopupPool();

        isPaused = true;
        restartMenu.SetActive(false);
        menuItems.SetActive(true);
        Debug.Log("GUIManager AWAKENS");
        Time.timeScale = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
                UnPause();
            else if (!isPaused)
                Pause();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Quit();
            else if (!isPaused)
                Pause();
        }
        //if (infamyScoreText == null)
            //StartCoroutine(PopulateVariables());
    }

    //IEnumerator PopulateVariables()
    //{
    //    yield return new WaitForSeconds(.1f);
    //    anim = GameObject.Find("GUI").GetComponent<Animator>();
    //    playerHealthText = GameObject.Find("PlayerHealthText").GetComponent<Text>();
    //    playerInfamyText = GameObject.Find("PlayerInfamyText").GetComponent<Text>();
    //    PlayerFameText = GameObject.Find("PlayerFameText").GetComponent<Text>();
    //    menuItems = GameObject.Find("MenuItems");
    //    restartMenu = GameObject.Find("RestartMenu");
    //    fameScoreText = GameObject.Find("FameScoreText").GetComponent<Text>();
    //    infamyScoreText = GameObject.Find("InfamyScoreText").GetComponent<Text>();
    //    Button[] menuButtons = new Button[3];
    //    menuButtons = GetComponentsInChildren<Button>();
    //    for (int i = 0; i < menuButtons.Length; i++)
    //    {
    //        if (menuButtons[i].name == "QuitButton")
    //            QuitButton = menuButtons[i];
    //        if (menuButtons[i].name == "CreditsButton")
    //            CreditsButton = menuButtons[i];
    //        if (menuButtons[i].name == "PlayButton")
    //            PlayButton = menuButtons[i];
    //    }
    //    RestartButton = restartMenu.GetComponentInChildren<Button>();
    //    QuitButton.onClick.AddListener(Quit);
    //    CreditsButton.onClick.AddListener(Quit);
    //    PlayButton.onClick.AddListener(UnPause);
    //    RestartButton.onClick.AddListener(RestartGame);


        
    //    yield return null;
    //}


    private void PopulatePopupPool()
    {
        popupPoolGameobject = GameObject.Find("popupPool");
        popupPool = new GameObject[20];
        for (int i = 0; i < popupPool.Length; i++)
        {
            popupPool[i] = Instantiate(popupPrefab, popupPoolGameobject.transform);
            popupPool[i].SetActive(false);
        }
    }

    private void ClearPopupPool()
    {
        if (popupPool != null)
        {
            for (int i = 0; i < popupPool.Length; i++)
            {
                popupPool[i].SetActive(false);
                Debug.Log("Destroying "+ popupPool[i]);
                Destroy(popupPool[i]);
            }
        }
    }

    public void Pause ()
    {
        StartCoroutine(PauseGame());
    }

    IEnumerator PauseGame()
    {
        isPaused = true;
        menuItems.SetActive(true);
        anim.SetBool("isPaused", true);
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 0;
    }

    public void UnPause()
    {
        StartCoroutine(UnPauseGame());
    }

    IEnumerator UnPauseGame()
    {
        isPaused = false;
        anim.SetBool("isPaused", false);
        Time.timeScale = 1;
        yield return new WaitForSeconds(0.5f);
        menuItems.SetActive(false);
    }


    public void UpdateScore()
    {
        playerHealthText.text = "Health: " + GameManager.instance.playerHealth;
        playerInfamyText.text = "Infamy: " + GameManager.instance.playerInfamy;
        PlayerFameText.text = "Fame: " + GameManager.instance.playerFame;
    }


    public void ShowCreditMenu()
    {
        if(creditsMenu.activeInHierarchy)
        {
            creditsMenu.SetActive(false);
        }
        else
        {
            creditsMenu.SetActive(true);
        }
        

    }

    public void ActivateDamagePopup(Transform target, int damage)
    {
        totalDamageForPopup += damage;
        if(!creatingDamagePopup)
        {
            StartCoroutine(MoveDamagePopup(target));
        }
    }

    public void ActivatePointsPopup(Transform target, int points)
    {
        totalPointsForPopup += points;
        if (!creatingPointsPopup)
        {
            StartCoroutine(MovePointsPopup(target));
        }
    }



    IEnumerator MoveDamagePopup(Transform target)
    {
        creatingDamagePopup = true;
        yield return new WaitForSeconds(.3f);

        GameObject myPopup = GetPopup();
        if (myPopup == null)
        {
            yield return new WaitForSeconds(.1f);
            myPopup = GetPopup();
        }

        myPopup.GetComponent<Transform>().localPosition = GetRelativeUIPosition(target.position);
        myPopup.GetComponentInChildren<Text>().text = "- " +totalDamageForPopup.ToString()+ " HP";
        myPopup.GetComponentInChildren<Text>().color = Color.red;
        myPopup.gameObject.SetActive(true);
        myPopup.GetComponentInChildren<Image>().color = new Color(0,0,0,0);
        float myScale = Mathf.Pow(totalDamageForPopup/10f+0.4f, 0.5f);
        myPopup.GetComponent<Transform>().localScale = new Vector3(myScale, myScale, myScale);
        myPopup.GetComponentInChildren<Animator>().SetTrigger("damagePopupTrigger");
        creatingDamagePopup = false;
        totalDamageForPopup = 0;
        float startTime = Time.time;
        while (Time.time -startTime <= 1.25f)
        {
            myPopup.GetComponent<Transform>().localPosition = GetRelativeUIPosition(target.position);
            yield return new WaitForFixedUpdate();
        }
        //yield return new WaitForSeconds(1.25f);

        myPopup.gameObject.SetActive(false); 
    }

    IEnumerator MovePointsPopup(Transform target)
    {
        creatingPointsPopup = true;
        yield return new WaitForSeconds(.2f);

        GameObject myPopup = GetPopup();
        myPopup.GetComponent<Transform>().localPosition = GetRelativeUIPosition(target.position);
        myPopup.GetComponentInChildren<Text>().text = "" + totalPointsForPopup.ToString() + "";
        myPopup.GetComponentInChildren<Text>().color = Color.red;
        myPopup.gameObject.SetActive(true);
        Color myColor = new Color(Random.Range(.3f,1.0f), Random.Range(.3f, 1.0f), Random.Range(.3f, 1.0f));
        float myScale = Mathf.Pow(totalPointsForPopup/200f+0.7f, 0.3f);
        myPopup.GetComponentInChildren<Image>().color = myColor;
        myPopup.GetComponent<Transform>().localScale = new Vector3(myScale,myScale,myScale);
        //Debug.Log(target.name +" at "+target.position.ToString());
        //Debug.Log(myPopup.name + " at " + myPopup.GetComponent<Transform>().position.ToString());
        myPopup.GetComponentInChildren<Animator>().SetTrigger("pointsPopupTrigger");
        creatingPointsPopup = false;
        totalPointsForPopup = 0;
        float startTime = Time.time;
        while (Time.time - startTime <= 1.25f)
        {
            myPopup.GetComponent<Transform>().localPosition = GetRelativeUIPosition(target.position);
            yield return new WaitForFixedUpdate();
        }
        //yield return new WaitForSeconds(1.25f);

        myPopup.gameObject.SetActive(false);
    }

    private GameObject GetPopup()
    {
        if (GameManager.instance.isGameOver)
            return null;

        do
        {
            lastpopup += 1;
            if (lastpopup >= popupPool.Length)
                lastpopup = 0;
                //Debug.Log("Trying to find a popup ");
        } while (popupPool[lastpopup].activeInHierarchy);

        return popupPool[lastpopup];
    }

    private Vector3 GetRelativeUIPosition(Vector3 targetPosition)
    {
        Vector3 relativePosition = new Vector3();
        Vector3 cameraPosition = GameObject.Find("Main Camera").transform.position;
        Vector3 directionVector = targetPosition - cameraPosition;
        relativePosition.x = directionVector.x / 7f * 755f;
        relativePosition.y = directionVector.y / 4f *425f;

        //Debug.Log("cameraPosition "+ cameraPosition);
        //Debug.Log("targetPosition " + targetPosition);
        //Debug.Log("relativePosition " + relativePosition);

        return relativePosition;
    }


    public void RestartGame()
    {
        StopAllCoroutines();
        GameManager.instance.ResetScores();
        UpdateScore();
        restartMenu.SetActive(false);
        StartCoroutine(RestartGameCoroutine());
    }

    IEnumerator RestartGameCoroutine()
    {
        //restartMenu.SetActive(false);
        Time.timeScale = 1f;
        Pause();
        yield return new WaitForSeconds(.5f);
        popupPool = null;
        lastpopup = -1;
        creatingDamagePopup = false;
        totalDamageForPopup = 0;
        creatingPointsPopup = false;
        totalPointsForPopup = 0;
        GameManager.instance.isGameOver = false;
        GameManager.instance.RestartGame();
        yield return null;
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        fameScoreText.text = "Fame : " + GameManager.instance.playerFame;
        infamyScoreText.text = "Infamy : " + GameManager.instance.playerInfamy;
        restartMenu.SetActive(true);
        ClearPopupPool();
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit ();
#endif
    }
}

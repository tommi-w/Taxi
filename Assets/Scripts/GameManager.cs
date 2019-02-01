using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    public int playerHealth = 100;
    public int playerFame = 0;
    public int playerInfamy = 0;
    public bool isGameOver = false;
    public GameObject guiPrefab;
    public GameObject playerPrefab;
    public List<GameObject> playerCharacterPrefabs;
    public List<GameObject> playerCagePrefabs;

    [HideInInspector]public GUIManager gui;
    [HideInInspector]public GameObject player;

    [HideInInspector] public GameObject playerCurrentCage;
    [HideInInspector] public GameObject playerCurrentCharacter;

    void Awake () {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        ResetScores();
        isGameOver = false;
        //gui = GameObject.Find("GUIManager").GetComponent<GUIManager>();
        gui = Instantiate<GameObject>(guiPrefab, Vector3.zero, Quaternion.identity).GetComponent<GUIManager>();
        player = Instantiate<GameObject>(playerPrefab, GameObject.Find("PlayerSpawn").transform.position, Quaternion.identity);
        playerCurrentCharacter = Instantiate<GameObject>(playerCharacterPrefabs[0], player.transform);
        playerCurrentCage = Instantiate<GameObject>(playerCagePrefabs[1], player.transform);
    }
	
    public void GivePlayerInfamy(int points)
    {
        playerInfamy += points;
        gui.UpdateScore();
    }

    public void GivePlayerFame(int points)
    {
        playerFame += points;
        gui.UpdateScore();
    }

    public void DamagePlayer(int damage)
    {
        playerHealth -= damage;
        gui.UpdateScore();
        if (playerHealth  <= 0)
        {
            GameOver();
        }
    }

    public void StartSlowMotion()
    {
        StartCoroutine(SlowMotion());
    }

    public IEnumerator SlowMotion()
    {
        Debug.Log("Slow Motion Started");
        float slowAmount = 0.3f;
        Time.timeScale = slowAmount;
        for (float i = slowAmount; i<.9f; i += .1f)
        {
            yield return new WaitForSeconds(.1f);
            Time.timeScale = i;
            //Debug.Log("Timescale is now: "+ i);
        }
        Time.timeScale = 1f;
        //Debug.Log("Timescale is now: " + Time.timeScale);
    }

    private void GameOver()
    {
        isGameOver = true;
        gui.GameOver();
    }

    public void RestartGame()
    {
        Destroy(gui);
        SceneManager.LoadScene(0);
        gui = Instantiate<GameObject>(guiPrefab, Vector3.zero, Quaternion.identity).GetComponent<GUIManager>();
    }

    public void ResetScores()
    {
        playerHealth = 100;
        playerFame = 0;
        playerInfamy = 0;
    }

    public void ChangeCageNext()
    {
        Destroy(playerCurrentCage);
        playerCurrentCage = Instantiate<GameObject>(playerCagePrefabs[0], player.transform);
    }
}

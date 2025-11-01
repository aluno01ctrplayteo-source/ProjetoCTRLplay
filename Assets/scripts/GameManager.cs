using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public PlayerHealthManager healthManager;
    public bool isPaused;
    public GameObject pauseMenu;
    public int currency = 0;
    public TMP_Text coinDisplay;
    public List<Enemy> enemies;
    public GameObject gameOverMenu;
    public GameObject inventoryMenu;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        healthManager = FindAnyObjectByType<PlayerHealthManager>();
    }

    private void Update()
    {
        enemies = new List<Enemy>(FindObjectsOfType<Enemy>());
    }

    public void GameOver()
    {
        gameOverMenu.SetActive(true);
    }

    public void ReloadLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void Start()
    {
        coinDisplay.text = $"Coins -> {currency}";  
        pauseMenu.SetActive(false);
        inventoryMenu.SetActive(false);
    }
    public void IsGamePaused()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
        }
    }
    public void IsInventoryOpen()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            inventoryMenu.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            inventoryMenu.SetActive(false);
            Time.timeScale = 1f;
        }
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void Continue()
    {
        IsGamePaused();
    }
}
public interface IInteracted
{
    void Interacted();
}

// Brackeys

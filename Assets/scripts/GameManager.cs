using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public HealthManager healthManager;
    public bool isPaused;
    public GameObject pauseMenu;
    public int currency = 0;
    public TMP_Text coinDisplay;
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
        healthManager = FindAnyObjectByType<HealthManager>();
    }
    void Start()
    {
        healthManager.HpChanger(-50);
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
        inventoryMenu.SetActive(!inventoryMenu.activeSelf);
        if (inventoryMenu.activeSelf)
        {
            Time.timeScale = 0f;
        }
        else
        {
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

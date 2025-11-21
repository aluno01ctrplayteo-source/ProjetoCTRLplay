using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public PlayerHealthManager healthManager;
    public bool isPaused;
    public GameObject pauseMenu;
    public int currency = 0;
    public int killCount = 0;
    public GameObject torchLight;
    public TMP_Text coinDisplay;
    public List<GameObject> enemies;
    public GameObject gameOverMenu;
    public GameObject inventoryMenu;
    public List<Items> items;
    public event Action OnEnemyCreation;
    public event Action OnEnemyDeath;
    
    private string initialCoinDisplayText;

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
    public void RaiseEnemyDeathEvent() => OnEnemyDeath?.Invoke();
    public void RaiseEnemyCreationEvent()
    {
        OnEnemyCreation?.Invoke();
        enemies = new(GameObject.FindGameObjectsWithTag("Enemy"));
    }
    public IEnumerator GameOver()
    {
        if (gameOverMenu == null) yield break;
        Graphic g = gameOverMenu.GetComponentInChildren<Graphic>();
        Color c = g.color;
        while (c.a != 1) { c.a += .2f * Time.deltaTime; g.color = c; yield return null; }
        yield return new WaitForSeconds(1);
        ReloadLevel();
    }

    public void ReloadLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void InstantiateLightingSources(GameObject light)
    {
        List<GameObject> list = new(GameObject.FindGameObjectsWithTag("LightSource"));
        foreach (GameObject source in list)
        {
            Vector3 pos = source.transform.position + Vector3.up;
            Instantiate(light, pos, Quaternion.identity, source.transform).SetActive(true);
        }
    }

    void Start()
    {
        if (coinDisplay != null)
            initialCoinDisplayText = coinDisplay.text;
        coinDisplay.text = $"{initialCoinDisplayText} -> {currency}";
        if (pauseMenu == null) return;
        pauseMenu.SetActive(false);
        if (inventoryMenu == null) return;
        inventoryMenu.SetActive(false);
        InstantiateLightingSources(torchLight);
    }
    public void ChangeCurrencyAmount(int amount)
    {
        currency += amount;
        if (coinDisplay != null)
        {
            coinDisplay.text = $"{initialCoinDisplayText} -> {currency}";
        }
    }
    public void GamePaused()
    {
        if (pauseMenu == null) return;
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
    public void InventoryOpen()
    {
        if (inventoryMenu == null) return;
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
        GamePaused();
    }
}
public interface IInteracted
{
    void Interacted()
    {
        throw new NotImplementedException();
    }
}

// Brackeys
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public bool isPaused;
    public GameObject pauseMenu;
    public int currency = 0;
    public int killCount = 0;
    public GameObject torchLight;
    public TMP_Text coinDisplay;
    public List<GameObject> enemies;
    public SaveSystem saveSystem;
    public GameObject gameOverMenu;
    public GameObject inventoryMenu;
    public Texture2D cursorSprite;
    public List<Items> items;
    public Player player;
    public event Action OnEnemyCreation;
    public event Action OnEnemyDeath;
    public Inventory inventory;


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

    }
    private void OnEnable()
    {
        OnEnemyCreation += () => { enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList(); };
    }
    public void RaiseEnemyDeathEvent() => OnEnemyDeath?.Invoke();
    public void RaiseEnemyCreationEvent()
    {
        OnEnemyCreation?.Invoke();
    }
    public IEnumerator GameOver()
    {
        if (gameOverMenu == null) yield break;
        Graphic g1 = gameOverMenu.transform.Find("death").GetComponent<Graphic>();
        Graphic g2 = gameOverMenu.GetComponent<Graphic>();
        Color c1 = new(1, 1, 1, 0);
        Color c2 = new(0, 0, 0, 0);

        while (c1.a <= 1 || c2.a <= 1) { 
            c1.a += .2f * Time.deltaTime;
            g1.color = c1;  
            if (c1.a >= .5f)
            { 
                c2.a += .3f * Time.deltaTime; 
                g2.color = c2; 
            } 
            yield return null; 
        }
        yield return new WaitForSeconds(1);
        ReloadLevel();
    }


    public void ReloadLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void InstantiateLightSources(GameObject light)
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
        InstantiateLightSources(torchLight);
    }
    public void ChangeCurrencyAmount(int amount)
    {
        currency += amount;
        if (coinDisplay != null)
        {
            coinDisplay.text = $"{initialCoinDisplayText} -> {currency}";
        }
    }
    public void ToggleMenu()
    {
        Debug.Log("Toggle Menu Called");
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
    public void ToggleInventory()
    {
        Debug.Log("Toggle Inventory Called");
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
    public void Save()
    {
        SaveData data = new(currency,SceneManager.GetActiveScene().buildIndex ,player.transform.position , inventory.inventory);
        saveSystem.Save(data);
    }
    public void Load()
    {
        SaveData data = saveSystem.Load();
        if (data == null) return;
        currency = data._currency;
        if (SceneManager.GetActiveScene().buildIndex != data._levelIndex) SceneManager.LoadScene(data._levelIndex);
        player.transform.position = data._playerPos;
        inventory.inventory = data._inventoryItems;
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
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool isPaused;
    public GameObject pauseMenu;
    [SerializeField]
    private string currentMenuPath;
    public string CurrentMenuPath { get { return currentMenuPath; } private set { currentMenuPath = isPaused ? value : "none"; } }
    public int currency = 0;
    public int killCount = 0;
    public GameObject torchLight;
    public TMP_Text coinDisplay;
    public List<GameObject> enemies;
    public SaveSystem saveSystem;
    public GameObject gameOverMenu;
    public GameObject inventoryMenu;
    public Texture2D cursorSprite;
    public List<Items> everyItem;
    public Player player;
    public event Action OnEnemyCreation;
    public event Action<EnemyDeathContext> OnEnemyDeath;
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
            Debug.LogWarning("Another GameManager was found within this scene!");
            Destroy(gameObject);
        }
        CurrentMenuPath = "none";
    }
    private void OnEnable()
    {
        OnEnemyCreation += () => { enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList(); };
    }
    public void LoadScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }
    public void RaiseEnemyDeathEvent(EnemyDeathContext context) => OnEnemyDeath?.Invoke(context);
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

        while (c1.a <= 1 || c2.a <= 1)
        {
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
        try
        {
            initialCoinDisplayText = coinDisplay.text;
            coinDisplay.text = $"{initialCoinDisplayText} -> {currency}";
            pauseMenu.SetActive(false);
            inventoryMenu.SetActive(false);
            InstantiateLightSources(torchLight);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
    public void ChangeCurrencyAmount(int amount)
    {
        currency += amount;
        if (coinDisplay != null)
        {
            coinDisplay.text = $"{initialCoinDisplayText} -> {currency}";
        }
    }
    public void ToggleMenu(InputAction.CallbackContext ctx)
    {
        switch (ctx.control.path)
        {
            case "/Keyboard/escape":
                if (CurrentMenuPath != "none" && CurrentMenuPath != ctx.control.path) { break; }
                isPaused = !isPaused;
                CurrentMenuPath = "/Keyboard/escape";
                pauseMenu.SetActive(isPaused);
                Time.timeScale = isPaused ? 0f : 1f;
                Debug.Log(ctx.control.path);
                break;
            case "/Keyboard/t":
                if (CurrentMenuPath != "none" && CurrentMenuPath != ctx.control.path) { break; }
                isPaused = !isPaused;
                CurrentMenuPath = "/Keyboard/t";
                inventoryMenu.SetActive(isPaused);
                Time.timeScale = isPaused ? 0f : 1f;
                Debug.Log(ctx.control.path);
                break;
            default:
                Debug.Log("what..?");
                Debug.Log(ctx.control.path);
                break;
        }
    }
    public void Quit()
    {
        UnityEngine.Application.Quit();
    }
    public void Save()
    {
        SaveData data = new(currency, SceneManager.GetActiveScene().buildIndex, player.transform.position, inventory.inventory);
        saveSystem.Save(data);
    }
    public void Load()
    {
        SaveData data = saveSystem.Load();
        if (SceneManager.GetActiveScene().buildIndex != data.levelBuildIndex) SceneManager.LoadScene(data.levelBuildIndex);
        currency = data.currency;
        player.transform.position = data.playerPos;

    }
}
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}
public class EnemyDeathContext
{
    public int id;
    public GameObject gameObject;
    public Collider collider;
    public EnemyDeathContext(int id, GameObject gameObject, Collider collider) 
    {
        this.id = id;
        this.gameObject = gameObject;
        this.collider = collider;
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour
{
    [Header("Health Manager")]
    public int health = 100;
    public Slider hpBar;
    public int maxHealth = 100;
    public int minHealth = 0;

    private void Start()
    {
        hpBar.value = health;
    }

    private void Update()
    {
        if (health <= minHealth)
        {
            Debug.Log("Game Over");
            GameManager.instance.GameOver();
        }
    }
    public void HpChanger(int amount)
    {
        health += amount;
        health = Mathf.Clamp(health, minHealth, maxHealth);
        hpBar.value = health;
    }


}

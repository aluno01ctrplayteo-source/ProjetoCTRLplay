using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public int health = 100;
    public Slider hpBar;
    public int damage;
    public int maxHealth = 100;
    public int minHealth = 0;
    public Button takeDamage;

    private void Start()
    {
        hpBar.value = health;
        InvokeRepeating("TakeDamage", 5f, 5f);
    }

    private void Update()
    {
        
    }
    public void TakeDamage()
    {
        Mathf.Clamp(health, minHealth, maxHealth);
        health += damage;
        hpBar.value = health;
    }


}

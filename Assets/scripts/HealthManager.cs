using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
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
        
    }
    public void HpChanger(int amount)
    {
        Mathf.Clamp(health, minHealth, maxHealth);
        health += amount;
        hpBar.value = health;
    }


}

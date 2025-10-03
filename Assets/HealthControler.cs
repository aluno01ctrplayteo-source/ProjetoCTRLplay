using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthControler : MonoBehaviour
{
    // Start is called before the first frame update
    public int health = 100;
    public Slider healthBar;
    public void Start()
    {
        healthBar.maxValue = health;
        healthBar.value = health;
    }
    public void DamageHealth()
    {
        health -= 10;
        UpdateInterface();
    }
    public void UpdateInterface()
    {
        healthBar.value = health;
    }

}

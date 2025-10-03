using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrencyCollection : MonoBehaviour
{
    public int currencyAmount = 0;
    public TMP_Text scoreDisplay;
    public HealthControler healthControler;

    void Start()
    {
        scoreDisplay.text = "0";
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("coin"))
        {
            currencyAmount++;
            scoreDisplay.text = currencyAmount.ToString();
            Destroy(other.gameObject);
            Debug.Log("Currency: " + currencyAmount);
        }
        if (other.CompareTag("enemy"))
        {
            healthControler.DamageHealth();
            Debug.Log("Player hit by enemy");
        }
    }

}

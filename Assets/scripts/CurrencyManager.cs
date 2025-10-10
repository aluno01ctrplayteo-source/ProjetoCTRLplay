using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
public class CurrencyManager : MonoBehaviour
{
    public int currency = 0;
    public TMP_Text display;
    private void Start()
    {
        display.text = $"Coins -> {currency}";
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            currency++;
            display.text = $"Coins -> {currency}";
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUI : MonoBehaviour
{
    [SerializeField] TMP_Text hpText;
    [SerializeField] float maxHP = 100f;
    [SerializeField] float currentHP;      

    [SerializeField] float decreaseRate = 5f;  

    void Start()
    {
        currentHP = maxHP;
        UpdateHPText();

        StartCoroutine(DecreaseHPOverTime());
    }

    private System.Collections.IEnumerator DecreaseHPOverTime()
    {
        while (currentHP > 0)
        {
            yield return new WaitForSeconds(1f); 
            currentHP -= decreaseRate;           
            currentHP = Mathf.Clamp(currentHP, 0, maxHP); 
            UpdateHPText();                      
        }
    }

    private void UpdateHPText()
    {
        hpText.text = "HP: " + currentHP.ToString("F0");
    }
}

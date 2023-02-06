using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class UIHandler : MonoBehaviour
{

    public TextMeshProUGUI healthText;

    void OnEnable()
    {
        FirstPersonController.OnHeal += UpdateHealth;
    }
    void OnDisable()
    {
        FirstPersonController.OnHeal -= UpdateHealth;
    }
    void UpdateHealth(int currentHealth, int maxHealth)
    {
        healthText.text = currentHealth.ToString() + "/" + maxHealth.ToString();                
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private PlayerCombatController _player;

    private GameManager gameManager;

    [SerializeField]
    private Slider slider;

    private void Start()
    {
        SetUp();
    }

    public void SetUp()
    {

        _player.OnHealthChanged += UpdateHealth;
    }

    private void UpdateHealth()
    {
        var currentHealthInPercent = (float)_player.currentHealth / PlayerCombatController.maxHealth;
        slider.value = currentHealthInPercent;
    }
}

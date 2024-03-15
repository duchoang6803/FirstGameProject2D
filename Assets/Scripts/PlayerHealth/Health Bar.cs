using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private PlayerCombatController _player;

    private GameManager gameManager;

    [SerializeField]
    private Slider slider;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }


    private void Start()
    {
        SetUp();
    }

    public void SetUp()
    {

        _player.OnHealthChanged += UpdateHealth;

    }


    public void UpdateHealth()
    {
        if (_player.currentHealth == PlayerCombatController.maxHealth || _player.currentHealth >= 0)
        {
            var currentHealthInPercent = (float)_player.currentHealth / PlayerCombatController.maxHealth;
            slider.value = currentHealthInPercent;

        }

        else if (_player.currentHealth <= 0 && gameManager.isRespawn)
        {
            var newHealth = (float)0.01 * PlayerCombatController.maxHealth;
            slider.value = newHealth;
        }
    }
}

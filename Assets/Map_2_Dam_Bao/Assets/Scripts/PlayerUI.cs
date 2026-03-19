using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Health playerHealth;
    public Mana playerMana;

    public Slider hpSlider;
    public Slider manaSlider;

    void Update()
    {
        FindPlayerIfNeeded();

        if (playerHealth != null && hpSlider != null)
        {
            hpSlider.maxValue = playerHealth.maxHP;
            hpSlider.value = playerHealth.currentHP;
        }

        if (playerMana != null && manaSlider != null)
        {
            manaSlider.maxValue = playerMana.maxMana;
            manaSlider.value = playerMana.currentMana;
        }
    }

    void FindPlayerIfNeeded()
    {
        if (playerHealth != null && playerMana != null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        if (playerHealth == null)
            playerHealth = player.GetComponent<Health>();

        if (playerMana == null)
            playerMana = player.GetComponent<Mana>();
    }
}
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Health playerHealth;
    public Mana playerMana;

    public Slider hpSlider;
    public Slider manaSlider;

    private GameObject player;

    void Update()
    {
        FindPlayerIfNeeded();

        if (hpSlider != null && PlayerCompatibilityUtility.TryGetHealthValues(player, out int currentHP, out int maxHP))
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = currentHP;
        }

        if (manaSlider != null && PlayerCompatibilityUtility.TryGetManaValues(player, out int currentMana, out int maxMana))
        {
            manaSlider.maxValue = maxMana;
            manaSlider.value = currentMana;
        }
    }

    void FindPlayerIfNeeded()
    {
        if (player != null) return;

        player = PlayerCompatibilityUtility.FindPlayer();
        if (player == null) return;

        playerHealth = player.GetComponent<Health>();
        playerMana = player.GetComponent<Mana>();
    }
}

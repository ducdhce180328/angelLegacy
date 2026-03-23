using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MeterSpriteUI : MonoBehaviour
{
    [Header("Player")]
    public Health playerHealth;
    public Mana playerMana;

    [Header("UI Images")]
    public Image hpFillImage;
    public Image manaFillImage;

    [Header("HP Sprites (Red)")]
    public Sprite[] hpSprites;

    [Header("Mana Sprites (Blue)")]
    public Sprite[] manaSprites;

    [Header("Value Text")]
    public TMP_Text hpValueText;
    public TMP_Text manaValueText;

    private GameObject player;

    void Update()
    {
        FindPlayerIfNeeded();
        UpdateHP();
        UpdateMana();
    }

    void FindPlayerIfNeeded()
    {
        if (player != null) return;

        player = PlayerCompatibilityUtility.FindPlayer();
        if (player == null) return;

        playerHealth = player.GetComponent<Health>();
        playerMana = player.GetComponent<Mana>();
    }

    void UpdateHP()
    {
        if (!PlayerCompatibilityUtility.TryGetHealthValues(player, out int currentHP, out int maxHP) || maxHP <= 0) return;

        float percent = (float)currentHP / maxHP;

        if (hpFillImage != null && hpSprites != null && hpSprites.Length > 0)
        {
            hpFillImage.sprite = GetMeterSprite(percent, hpSprites);
            hpFillImage.enabled = hpFillImage.sprite != null;
        }

        if (hpValueText != null)
        {
            hpValueText.text = currentHP + " / " + maxHP;
        }
    }

    void UpdateMana()
    {
        if (!PlayerCompatibilityUtility.TryGetManaValues(player, out int currentMana, out int maxMana) || maxMana <= 0) return;

        float percent = (float)currentMana / maxMana;

        if (manaFillImage != null && manaSprites != null && manaSprites.Length > 0)
        {
            manaFillImage.sprite = GetMeterSprite(percent, manaSprites);
            manaFillImage.enabled = manaFillImage.sprite != null;
        }

        if (manaValueText != null)
        {
            manaValueText.text = currentMana + " / " + maxMana;
        }
    }

    Sprite GetMeterSprite(float percent, Sprite[] sprites)
    {
        percent = Mathf.Clamp01(percent);

        if (percent <= 0f)
            return null;

        int index = Mathf.CeilToInt(percent * sprites.Length) - 1;
        index = Mathf.Clamp(index, 0, sprites.Length - 1);

        return sprites[index];
    }
}

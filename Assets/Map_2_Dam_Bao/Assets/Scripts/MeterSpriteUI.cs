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

    void Update()
    {
        FindPlayerIfNeeded();
        UpdateHP();
        UpdateMana();
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

    void UpdateHP()
    {
        if (playerHealth == null) return;

        float percent = (float)playerHealth.currentHP / playerHealth.maxHP;

        if (hpFillImage != null && hpSprites != null && hpSprites.Length > 0)
        {
            hpFillImage.sprite = GetMeterSprite(percent, hpSprites);
            hpFillImage.enabled = hpFillImage.sprite != null;
        }

        if (hpValueText != null)
        {
            hpValueText.text = playerHealth.currentHP + " / " + playerHealth.maxHP;
        }
    }

    void UpdateMana()
    {
        if (playerMana == null) return;

        float percent = (float)playerMana.currentMana / playerMana.maxMana;

        if (manaFillImage != null && manaSprites != null && manaSprites.Length > 0)
        {
            manaFillImage.sprite = GetMeterSprite(percent, manaSprites);
            manaFillImage.enabled = manaFillImage.sprite != null;
        }

        if (manaValueText != null)
        {
            manaValueText.text = playerMana.currentMana + " / " + playerMana.maxMana;
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
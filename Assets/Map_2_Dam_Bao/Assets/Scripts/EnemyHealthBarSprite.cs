using UnityEngine;

public class EnemyHealthBarSprite : MonoBehaviour
{
    public Health enemyHealth;
    public SpriteRenderer barFillRenderer;

    [Header("Red Meter Sprites (01 -> 10)")]
    public Sprite[] hpSprites;

    void Update()
    {
        if (enemyHealth == null || barFillRenderer == null || hpSprites == null || hpSprites.Length == 0)
            return;

        float percent = (float)enemyHealth.currentHP / enemyHealth.maxHP;
        barFillRenderer.sprite = GetMeterSprite(percent);

        if (enemyHealth.isDead)
        {
            gameObject.SetActive(false);
        }
    }

    Sprite GetMeterSprite(float percent)
    {
        percent = Mathf.Clamp01(percent);

        if (percent <= 0f)
            return null;

        int index = Mathf.CeilToInt(percent * hpSprites.Length) - 1;
        index = Mathf.Clamp(index, 0, hpSprites.Length - 1);

        return hpSprites[index];
    }

    void LateUpdate()
    {
        transform.localScale = new Vector3(1, 1, 1);
    }
}
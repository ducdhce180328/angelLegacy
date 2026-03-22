using TMPro;
using UnityEngine;

public class SkillUI : MonoBehaviour
{
    [Header("Skill 1 UI")]
    [SerializeField] private GameObject skill1CooldownMask;
    [SerializeField] private TextMeshProUGUI skill1CooldownText;

    [Header("Skill 2 UI")]
    [SerializeField] private GameObject skill2CooldownMask;
    [SerializeField] private TextMeshProUGUI skill2CooldownText;

    private void Start()
    {
        SetSkill1CooldownVisible(false);
        SetSkill2CooldownVisible(false);
    }

    public void UpdateSkill1Cooldown(float remainTime)
    {
        if (remainTime > 0f)
        {
            SetSkill1CooldownVisible(true);
            skill1CooldownText.text = Mathf.CeilToInt(remainTime).ToString();
        }
        else
        {
            SetSkill1CooldownVisible(false);
        }
    }

    public void UpdateSkill2Cooldown(float remainTime)
    {
        if (remainTime > 0f)
        {
            SetSkill2CooldownVisible(true);
            skill2CooldownText.text = Mathf.CeilToInt(remainTime).ToString();
        }
        else
        {
            SetSkill2CooldownVisible(false);
        }
    }

    private void SetSkill1CooldownVisible(bool isVisible)
    {
        if (skill1CooldownMask != null)
        {
            skill1CooldownMask.SetActive(isVisible);
        }

        if (skill1CooldownText != null)
        {
            skill1CooldownText.gameObject.SetActive(isVisible);
        }
    }

    private void SetSkill2CooldownVisible(bool isVisible)
    {
        if (skill2CooldownMask != null)
        {
            skill2CooldownMask.SetActive(isVisible);
        }

        if (skill2CooldownText != null)
        {
            skill2CooldownText.gameObject.SetActive(isVisible);
        }
    }
}
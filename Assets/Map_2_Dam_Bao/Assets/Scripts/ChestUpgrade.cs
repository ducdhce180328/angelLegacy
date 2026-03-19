using UnityEngine;
using System.Collections;

public class ChestUpgrade : MonoBehaviour
{
    public GameObject openPrompt;
    public GameObject rewardItemPrefab;
    public Transform spawnPoint;

    public int skillDamageIncrease = 10;
    public float attackRangeIncrease = 0.2f;
    public int manaCostReduce = 2;

    public bool isOpened = false;

    private bool playerInRange = false;
    private GameObject player;

    void Start()
    {
        if (openPrompt != null)
            openPrompt.SetActive(false);
    }

    void Update()
    {
        if (isOpened) return;

        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            OpenChest();
        }
    }

    void OpenChest()
    {
        isOpened = true;

        if (openPrompt != null)
            openPrompt.SetActive(false);

        StartCoroutine(OpenChestRoutine());
    }

    IEnumerator OpenChestRoutine()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null && HasParameter(anim, "Open"))
        {
            anim.SetTrigger("Open");
        }

        yield return new WaitForSeconds(0.3f);

        GameObject rewardObj = null;

        if (rewardItemPrefab != null && spawnPoint != null)
        {
            rewardObj = Instantiate(rewardItemPrefab, spawnPoint.position, Quaternion.identity);
        }

        if (rewardObj != null)
        {
            float time = 0f;
            Vector3 startPos = rewardObj.transform.position;
            Vector3 endPos = startPos + new Vector3(0f, 1.2f, 0f);

            while (time < 0.6f)
            {
                if (rewardObj == null)
                    yield break;

                time += Time.deltaTime;
                rewardObj.transform.position = Vector3.Lerp(startPos, endPos, time / 0.6f);
                yield return null;
            }
        }

        if (player != null)
        {
            PlayerCombat combat = player.GetComponent<PlayerCombat>();
            PlayerCombat_BuffHeal buffHealCombat = player.GetComponent<PlayerCombat_BuffHeal>();

            if (combat != null)
            {
                combat.UpgradeSkillDamage(skillDamageIncrease);
                combat.UpgradeAttackRange(attackRangeIncrease);
                combat.ReduceSkillManaCost(manaCostReduce);
            }

            if (buffHealCombat != null)
            {
                buffHealCombat.UpgradeBuffExtraDamage(skillDamageIncrease);
                buffHealCombat.UpgradeAttackRange(attackRangeIncrease);
                buffHealCombat.ReduceHealManaCost(manaCostReduce);
                buffHealCombat.UpgradeHealAmount(10);
            }

            PlayerSpeechBubble bubble = player.GetComponent<PlayerSpeechBubble>();
            if (bubble != null)
            {
                bubble.ShowBubble("Skill đã năng cấp!", 2f);
            }
        }

        yield return new WaitForSeconds(0.5f);

        if (rewardObj != null)
            Destroy(rewardObj);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isOpened && other.CompareTag("Player"))
        {
            playerInRange = true;
            player = other.gameObject;

            if (openPrompt != null)
                openPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (openPrompt != null)
                openPrompt.SetActive(false);
        }
    }

    bool HasParameter(Animator animator, string paramName)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }
        return false;
    }
}
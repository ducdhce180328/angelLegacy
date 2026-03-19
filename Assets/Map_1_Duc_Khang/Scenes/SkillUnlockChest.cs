using UnityEngine;

public class SkillUnlockChest : MonoBehaviour
{
    [TextArea]
    public string unlockMessage = "Bạn đã mở khóa kỹ năng L!";

    private bool isOpened = false;
    private Collider2D triggerCollider;
    private Animator anim;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
        anim = GetComponentInParent<Animator>();

        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isOpened) return;

        PlayerController player = other.GetComponent<PlayerController>();

        if (player == null)
        {
            player = other.GetComponentInParent<PlayerController>();
        }

        if (player == null) return;

        isOpened = true;

        player.UnlockSkillL();

        if (anim != null)
        {
            anim.SetBool("IsOpened", true);
        }

        if (triggerCollider != null)
        {
            triggerCollider.enabled = false;
        }

        if (UIManager.instance != null)
        {
            UIManager.instance.ShowUnlockMessage(unlockMessage);
        }

        Debug.Log("Da mo khoa Skill L");
    }
}
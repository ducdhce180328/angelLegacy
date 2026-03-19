using UnityEngine;
using System.Collections;

public class PlayerRespawn : MonoBehaviour
{
    public Transform currentCheckpoint;
    public float respawnDelay = 1.5f;

    private Vector3 startPosition;
    private Health health;
    private Rigidbody2D rb;
    private Animator anim;
    private PlayerController controller;
    private PlayerCombat combat;

    void Start()
    {
        startPosition = transform.position;

        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        controller = GetComponent<PlayerController>();
        combat = GetComponent<PlayerCombat>();
    }

    public void SetCheckpoint(Transform checkpoint)
    {
        currentCheckpoint = checkpoint;
    }

    public void Respawn()
    {
        StopAllCoroutines();
        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        if (controller != null) controller.enabled = false;
        if (combat != null) combat.enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        Vector3 respawnPosition = currentCheckpoint != null ? currentCheckpoint.position : startPosition;
        transform.position = respawnPosition;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (health != null)
        {
            health.isDead = false;
            health.currentHP = health.maxHP;
        }

        if (anim != null)
        {
            anim.ResetTrigger("Death");
            anim.Play("Idle");
        }

        if (controller != null) controller.enabled = true;
        if (combat != null) combat.enabled = true;
    }
}
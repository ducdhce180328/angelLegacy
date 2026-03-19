using UnityEngine;
using System.Collections;

public class UpgradeEffectSpawner : MonoBehaviour
{
    public enum UpgradeEffectType
    {
        HP,
        Mana,
        Attack
    }

    [Header("Effect Prefabs")]
    public GameObject hpEffectPrefab;
    public GameObject manaEffectPrefab;
    public GameObject attackEffectPrefab;

    [Header("Spawn Settings")]
    public Vector3 offset = new Vector3(0f, 0.8f, 0f);
    public float lifeTime = 1.2f;

    private GameObject currentEffect;

    public void PlayEffect(UpgradeEffectType effectType)
    {
        GameObject prefab = GetEffectPrefab(effectType);
        if (prefab == null) return;

        if (currentEffect != null)
            Destroy(currentEffect);

        currentEffect = Instantiate(prefab, transform.position + offset, Quaternion.identity);
        currentEffect.transform.SetParent(transform);

        StartCoroutine(DestroyEffectAfterDelay(currentEffect));
    }

    GameObject GetEffectPrefab(UpgradeEffectType effectType)
    {
        switch (effectType)
        {
            case UpgradeEffectType.HP:
                return hpEffectPrefab;
            case UpgradeEffectType.Mana:
                return manaEffectPrefab;
            case UpgradeEffectType.Attack:
                return attackEffectPrefab;
        }

        return null;
    }

    IEnumerator DestroyEffectAfterDelay(GameObject effectObj)
    {
        yield return new WaitForSeconds(lifeTime);

        if (effectObj != null)
            Destroy(effectObj);
    }
}
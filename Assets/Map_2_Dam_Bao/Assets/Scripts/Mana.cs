using UnityEngine;

public class Mana : MonoBehaviour
{
    public int maxMana = 100;
    public int currentMana;

    void Start()
    {
        currentMana = maxMana;
    }

    public bool UseMana(int amount)
    {
        if (currentMana < amount) return false;

        currentMana -= amount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);
        return true;
    }

    public void RestoreMana(int amount)
    {
        currentMana += amount;
        currentMana = Mathf.Clamp(currentMana, 0, maxMana);
    }
}
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Resources")]
    public int currentLevel;
    public float currentRange;
    public int damage;

    [Header("Global Bonuses")]
    public float globalDamageMultiplier = 1.1f;
    public float globalRangeBonus = 0.5f;
    public float globalAttackSpeedBonus = 1f;

    public void ApplyUpgrade(string upgradeName)
    {
        switch (upgradeName)
        {
            case "IncreaseDamage":
                damage = (int)(damage * globalDamageMultiplier);
                Debug.Log($"Dégâts globaux: x{globalDamageMultiplier}");
                break;

            case "IncreaseRange":
                currentRange += globalRangeBonus;
                Debug.Log($"Portée globale: +{globalRangeBonus}");
                break;

            case "IncreaseAttackSpeed":
                globalAttackSpeedBonus += 0.1f;
                Debug.Log($"Vitesse d'attaque globale: +{globalAttackSpeedBonus * 100}%");
                break;
        }
    }
}
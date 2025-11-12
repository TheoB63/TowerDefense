using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Resources")]
    public int currentLevel = 1;

    [Header("Global Bonuses")]
    public float globalDamageMultiplier = 1f;
    public float globalRangeBonus = 0f;
    public float globalAttackSpeedBonus = 0f;

    public void ApplyUpgrade(string upgradeName)
    {
        switch (upgradeName)
        {
            case "IncreaseDamage":
                globalDamageMultiplier += 0.1f;
                Debug.Log($"Dégâts globaux: x{globalDamageMultiplier}");
                break;

            case "IncreaseRange":
                globalRangeBonus += 0.5f;
                Debug.Log($"Portée globale: +{globalRangeBonus}");
                break;

            case "IncreaseAttackSpeed":
                globalAttackSpeedBonus += 0.1f;
                Debug.Log($"Vitesse d'attaque globale: +{globalAttackSpeedBonus * 100}%");
                break;
        }
    }
}
using Unity.VisualScripting;
using UnityEngine;

public class TowerStats
{
    public TowerType type;
    public int level = 0;
    public int goldCost;
    public int fragmentsCost;

    [Header("Common Stats")]
    public float damage;
    public float cooldown;
    public float range;

    [Header("Special Stats")]
    public float explosionDamage;
    public float damageRange;
    public float duration;
    public float damageInterval;
    public int nbAttack;
    public int penetration;

    public void Create()
    {
        TowerData data = GameRegistry.Instance.GetTowerByType(type);
        level = 1;
        goldCost = data.goldCost;
        fragmentsCost = data.fragmentsCost;

        damage = data.baseDamage;
        cooldown = data.baseCooldown;
        range = data.baseRange;
        explosionDamage = data.baseExplosionDamage;
        damageRange = data.baseDamageRange;
        duration = data.baseDuration;
        damageInterval = data.baseDamageInterval;
        nbAttack = data.baseNbAttack;

        penetration = data.basePenetration;
    }
}

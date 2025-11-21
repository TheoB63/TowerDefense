using Unity.VisualScripting;
using UnityEngine;

public class TowerStats
{
    public TowerType type;
    public int level = 0;

    [Header("Common Stats")]
    public float damage;
    public float cooldown;
    public float range;

    [Header("Special Stats")]
    public float explosionDamage;
    public float damageRange;
    public float duration;
    public float damageInterval;
    public float nbAttack;
    public float penetration;

    public void Create()
    {
        TowerData data = GameRegistry.Instance.GetTowerByType(type);
        level = 1;
        damage = data.baseDamage;
        cooldown = data.baseCooldown;
        range = data.baseRange;
        explosionDamage = data.baseExplosionDamage;
        damageRange = data.baseDamageRange;
        duration = data.baseDuration;
        damageInterval = data.baseDamageInterval;
        nbAttack = data.baseNbAttack;

        Debug.Log(data.type.ToString() + " p = " + data.basePenetration);
        penetration = data.basePenetration;
    }
}

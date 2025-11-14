using System.Collections.Generic;
using UnityEngine;

public class RailgunTower : MonoBehaviour,ITower
{
    [Header("Tower Settings")]
    public GameObject projectilesPrefab;
    int baseDamage = 10;
    float baseRange = 20f; 
    float baseAttackSpeed = 8f;

    [Header("Upgrade Levels")]
    int damageLevel = 2;
    int rangeLevel = 2;
    int attackSpeedLevel = 8;
    public bool hasMultiTarget = false;

    private float currentDamage;
    private float currentRange;
    private float currentAttackSpeed;
    private Transform currentTarget;

    float m_timerShoot;

    private void Start()
    {
        UpdateStats();
    }

    private void UpdateStats()
    {
        currentDamage = baseDamage * Mathf.Pow(1.5f, damageLevel);
        currentRange = baseRange + (rangeLevel * 1f);
        currentAttackSpeed = baseAttackSpeed + (attackSpeedLevel * 1f);
    }

    void Update()
    {
        m_timerShoot += Time.deltaTime;

        if (m_timerShoot > (1/ baseAttackSpeed))
        {
            if (Shoot())
            {
                m_timerShoot = 0;
            }
        }
    }

    bool Shoot()
    {
        if (GameManager.Instance.Enemies.Count > 0)
        {
            Enemy target = GetNearestEnemy();

            float dist = Vector2.Distance(transform.position, target.transform.position);

            if (dist <= baseDamage)
            {
                //need to add nbProjectiles

                Vector2 direction = target.transform.position - transform.position;
                direction.Normalize();

                GameObject go = Instantiate(projectilesPrefab, transform.position, Quaternion.identity);
                go.GetComponent<Projectiles>().Direction = direction;
                go.GetComponent<Projectiles>().SetStats(baseDamage, 0);

                return true;
            }
        }
        return false;
    }

    private Enemy GetNearestEnemy()
    {
        Enemy target = GameManager.Instance.Enemies[0];

        float dist = float.MaxValue;
        foreach (Enemy enemy in GameManager.Instance.Enemies)
        {
            float newDist = Vector2.Distance(transform.position, enemy.transform.position);
            if (newDist < dist)
            {
                target = enemy;
                dist = newDist;
            }
        }

        return target;
    }

    public TowerType GetTowerType() => TowerType.Projectile;
    public string GetTowerName() => "Basic Tower";

    public void ApplyUpgrade(string upgradeName)
    {
        switch (upgradeName)
        {
            case "IncreaseDamage":
                damageLevel++;
                break;
            case "IncreaseRange":
                rangeLevel++;
                break;
            case "AddMultiTarget":
                // Logique pour cibler plusieurs ennemis
                break;
        }
        UpdateStats();
    }

    public List<string> GetAvailableUpgrades()
    {
        var upgrades = new List<string>();
        /*if (damageLevel < 5) */
        upgrades.Add("IncreaseDamage");
        /*if (rangeLevel < 3)*/ 
        upgrades.Add("IncreaseRange");
        return upgrades;
    }
}

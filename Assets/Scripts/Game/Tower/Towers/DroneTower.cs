using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DroneTower : Tower
{
    [Header("Spawner Settings")]
    [SerializeField] int m_baseMaxDrone;
    [SerializeField] float m_baseSpawnCooldown;
    [SerializeField] float m_baseReloadCooldown;
    [SerializeField] int m_baseNbAttack;

    [Header("Upgrades")]
    [SerializeField] int m_maxDroneIncrease;
    [SerializeField] float m_spawnCooldownDecrease;
    [SerializeField] float m_reloadCooldownDecrease;
    [SerializeField] int m_nbAttackIncrease;

    //Privates Current values in run time
    int m_currentMaxDrone;
    float m_currentSpawnCooldown;
    float m_currentReloadCooldown;
    int m_currentNbAttack;

    int m_nbDroneSpawn; //nb spawn before reload

    //Timer
    float m_timerSpawn;
    float m_timerReload;

    private void Start()
    {
        InitializeTower();
    }

    protected override void InitializeTower()
    {
        base.InitializeTower();
        m_currentMaxDrone = m_baseMaxDrone;
        m_currentSpawnCooldown = m_baseSpawnCooldown;
        m_currentReloadCooldown = m_baseReloadCooldown;
        m_currentNbAttack = m_baseNbAttack;
    }

    private void Update()
    {
        MoveLauncher();

        if (m_timerReload <= 0)
        {
            m_timerSpawn -= Time.deltaTime;

            if (m_timerSpawn <= 0)
            {
                Spawn();
                m_nbDroneSpawn++;
                if (m_nbDroneSpawn >= m_currentMaxDrone)
                {
                    m_timerReload = m_currentReloadCooldown;
                }
            }
        }
        else
        {
            m_timerReload -= Time.deltaTime;
        }

    }

    protected override void MoveLauncher()
    {
        m_launcher.Rotate(0, 0, m_armRotationSpeed * Time.deltaTime);
    }

    void Spawn()
    {
        GameObject go = Instantiate(m_projectilesPrefab, m_barrel.position, Quaternion.identity);
        go.GetComponent<Drone>().SetStats(m_currentDamage, m_currentNbAttack);
        Debug.Log("spawn drone");
    }

    public override void ApplyUpgrade(string upgradeName)
    {
        switch (upgradeName)
        {
            case "IncreaseMaxDrones":
                IncreaseMaxDrones();
                break;

            case "ReduceSpawnTime":
                break;

            case "ImproveDroneHealth":
                break;
        }
    }

    public override List<UpgradeBase> GetAvailableUpgrades()
    {
        List<UpgradeBase> available = new List<UpgradeBase>();

        foreach (TowerUpgrade upgrade in m_towerUpgrades)
        {
            TowerUpgrade copy = Instantiate(upgrade);
            available.Add(copy);
        }
        return available;
    }

    private void IncreaseMaxDrones()
    {
        m_currentMaxDrone += m_maxDroneIncrease;
    }
}